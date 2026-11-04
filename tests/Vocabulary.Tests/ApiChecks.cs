using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api;
using Application;
using Application.Common.Exceptions;
using Application.Vocabulary.Models;
using Application.Vocabulary.Queries.GetVocabularyEntries;
using Application.Vocabulary.Queries.GetVocabularyEntry;
using Application.Vocabulary.Queries.GetVocabularySenses;
using Domain.Languages;
using Domain.Vocabulary;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vocabulary.Tests;

internal static class ApiChecks
{
    public static async Task RunAsync(string database)
    {
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
        await using var app = CreateApp(database, 10000);
        var now = new DateTimeOffset(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Languages.AddRange(Language.Create(10000, "en", "English", true, 0, now),
                Language.Create(10010, "zz", "Empty", true, 0, now), Language.Create(10020, "es", "Español", false, 0, now));
            db.VocabularyEntries.AddRange(
                VocabularyEntry.Create(10001, 10000, "book", "book", null, "https://media.example/book.mp3", true, 0, now),
                VocabularyEntry.Create(10002, 10000, "water", "water", "water", null, true, 0, now),
                VocabularyEntry.Create(10003, 10000, "without-senses", "draft wording", null, null, true, 1, now),
                VocabularyEntry.Create(10004, 10000, "hidden", "hidden", null, null, false, 2, now),
                VocabularyEntry.Create(10021, 10020, "book", "libro", null, null, true, 0, now));
            db.VocabularySenses.AddRange(
                VocabularySense.Create(10101, 10001, "publication", PartOfSpeech.Noun, 0, now),
                VocabularySense.Create(10102, 10001, "reserve", PartOfSpeech.Verb, 0, now),
                VocabularySense.Create(10201, 10002, "liquid", PartOfSpeech.Noun, 0, now),
                VocabularySense.Create(10401, 10004, "hidden-meaning", PartOfSpeech.Noun, 0, now),
                VocabularySense.Create(10501, 10021, "publication", PartOfSpeech.Noun, 0, now));
            db.VocabularySenseTranslations.AddRange(
                VocabularySenseTranslation.Create(11101, 10101, "vi", "Cuốn sách.", "Ghi chú nghĩa danh từ.", now),
                VocabularySenseTranslation.Create(11102, 10101, "en", "Written publication.", "An English note.", now),
                VocabularySenseTranslation.Create(11103, 10101, "en-us", "US publication.", null, now),
                VocabularySenseTranslation.Create(11104, 10101, "fr", "Livre.", null, now),
                VocabularySenseTranslation.Create(11201, 10102, "vi", "Đặt trước.", "Nghĩa động từ.", now),
                VocabularySenseTranslation.Create(11202, 10102, "en", "Reserve a place.", "A verb.", now),
                VocabularySenseTranslation.Create(11301, 10201, "fr", "Eau.", null, now),
                VocabularySenseTranslation.Create(11401, 10401, "vi", "Không công khai.", null, now),
                VocabularySenseTranslation.Create(11501, 10501, "vi", "Danh mục bị tắt.", null, now));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = Client(app);
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        const string root = "/api/v1/languages/en/vocabulary";
        async Task<HttpResponseMessage> Get(string path, int status = 200, int queries = 1, string user = "a", string? key = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "vocabulary-test-" + user);
            key ??= Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode != status)
                throw new InvalidOperationException($"GET {path}: expected {status}, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            Check.That(true, $"Status for GET {path}.");
            Check.That(counter.Count(key) == queries, $"SQL budget GET {path}: expected {queries}, got {counter.Count(key)}.");
            return response;
        }
        async Task<VocabularyEntryPage> Entries(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<VocabularyEntryPage>(json))!;
        async Task<VocabularyEntryDto> Entry(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<VocabularyEntryDto>(json))!;
        async Task<VocabularySensePage> Senses(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<VocabularySensePage>(json))!;

        await Get(root, 401, 0, "none");
        await Get(root + "/book", 401, 0, "none");
        await Get(root + "/book/senses", 401, 0, "none");
        await Get("/languages/en/vocabulary", 404, 0);
        var response = await Get(root + "?locale=vi", key: "list-projection");
        var page = await Entries(response);
        Check.That(response.Headers.CacheControl?.NoStore == true && response.Headers.Pragma.Any(x => x.Name == "no-cache"), "Private catalog cache headers.");
        Check.That(page.Items.Count == 3 && page.NextCursor is null, "Only published entries are listed.");
        Check.That(page.Items.Select(x => x.Id).SequenceEqual(new[] { "10001", "10002", "10003" }), "Stable sort order/ID tie-break.");
        Check.That(!counter.Sql("list-projection").Contains("usage_note", StringComparison.OrdinalIgnoreCase) &&
                   counter.Sql("list-projection").Contains("LIMIT", StringComparison.OrdinalIgnoreCase) &&
                   !counter.Sql("list-projection").Contains("OFFSET", StringComparison.OrdinalIgnoreCase), "List projects previews only and uses keyset/limit.");
        var book = page.Items[0];
        Check.That(book.LanguageCode == "en" && book.Term == "book" && book.Reading is null && book.AudioUrl == "https://media.example/book.mp3", "Entry metadata and nullable reading survive HTTP.");
        Check.That(book.PrimarySense is { Code: "publication", Meaning: "Cuốn sách.", TranslationLocale: "vi", IsTranslationFallback: false, PartOfSpeech: PartOfSpeech.Noun }, "Primary sense uses stable first sense and exact locale.");
        Check.That(page.Items[1].PrimarySense is { Meaning: null, TranslationLocale: null, IsTranslationFallback: true }, "Missing requested/English translation is null, not another locale or fabricated meaning.");
        Check.That(page.Items[2].PrimarySense is null, "Published entry with no senses stays visible without a fake preview.");
        var defaultLocale = await Entries(await Get(root));
        Check.That(defaultLocale.Items[0].PrimarySense is { Meaning: "Written publication.", TranslationLocale: "en", IsTranslationFallback: false }, "Omitted locale defaults to English.");
        var normalized = await Entries(await Get(root.Replace("/en/", "/EN/") + "?locale=%20VI%20"));
        Check.That(normalized.Items[0].PrimarySense!.Meaning == "Cuốn sách." && normalized.Items[0].LanguageCode == "en", "Language/locale normalize consistently.");
        var englishRegional = await Entries(await Get(root + "?locale=EN-US"));
        Check.That(englishRegional.Items[0].PrimarySense is { Meaning: "US publication.", TranslationLocale: "en-us", IsTranslationFallback: false }, "Exact regional locale wins.");
        var regionalFallback = await Entries(await Get(root + "?locale=fr-ca"));
        Check.That(regionalFallback.Items[0].PrimarySense is { Meaning: "Written publication.", TranslationLocale: "en", IsTranslationFallback: true }, "Fallback uses English; no implicit parent-locale fallback.");
        var shared = await Entries(await Get(root + "?locale=vi", user: "b"));
        Check.That(shared.Items.Select(x => x.Id).SequenceEqual(page.Items.Select(x => x.Id)), "Catalog is shared between authenticated users, not owned per profile.");
        var verbs = await Entries(await Get(root + "?locale=vi&partOfSpeech=Verb"));
        Check.That(verbs.Items.Count == 1 && verbs.Items[0].PrimarySense is { Code: "reserve", PartOfSpeech: PartOfSpeech.Verb, Meaning: "Đặt trước." }, "Filter matches any sense and previews the first matching sense.");
        var noMatch = await Entries(await Get(root + "?partOfSpeech=Adjective", queries: 2));
        Check.That(noMatch.Items.Count == 0 && noMatch.NextCursor is null, "Empty POS filter returns 200 for an enabled language.");

        var first = await Entries(await Get(root + "?pageSize=1&locale=vi"));
        var second = await Entries(await Get(root + "?pageSize=1&locale=vi&after=" + first.NextCursor));
        var third = await Entries(await Get(root + "?pageSize=1&locale=vi&after=" + second.NextCursor));
        Check.That(first.Items[0].Id == "10001" && second.Items[0].Id == "10002" && third.Items[0].Id == "10003" && third.NextCursor is null, "Cursor handles tied sort orders with no duplicates.");
        Check.That((await Entries(await Get(root + "?after=1:10003", queries: 2))).Items.Count == 0, "Catalog cursor end is an empty page.");
        var detail = await Entry(await Get(root + "/BOOK"));
        Check.That(detail.Id == book.Id && detail.Code == "book" && detail.Term == "book", "Detail uses normalized stable code.");
        var emptyWord = await Senses(await Get(root + "/without-senses/senses", queries: 2));
        Check.That(emptyWord.Items.Count == 0 && emptyWord.NextCursor is null, "Existing published entry may have no senses.");
        var senses = await Senses(await Get(root + "/book/senses?locale=vi"));
        Check.That(senses.Items.Select(x => x.Code).SequenceEqual(new[] { "publication", "reserve" }), "Detail returns all senses in sort/ID order.");
        Check.That(senses.Items[0].UsageNote == "Ghi chú nghĩa danh từ." && senses.Items[1].PartOfSpeech == PartOfSpeech.Verb && senses.Items[1].Meaning == "Đặt trước.", "Meaning/POS/usage note remain aligned by sense.");
        var fr = await Senses(await Get(root + "/book/senses?locale=fr"));
        Check.That(fr.Items[0].Meaning == "Livre." && !fr.Items[0].IsTranslationFallback &&
                   fr.Items[1].Meaning == "Reserve a place." && fr.Items[1].TranslationLocale == "en" && fr.Items[1].IsTranslationFallback,
            "Fallback is resolved independently per sense.");
        var missing = await Senses(await Get(root + "/water/senses?locale=vi"));
        Check.That(missing.Items[0].Meaning is null && missing.Items[0].UsageNote is null && missing.Items[0].TranslationLocale is null, "Missing translation does not leak a different language's note.");
        var senseFirst = await Senses(await Get(root + "/book/senses?pageSize=1"));
        var senseSecond = await Senses(await Get(root + "/book/senses?pageSize=1&after=" + senseFirst.NextCursor));
        Check.That(senseFirst.Items[0].Id == "10101" && senseSecond.Items[0].Id == "10102" && senseSecond.NextCursor is null, "Sense cursor handles tied sort orders.");
        Check.That((await Senses(await Get(root + "/book/senses?after=0:10102", queries: 2))).Items.Count == 0, "Sense cursor end is an empty page.");
        var japanese = await Senses(await Get("/api/v1/languages/ja/vocabulary/kiku/senses?locale=vi"));
        Check.That(japanese.Items.Count == 2 && japanese.Items.All(x => x.Meaning is not null && x.TranslationLocale == "vi"), "Existing Japanese multi-sense seed works through the API.");

        await Get(root + "/hidden", 404);
        await Get(root + "/hidden/senses", 404, 2);
        await Get(root + "/missing", 404);
        await Get(root + "/missing/senses", 404, 2);
        await Get("/api/v1/languages/xx/vocabulary", 404, 2);
        await Get("/api/v1/languages/xx/vocabulary/book", 404);
        await Get("/api/v1/languages/xx/vocabulary/book/senses", 404, 2);
        await Get("/api/v1/languages/es/vocabulary", 404, 2);
        await Get("/api/v1/languages/es/vocabulary/book", 404);
        await Get("/api/v1/languages/es/vocabulary/book/senses", 404, 2);
        Check.That((await Entries(await Get("/api/v1/languages/zz/vocabulary", queries: 2))).Items.Count == 0, "Enabled empty language returns an empty catalog.");
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "pageSize=abc", "after=-1:1", "after=0:0", "after=0:9223372036854775808", "after=bad", "after=2147483648:1", "locale=en_US", "locale=x", "locale=en--us" })
        {
            await Get(root + "?" + query, 400, 0);
            await Get(root + "/book/senses?" + query, 400, 0);
        }
        await Get(root + "?locale=" + new string('a', 36), 400, 0);
        await Get(root + "?partOfSpeech=Unknown", 400, 0);
        await Get(root + "?partOfSpeech=99", 400, 0);
        await Get(root + "/1bad", 400, 0);
        await Get(root + "/bad_code/senses", 400, 0);
        await Get("/api/v1/languages/en_US/vocabulary", 400, 0);

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var entriesHandler = scope.ServiceProvider.GetRequiredService<GetVocabularyEntriesQueryHandler>();
            await Check.ThrowsAsync<RequestValidationException>(() => entriesHandler.Handle(new("en", PartOfSpeech: (PartOfSpeech)99), default), "Application validates enum independent of HTTP binder.");
            await Check.ThrowsAsync<RequestValidationException>(() => entriesHandler.Handle(new("en", PageSize: 0), default), "Application validates page bounds independent of HTTP binder.");
            var senseHandler = scope.ServiceProvider.GetRequiredService<GetVocabularySensesQueryHandler>();
            await Check.ThrowsAsync<RequestValidationException>(() => senseHandler.Handle(new("en", "book", After: "bad"), default), "Application validates cursor independent of HTTP binder.");
            var entryHandler = scope.ServiceProvider.GetRequiredService<GetVocabularyEntryQueryHandler>();
            await Check.ThrowsAsync<RequestValidationException>(() => entryHandler.Handle(new("en", "bad_code"), default), "Application validates entry codes independent of HTTP binder.");
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var translation = await db.VocabularySenseTranslations.SingleAsync(x => x.Id == 11101);
            translation.Update("Sách đã sửa.", "Ghi chú mới.", now.AddMinutes(2));
            await db.SaveChangesAsync();
        }
        var refreshed = await Entries(await Get(root + "?locale=vi"));
        Check.That(refreshed.Items[0].PrimarySense!.Meaning == "Sách đã sửa." && refreshed.Items[0].UpdatedAt == now.AddMinutes(2) &&
                   refreshed.Items[0].PrimarySense!.UpdatedAt == now.AddMinutes(2) && refreshed.Items[0].Version == 1, "Effective preview UpdatedAt includes selected translation without inventing an aggregate version.");
        Check.That((await Entry(await Get(root + "/book"))).UpdatedAt == now, "Metadata detail timestamp refers only to entry metadata.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var entry = await db.VocabularyEntries.SingleAsync(x => x.Id == 10001);
            entry.Update(entry.Term, entry.Reading, entry.AudioUrl, false, entry.SortOrder, now.AddMinutes(3));
            await db.SaveChangesAsync();
        }
        await Get(root + "/book", 404);
        await Get(root + "/book/senses", 404, 2);
        Check.That((await Entries(await Get(root))).Items.All(x => x.Id != "10001"), "Unpublishing parent hides every sense and preview.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var language = await db.Languages.SingleAsync(x => x.Id == 10000);
            language.Update(language.NativeName, false, language.SortOrder, now.AddMinutes(4));
            await db.SaveChangesAsync();
            Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 &&
                       await db.WritingSystemItemProgress.CountAsync() == 1, "Vocabulary GET flows preserve personal learning state.");
        }
        await Get(root, 404, 2);
        await Get(root + "/water", 404);
        await Get(root + "/water/senses", 404, 2);
        Check.That(!counter.HasWrites, "All HTTP vocabulary flows are read-only.");
        await app.StopAsync();

        await using var limited = CreateApp(database, 1);
        await limited.StartAsync();
        using var limitedClient = Client(limited);
        limitedClient.DefaultRequestHeaders.Authorization = new("Bearer", "vocabulary-test-a");
        Check.That((await limitedClient.GetAsync("/api/v1/languages/ja/vocabulary/kiku")).IsSuccessStatusCode, "First rate-limited request succeeds.");
        using var limitedRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/languages/ja/vocabulary/kiku/senses");
        limitedRequest.Headers.Add("X-Test-Query", "rate-rejected");
        var rejected = await limitedClient.SendAsync(limitedRequest);
        Check.That((int)rejected.StatusCode == 429 && rejected.Headers.RetryAfter is not null, "Vocabulary endpoints share learning-content policy with Retry-After.");
        Check.That(limited.Services.GetRequiredService<SqlCommandCounter>().Count("rate-rejected") == 0, "Rate rejection happens before feature DB reads.");
        await limited.StopAsync();
    }

    private static HttpClient Client(WebApplication app) => new()
    {
        BaseAddress = new Uri(app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single())
    };

    private static WebApplication CreateApp(string database, int permitLimit)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development", Args = ["--urls", "http://127.0.0.1:0"] });
        builder.Configuration.AddJsonFile(Path.GetFullPath("../../../../../src/Api/appsettings.json", AppContext.BaseDirectory))
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database, ["SnowflakeId:GeneratorId"] = "1020",
                ["Authentication:Google:ClientId"] = "test.apps.googleusercontent.com", ["Authentication:Google:ClientSecret"] = "test-only",
                ["Authentication:Microsoft:ClientId"] = "11111111-1111-1111-1111-111111111111", ["Authentication:Microsoft:ClientSecret"] = "test-only",
                ["Authentication:Facebook:AppId"] = "123456789", ["Authentication:Facebook:AppSecret"] = "test-only",
                ["Authentication:Jwt:SigningKey"] = "local-vocabulary-test-only-signing-key-with-more-than-32-bytes",
                ["AI:KeyProtection:Keys:primary"] = Convert.ToBase64String(new byte[32]),
                ["AI:KeyProtection:FingerprintKey"] = Convert.ToBase64String(Enumerable.Repeat((byte)1, 32).ToArray()),
                ["Storage:R2:ServiceUrl"] = "https://11111111111111111111111111111111.r2.cloudflarestorage.com",
                ["Storage:R2:BucketName"] = "vocabulary-tests", ["Storage:R2:AccessKeyId"] = "test-only", ["Storage:R2:SecretAccessKey"] = "test-only",
                ["RateLimiting:LearningContent:PermitLimit"] = permitLimit.ToString()
            });
        builder.Logging.ClearProviders().AddSimpleConsole().SetMinimumLevel(LogLevel.Warning)
            .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None).AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApi(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(Api.Controllers.VocabularyController).Assembly);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "VocabularyTests";
            options.DefaultChallengeScheme = "VocabularyTests";
        }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("VocabularyTests", _ => { });
        builder.Services.AddSingleton<SqlCommandCounter>();
        builder.Services.AddDbContext<ApplicationDbContext>((services, options) => options.AddInterceptors(services.GetRequiredService<SqlCommandCounter>()));
        var app = builder.Build();
        app.UseApi();
        return app;
    }
}
