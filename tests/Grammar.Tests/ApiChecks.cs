using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Exceptions;
using Application.Grammar.Models;
using Application.Grammar.Queries.GetGrammarPoint;
using Application.Grammar.Queries.GetGrammarPoints;
using Domain.Grammar;
using Domain.Languages;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grammar.Tests;

internal static class ApiChecks
{
    public static async Task RunAsync(string database)
    {
        await using var app = TestApiHost.Build(database);
        var now = DomainChecks.Now;
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Languages.AddRange(Language.Create(10000, "en", "English", true, 0, now),
                Language.Create(10010, "zz", "Empty", true, 0, now), Language.Create(10020, "es", "Español", false, 0, now));
            db.GrammarPoints.AddRange(
                GrammarPoint.Create(10001, 10000, "noun-copula", "Native A", "N is N", true, 0, now),
                GrammarPoint.Create(10002, 10000, "question-ka", "Native B", "Question", true, 0, now),
                GrammarPoint.Create(10003, 10000, "no-translation", "Native C", "Pattern", true, 1, now),
                GrammarPoint.Create(10004, 10000, "hidden", "Hidden", "Secret", false, 2, now),
                GrammarPoint.Create(10021, 10020, "noun-copula", "Hidden language", "Secret", true, 0, now));
            db.GrammarPointTranslations.AddRange(
                GrammarPointTranslation.Create(11001, 10001, "vi", "Tên Việt", "Giải thích Việt.", "Lưu ý Việt.", now),
                GrammarPointTranslation.Create(11002, 10001, "en", "English title", "English explanation.", "English note.", now),
                GrammarPointTranslation.Create(11003, 10001, "en-us", "US title", "US explanation.", null, now),
                GrammarPointTranslation.Create(11004, 10001, "fr", "Titre", "Explication.", "Note française.", now),
                GrammarPointTranslation.Create(11005, 10002, "fr", "Autre titre", "Autre explication.", null, now),
                GrammarPointTranslation.Create(11006, 10004, "vi", "Bản nháp", "Nội dung nháp.", null, now),
                GrammarPointTranslation.Create(11007, 10021, "vi", "Ngôn ngữ bị tắt", "Nội dung bị ẩn.", null, now));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = TestApiHost.Client(app);
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        const string root = "/api/v1/languages/en/grammar-points";
        async Task<HttpResponseMessage> Get(string path, int status = 200, int queries = 1, string user = "a", string? key = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "grammar-test-" + user);
            key ??= Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode != status)
                throw new InvalidOperationException($"GET {path}: expected {status}, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            Check.That(true, "HTTP status.");
            Check.That(counter.Count(key) == queries, $"Expected {queries} SQL commands, got {counter.Count(key)}: {counter.Sql(key)}");
            return response;
        }
        async Task<GrammarPointPage> Page(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<GrammarPointPage>())!;
        async Task<GrammarPointDto> Detail(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<GrammarPointDto>())!;

