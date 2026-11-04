using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Handwriting.Models;
using Domain.Handwriting;
using Domain.LearningProfiles;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Handwriting.Tests;

internal static class ApiChecks
{
    public static async Task RunAsync(string database)
    {
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
        await using var app = TestApiHost.Build(database);
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        const string root = "/api/v1/learning-profiles/9001";
        const string templatesUrl = root + "/writing-systems/hiragana/handwriting-templates";
        const string submitUrl = root + "/writing-systems/hiragana/handwriting-attempts";
        const string historyUrl = root + "/handwriting-attempts";
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = new TestClock().GetUtcNow();
            db.Users.AddRange(User.Create(9000, "a@handwriting-api.invalid", "A", "Test", null, now),
                User.Create(9100, "b@handwriting-api.invalid", "B", "Test", null, now),
                User.Create(9200, "c@handwriting-api.invalid", "C", "Test", null, now));
            db.LearningProfiles.AddRange(
                LearningProfile.Create(9001, 9000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
                LearningProfile.Create(9002, 9000, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
                LearningProfile.Create(9101, 9100, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, now));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = TestApiHost.Client(app);
        async Task<HttpResponseMessage> Send(string method, string path, object? body = null, int status = 200,
            int queries = 1, string user = "a", string? key = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "handwriting-test-" + user);
            key ??= Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            if (body is string raw) request.Content = new StringContent(raw, Encoding.UTF8, "application/json");
            else if (body is not null) request.Content = JsonContent.Create(body);
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode != status)
                throw new InvalidOperationException($"{method} {path}: expected {status}, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            Check.That(true, $"HTTP {status} {method} {path}.");
            Check.That(counter.Count(key) == queries, $"Expected {queries} commands, got {counter.Count(key)}: {counter.Sql(key)}");
            return response;
        }
        async Task<T> Read<T>(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<T>(json))!;
        static object Submission(params (string Id, IReadOnlyList<IReadOnlyList<InkPoint>> Strokes)[] characters) =>
            new { characters = characters.Select(x => new { templateId = x.Id, strokes = x.Strokes }).ToArray() };

        foreach (var path in new[] { templatesUrl, historyUrl, historyUrl + "/1", templatesUrl + "/1" })
            await Send("GET", path, status: 401, queries: 0, user: "none");
        await Send("POST", submitUrl, new { characters = new object[0] }, 401, 0, "none");
        var listResponse = await Send("GET", templatesUrl + "?pageSize=100", key: "templates-summary");
        var page = await Read<HandwritingTemplatePage>(listResponse);
        Check.That(page.Items.Count == 46 && page.NextCursor is null && page.Policy.Version == "geometry-v1", "Full Hiragana coverage and pinned scoring policy.");
        Check.That(listResponse.Headers.CacheControl?.NoStore == true && listResponse.Headers.Pragma.Any(x => x.Name == "no-cache"), "No private response caching.");
        Check.That(!counter.Sql("templates-summary").Contains(".drawing", StringComparison.OrdinalIgnoreCase), "Template list never loads geometry.");
        Check.That(page.Items.All(x => x.Reference.Source.License == "CC BY-SA 3.0" && x.Reference.Source.Attribution.Contains("Ulrich Apel")), "Attribution/license reach the frontend.");
        var a = await Read<HandwritingTemplateDto>(await Send("GET", templatesUrl + "/" + page.Items[0].Reference.Id));
        var o = await Read<HandwritingTemplateDto>(await Send("GET", templatesUrl + "/" + page.Items.Single(x => x.Reference.Symbol == "お").Reference.Id));
        Check.That(a.StrokeCount == 3 && a.DrawingFormatVersion == 1 && a.Policy.MaximumTotalPoints == 4096, "Detail includes reference strokes and input budget.");
        var first = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl + "?pageSize=1"));
        var second = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl + "?pageSize=1&after=" + first.NextCursor));
        Check.That(first.Items[0].Reference.Symbol == "あ" && second.Items[0].Reference.Symbol == "い", "Template keyset pages preserve catalog order.");
        var allPages = new List<string>();
        string? after = null;
        do
        {
            var batch = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl + "?pageSize=20" +
                (after is null ? "" : "&after=" + after)));
            allPages.AddRange(batch.Items.Select(x => x.Reference.Id));
            after = batch.NextCursor;
            if (allPages.Count > 46) throw new InvalidOperationException("Pagination repeated templates.");
        } while (after is not null);
        Check.That(allPages.SequenceEqual(page.Items.Select(x => x.Reference.Id)), "FE's 20-item pagination reaches all 46 characters exactly once.");
        await Send("GET", templatesUrl + "?after=999:9223372036854775807", queries: 2);
        await Send("GET", templatesUrl, status: 404, queries: 2, user: "b");
        await Send("GET", templatesUrl + "/" + a.Template.Reference.Id, status: 404, user: "b");
        await Send("GET", templatesUrl.Replace("9001", "9002"), status: 404, queries: 2);
        var empty = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl.Replace("hiragana", "katakana") + "?pageSize=100"));
        Check.That(empty.Items.Count == 46, "Full Katakana coverage is available.");
        var valid = Submission((a.Template.Reference.Id, a.Strokes), (o.Template.Reference.Id, o.Strokes));
        var createdResponse = await Send("POST", submitUrl, valid, 201, 3, key: "create-word");
        var created = await Read<HandwritingAttemptDto>(createdResponse);
        Check.That(created.Attempt.Passed && created.Attempt.MatchScore == 100 && created.Attempt.TargetText == "あお", "Server grades an exact two-character reference.");
        Check.That(createdResponse.Headers.Location?.ToString().EndsWith(historyUrl + "/" + created.Attempt.Id) == true, "201 Location points to owned attempt detail.");
        Check.That(created.Characters.Select(x => x.Position).SequenceEqual(new[] { 0, 1 }) &&
            created.Characters.All(x => x.Feedback.Passed && x.Feedback.Strokes.Count == 3), "Detailed per-character and per-stroke results.");
        var roundTrip = await Read<HandwritingAttemptDto>(await Send("GET", historyUrl + "/" + created.Attempt.Id, key: "attempt-detail"));
        Check.That(JsonSerializer.Serialize(created, json) == JsonSerializer.Serialize(roundTrip, json), "Create and detail responses round-trip exactly without a post-write read.");
        Check.That(System.Text.RegularExpressions.Regex.Matches(counter.Sql("attempt-detail"), @"\.drawing(?:,|\s)").Count == 1,
            "History detail loads submitted geometry once and never reloads reference geometry.");
        await Send("GET", historyUrl + "/" + created.Attempt.Id, status: 404, user: "b");
        await Send("GET", historyUrl, status: 404, queries: 2, user: "b");
        await Send("GET", historyUrl.Replace("9001", "9101") + "/" + created.Attempt.Id, status: 404);
        await Send("POST", submitUrl, valid, 404, 1, "b");
        await Send("POST", submitUrl.Replace("9001", "9002"), valid, 404);
        await Send("POST", submitUrl, Submission(("999999", a.Strokes)), 404);
        await Send("POST", submitUrl.Replace("hiragana", "katakana"), valid, 404);
        var repeated = await Read<HandwritingAttemptDto>(await Send("POST", submitUrl,
            Submission((a.Template.Reference.Id, a.Strokes), (a.Template.Reference.Id, a.Strokes)), 201, 3));
        Check.That(repeated.Attempt.TargetText == "ああ" && repeated.Attempt.Passed, "Repeated letters do not violate child uniqueness.");
        var missing = await Read<HandwritingAttemptDto>(await Send("POST", submitUrl,
            Submission((a.Template.Reference.Id, Array.Empty<IReadOnlyList<InkPoint>>())), 201, 3));
        Check.That(!missing.Attempt.Passed && missing.Attempt.MatchScore == 0 && missing.Characters[0].Feedback.Issues.Single() == HandwritingIssue.MissingDrawing, "Blank drawing is saved as failed feedback, not an HTTP error.");
        var reverse = a.Strokes.Select(stroke => (IReadOnlyList<InkPoint>)stroke.Reverse().ToArray()).ToArray();
        var reversed = await Read<HandwritingAttemptDto>(await Send("POST", submitUrl, Submission((a.Template.Reference.Id, reverse)), 201, 3));
        Check.That(!reversed.Attempt.Passed && reversed.Characters[0].Feedback.Issues.Contains(HandwritingIssue.WrongDirection), "Direction is graded on the backend.");
        var permuted = new[] { a.Strokes[1], a.Strokes[0], a.Strokes[2] };
        var order = await Read<HandwritingAttemptDto>(await Send("POST", submitUrl, Submission((a.Template.Reference.Id, permuted)), 201, 3));
        Check.That(!order.Attempt.Passed && order.Characters[0].Feedback.Issues.Contains(HandwritingIssue.WrongStrokeOrder), "Order is graded on the backend.");
        var history = await Read<HandwritingAttemptPage>(await Send("GET", historyUrl + "?pageSize=1", key: "history-summary"));
        Check.That(history.Items.Count == 1 && history.NextCursor is not null, "History is paged with a descending Snowflake cursor.");
        Check.That(!counter.Sql("history-summary").Contains("handwriting_practice_characters", StringComparison.OrdinalIgnoreCase) &&
            !counter.Sql("history-summary").Contains("OFFSET", StringComparison.OrdinalIgnoreCase), "History is a bounded root-only keyset query.");
        var history2 = await Read<HandwritingAttemptPage>(await Send("GET", historyUrl + "?pageSize=1&before=" + history.NextCursor));
        Check.That(long.Parse(history.Items[0].Id) > long.Parse(history2.Items[0].Id), "History cursor advances without duplicates.");
        var passed = await Read<HandwritingAttemptPage>(await Send("GET", historyUrl + "?passed=true&systemCode=hiragana"));
        Check.That(passed.Items.Count == 2 && passed.Items.All(x => x.Passed), "Passed/system filters apply before paging.");
        var failed = await Read<HandwritingAttemptPage>(await Send("GET", historyUrl + "?passed=false"));
        Check.That(failed.Items.Count == 3 && failed.Items.All(x => !x.Passed), "Failed attempts are retained and filterable.");
        await Send("GET", historyUrl + "?before=1", queries: 2);
        await Send("GET", historyUrl + "/1", status: 404);

        foreach (var query in new[] { "pageSize=0", "pageSize=101", "pageSize=abc", "after=bad", "after=0:0", "after=0:9223372036854775808", "after=-1:1" })
            await Send("GET", templatesUrl + "?" + query, status: 400, queries: 0);
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "before=0", "before=-1", "before=bad", "before=9223372036854775808", "passed=maybe", "systemCode=bad_code" })
            await Send("GET", historyUrl + "?" + query, status: 400, queries: 0);
        await Send("GET", templatesUrl.Replace("hiragana", "bad_code"), status: 400, queries: 0);
        var id = a.Template.Reference.Id;
        foreach (var raw in new[]
        {
            "{}", "null", "{\"characters\":null}", "{\"characters\":[]}", "{\"characters\":[null]}",
            "{\"characters\":[{}]}", "{\"characters\":[],\"passed\":true}",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":null}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[null]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[null]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"y":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":null,"y":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":"0","y":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":1.01,"y":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":-0.01,"y":0}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":0,"y":1e999}]]}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[],"score":100}]}""",
            $$"""{"characters":[{"templateId":"{{id}}","strokes":[[{"x":0,"y":0,"pressure":1}]]}]}""",
            "{\"characters\":[{\"templateId\":9007199254740993,\"strokes\":[]}]}",
            "{\"characters\":[{\"templateId\":\"9223372036854775808\",\"strokes\":[]}]}"
        }) await Send("POST", submitUrl, raw, 400, 0);
        await Send("POST", submitUrl, Submission(Enumerable.Repeat((id, a.Strokes), 5).ToArray()), 400, 0);
        await Send("POST", submitUrl, Submission((id, Enumerable.Repeat(a.Strokes[0], 13).ToArray())), 400, 0);
        await Send("POST", submitUrl, Submission((id, new[] { Enumerable.Repeat(new InkPoint(0, 0), 513).ToArray() })), 400, 0);
        var big = Enumerable.Repeat((IReadOnlyList<InkPoint>)Enumerable.Repeat(new InkPoint(0, 0), 512).ToArray(), 3).ToArray();
        await Send("POST", submitUrl, Submission((id, big), (id, big), (id, big)), 400, 0);
        await Send("POST", submitUrl, new string(' ', 262145) + "{}", 413, 0);
        using (var request = new HttpRequestMessage(HttpMethod.Post, submitUrl))
        {
            request.Headers.Authorization = new("Bearer", "handwriting-test-a");
            request.Content = new StringContent("{}", Encoding.UTF8, "text/plain");
            Check.That((int)(await client.SendAsync(request)).StatusCode == 415, "Wrong content type rejected.");
        }
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var old = await db.HandwritingTemplates.SingleAsync(x => x.Id == long.Parse(id));
            var item = await db.WritingSystemItems.SingleAsync(x => x.Id == old.WritingSystemItemId);
            db.HandwritingTemplates.Add(HandwritingTemplate.Create(9300, item, 2, old.Drawing, old.SourceUrl,
                old.SourceRevision, old.Attribution, old.License, old.LicenseUrl, true, new TestClock().GetUtcNow()));
            await db.SaveChangesAsync();
            var latest = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl));
            Check.That(latest.Items.Count == 20 && latest.Items[0].Reference.Id == "9300" && latest.Items[0].Reference.Revision == 2,
                "Catalog selects latest published revision by revision, not Snowflake magnitude.");
            await Send("GET", templatesUrl + "/" + id);
            await Send("POST", submitUrl, valid, 201, 3);
            await db.HandwritingTemplates.Where(x => x.Id == 9300).ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsPublished, false).SetProperty(x => x.Version, 2L), default);
            var fallback = await Read<HandwritingTemplatePage>(await Send("GET", templatesUrl));
            Check.That(fallback.Items[0].Reference.Id == id, "Unpublishing latest revision falls back to an older published revision.");
            await Send("GET", templatesUrl + "/9300", status: 404);
            await Send("POST", submitUrl, Submission(("9300", a.Strokes)), 404);
        }
        foreach (var (code, references) in new[] { ("hiragana", page.Items), ("katakana", empty.Items) })
        {
            foreach (var reference in references)
            {
                var detail = await Read<HandwritingTemplateDto>(await Send("GET", templatesUrl.Replace("hiragana", code) + "/" + reference.Reference.Id));
                var result = await Read<HandwritingAttemptDto>(await Send("POST", submitUrl.Replace("hiragana", code),
                    Submission((reference.Reference.Id, detail.Strokes)), 201, 3));
                Check.That(result.Attempt.Passed && result.Attempt.MatchScore == 100 && result.Attempt.TargetText == reference.Reference.Symbol,
                    $"Full API round-trip for {reference.Reference.Symbol}.");
            }
        }
        var concurrent = await Task.WhenAll(Enumerable.Range(0, 12).Select(async _ =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, submitUrl);
            request.Headers.Authorization = new("Bearer", "handwriting-test-a");
            request.Content = JsonContent.Create(valid);
            using var response = await client.SendAsync(request);
            if ((int)response.StatusCode != 201) throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            return await Read<HandwritingAttemptDto>(response);
        }));
        Check.That(concurrent.Select(x => x.Attempt.Id).Distinct().Count() == 12 && concurrent.All(x => x.Attempt.Passed), "Concurrent grading/inserts are isolated with unique Snowflakes.");
        await PersistenceRaceChecks.RunAsync(app.Services, a, created, json);
        await Send("GET", historyUrl + "/" + created.Attempt.Id);
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Check.That(await db.WritingSystemItemProgress.CountAsync(x => x.LearningProfileId == 9001) == 0 &&
                (await db.LearningProfiles.SingleAsync(x => x.Id == 9001)).Version == 1, "Handwriting does not implicitly change progress or profile preferences.");
        }
        await using var limited = TestApiHost.Build(database, 1);
        await limited.StartAsync();
        using var limitedClient = TestApiHost.Client(limited);
        for (var i = 0; i < 2; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, submitUrl);
            request.Headers.Authorization = new("Bearer", "handwriting-test-a");
            request.Content = JsonContent.Create(valid);
            using var response = await limitedClient.SendAsync(request);
            Check.That((int)response.StatusCode == (i == 0 ? 201 : 429), "Grading action uses its own rate limit.");
            if (i == 1) Check.That(response.Headers.RetryAfter is not null && response.Headers.CacheControl?.NoStore == true, "429 includes retry guidance and no-store.");
        }
        await limited.StopAsync();
        await app.StopAsync();
    }
}