        await Get(root, 401, 0, "none");
        await Get(root + "/noun-copula", 401, 0, "none");
        await Get("/languages/en/grammar-points", 404, 0);
        var response = await Get(root + "?locale=vi", key: "list-shape");
        var page = await Page(response);
        Check.That(response.Headers.CacheControl?.NoStore == true && response.Headers.Pragma.Any(x => x.Name == "no-cache"), "Private catalog caching headers.");
        Check.That(page.Items.Select(x => x.Id).SequenceEqual(new[] { "10001", "10002", "10003" }) && page.NextCursor is null, "Published list sorted by order/ID, including tied sort orders.");
        Check.That(page.Items[0] is { Name: "Tên Việt", Pattern: "N is N", TranslationLocale: "vi", IsTranslationFallback: false }, "List returns selected title and source pattern.");
        Check.That(page.Items[1] is { Name: "Native B", TranslationLocale: null, IsTranslationFallback: true } && page.Items[2].Name == "Native C", "Missing requested/English translation falls back to the native name.");
        Check.That(!counter.Sql("list-shape").Contains("explanation", StringComparison.OrdinalIgnoreCase) &&
            !counter.Sql("list-shape").Contains("usage_note", StringComparison.OrdinalIgnoreCase) &&
            counter.Sql("list-shape").Contains("LIMIT", StringComparison.OrdinalIgnoreCase) &&
            !counter.Sql("list-shape").Contains("OFFSET", StringComparison.OrdinalIgnoreCase), "Bounded list projection omits long content.");
        var detail = await Detail(await Get(root + "/noun-copula?locale=vi", key: "detail-shape"));
        Check.That(detail.Explanation == "Giải thích Việt." && detail.UsageNote == "Lưu ý Việt." && detail.Pattern == page.Items[0].Pattern, "Detail keeps explanation/note aligned to the selected translation.");
        Check.That(counter.Sql("detail-shape").Contains("explanation", StringComparison.OrdinalIgnoreCase), "Detail loads explanation in its single SQL query.");
        var english = await Detail(await Get(root + "/noun-copula"));
        Check.That(english.Name == "English title" && english.TranslationLocale == "en" && !english.IsTranslationFallback, "Omitted locale defaults to English, following catalog convention.");
        var normalized = await Detail(await Get(root.Replace("/en/", "/EN/") + "/NOUN-COPULA?locale=%20VI%20"));
        Check.That(normalized.LanguageCode == "en" && normalized.Code == "noun-copula" && normalized.Name == "Tên Việt", "Normalize language, code and locale consistently.");
        var regional = await Detail(await Get(root + "/noun-copula?locale=EN-US"));
        Check.That(regional.Name == "US title" && regional.TranslationLocale == "en-us" && !regional.IsTranslationFallback, "Exact regional translation wins.");
        var regionalFallback = await Detail(await Get(root + "/noun-copula?locale=fr-ca"));
        Check.That(regionalFallback.Name == "English title" && regionalFallback.Explanation == "English explanation." && regionalFallback.UsageNote == "English note." &&
            regionalFallback.TranslationLocale == "en" && regionalFallback.IsTranslationFallback, "Fallback is exact to English, without implicit parent locale matching.");
        foreach (var code in new[] { "question-ka", "no-translation" })
        {
            var missing = await Detail(await Get(root + "/" + code + "?locale=vi"));
            Check.That(missing.Explanation is null && missing.UsageNote is null && missing.TranslationLocale is null && missing.IsTranslationFallback,
                "Missing content is represented explicitly and does not leak a different locale.");
        }
        Check.That((await Detail(await Get(root + "/question-ka?locale=fr"))).Name == "Autre titre", "A non-English locale can be selected exactly.");
        var userB = await Page(await Get(root + "?locale=vi", user: "b"));
        Check.That(JsonSerializer.Serialize(userB) == JsonSerializer.Serialize(page), "Authenticated users share the catalog.");
        var first = await Page(await Get(root + "?pageSize=1"));
        var second = await Page(await Get(root + "?pageSize=1&after=" + first.NextCursor));
        var third = await Page(await Get(root + "?pageSize=1&after=" + second.NextCursor));
        Check.That(first.Items[0].Id == "10001" && second.Items[0].Id == "10002" && third.Items[0].Id == "10003" && third.NextCursor is null, "Keyset pages traverse tied sort orders without duplicates.");
        Check.That((await Page(await Get(root + "?after=1:10003", queries: 2))).Items.Count == 0, "Cursor at end returns empty page for an enabled language.");
        var seed = await Page(await Get("/api/v1/languages/ja/grammar-points?locale=vi&pageSize=100"));
        Check.That(seed.Items.Count == 50 && seed.Items.All(x => x.TranslationLocale == "vi" && !x.IsTranslationFallback && long.Parse(x.Id) > 9007199254740991), "All fifty Japanese seed points are reachable with string Snowflake IDs.");
        foreach (var point in seed.Items)
        {
            var seededDetail = await Detail(await Get("/api/v1/languages/ja/grammar-points/" + point.Code + "?locale=vi"));
            Check.That(seededDetail.Explanation is not null && seededDetail.Pattern == point.Pattern, "Every seed point has accessible translated detail.");
        }
        foreach (var path in new[] { root + "/hidden", root + "/missing", "/api/v1/languages/es/grammar-points/noun-copula", "/api/v1/languages/xx/grammar-points/noun-copula" })
            await Get(path, 404);
        await Get("/api/v1/languages/es/grammar-points", 404, 2);
        await Get("/api/v1/languages/xx/grammar-points", 404, 2);
        Check.That((await Page(await Get("/api/v1/languages/zz/grammar-points", queries: 2))).Items.Count == 0, "Enabled language without points returns 200 empty.");
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "pageSize=abc", "after=bad", "after=-1:1", "after=0:0", "after=0:9223372036854775808", "after=2147483648:1" })
            await Get(root + "?" + query, 400, 0);
        foreach (var query in new[] { "locale=en_US", "locale=x", "locale=en--us", "locale=" + new string('a', 36) })
        {
            await Get(root + "?" + query, 400, 0);
            await Get(root + "/noun-copula?" + query, 400, 0);
        }
        await Get(root + "/bad_code", 400, 0);
        await Get(root + "/1bad", 400, 0);
        await Get("/api/v1/languages/en_US/grammar-points", 400, 0);
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var listHandler = scope.ServiceProvider.GetRequiredService<GetGrammarPointsQueryHandler>();
            try { await listHandler.Handle(new("en", PageSize: 0), default); throw new InvalidOperationException("Application accepted invalid page size."); }
            catch (RequestValidationException) { Check.That(true, "Application validation is independent of HTTP."); }
            var detailHandler = scope.ServiceProvider.GetRequiredService<GetGrammarPointQueryHandler>();
            try { await detailHandler.Handle(new("en", "bad_code"), default); throw new InvalidOperationException("Application accepted invalid code."); }
            catch (RequestValidationException) { Check.That(true, "Detail handler validates code."); }
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var translation = await db.GrammarPointTranslations.SingleAsync(x => x.Id == 11001);
            translation.Update("Tên đã sửa", "Nội dung mới", null, now.AddMinutes(2));
            await db.SaveChangesAsync();
        }
        var updated = await Detail(await Get(root + "/noun-copula?locale=vi"));
        var updatedList = await Page(await Get(root + "?locale=vi"));
        Check.That(updated.Name == "Tên đã sửa" && updated.Explanation == "Nội dung mới" && updated.UsageNote is null &&
            updated.UpdatedAt == now.AddMinutes(2) && updated.Version == 1 && updatedList.Items[0].UpdatedAt == updated.UpdatedAt, "Selected translation changes effective timestamp while Version remains the point's version.");
        Check.That((await Detail(await Get(root + "/noun-copula?locale=en"))).UpdatedAt == now, "Other locale edits leave this response timestamp unchanged.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var point = await db.GrammarPoints.SingleAsync(x => x.Id == 10001);
            point.Update(point.NativeName, "Updated pattern", true, point.SortOrder, now.AddMinutes(3));
            await db.SaveChangesAsync();
        }
        Check.That((await Detail(await Get(root + "/noun-copula?locale=vi"))) is { Pattern: "Updated pattern", Version: 2 } newer && newer.UpdatedAt == now.AddMinutes(3), "Parent edits update pattern/version and effective timestamp.");
        var concurrent = await Task.WhenAll(Enumerable.Range(0, 8).Select(async _ =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/languages/ja/grammar-points?locale=vi");
            request.Headers.Authorization = new("Bearer", "grammar-test-a");
            using var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : throw new InvalidOperationException("Concurrent GET failed.");
        }));
        Check.That(concurrent.Distinct().Count() == 1, "Parallel reads return consistent content.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var point = await db.GrammarPoints.SingleAsync(x => x.Id == 10001);
            point.Update(point.NativeName, point.Pattern, false, point.SortOrder, now.AddMinutes(4));
            await db.SaveChangesAsync();
        }
        await Get(root + "/noun-copula", 404);
        Check.That((await Page(await Get(root))).Items.All(x => x.Id != "10001"), "Unpublishing hides the parent and all translations.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var language = await db.Languages.SingleAsync(x => x.Id == 10000);
            language.Update(language.NativeName, false, language.SortOrder, now.AddMinutes(5));
            await db.SaveChangesAsync();
            Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1, "GET flows preserve private user/profile state.");
        }
        await Get(root, 404, 2);
        await Get(root + "/question-ka", 404);
        Check.That(!counter.HasWrites, "Grammar HTTP endpoints execute SELECT statements only.");
        await app.StopAsync();
        await using var limited = TestApiHost.Build(database, 1);
        await limited.StartAsync();
        using var limitedClient = TestApiHost.Client(limited);
        limitedClient.DefaultRequestHeaders.Authorization = new("Bearer", "grammar-test-a");
        using var accepted = await limitedClient.GetAsync("/api/v1/languages/ja/grammar-points");
        Check.That(accepted.IsSuccessStatusCode, "First catalog request is accepted.");
        using var blockedRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/languages/ja/grammar-points/noun-copula");
        blockedRequest.Headers.Add("X-Test-Query", "rate-blocked");
        using var blocked = await limitedClient.SendAsync(blockedRequest);
        Check.That((int)blocked.StatusCode == 429 && blocked.Headers.RetryAfter is not null && blocked.Headers.CacheControl?.NoStore == true,
            "List and detail share learning-content rate limits with Retry-After.");
        Check.That(limited.Services.GetRequiredService<SqlCommandCounter>().Count("rate-blocked") == 0, "Rate limit rejection happens before feature SQL.");
        await limited.StopAsync();
    }
}
