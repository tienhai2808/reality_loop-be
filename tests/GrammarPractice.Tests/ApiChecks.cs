using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.GrammarPractice.Models;
using Domain.Grammar;
using Domain.GrammarPractice;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrammarPractice.Tests;

internal static class ApiChecks
{
    private const string Root = "/api/v1/learning-profiles/7001/grammar-practice";
    private const string Sessions = Root + "/sessions";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task RunAsync(string database)
    {
        await using var app = TestApiHost.Build(database);
        var clock = app.Services.GetRequiredService<TestClock>();
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = clock.GetUtcNow();
            db.Users.AddRange(User.Create(7000, "a@grammar-practice.invalid", "A", "Test", null, now),
                User.Create(8000, "b@grammar-practice.invalid", "B", "Test", null, now));
            db.LearningProfiles.AddRange(
                LearningProfile.Create(7001, 7000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Study, null, now),
                LearningProfile.Create(7002, 7000, "de", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, now),
                LearningProfile.Create(7003, 7000, "fr", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
                LearningProfile.Create(8001, 8000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Study, null, now));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = TestApiHost.Client(app);
        async Task<HttpResponseMessage> Send(string method, string path, object? body = null, int status = 200,
            string user = "a", int? queries = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "grammar-practice-test-" + user);
            var key = Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            if (body is string raw) request.Content = new StringContent(raw, Encoding.UTF8, "application/json");
            else if (body is not null) request.Content = JsonContent.Create(body);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if ((int)response.StatusCode != status)
                throw new InvalidOperationException($"{method} {path}: expected {status}, got {(int)response.StatusCode}: {content}");
            Check.That(true, $"HTTP status for {method} {path}.");
            if (queries.HasValue) Check.That(counter.Count(key) == queries, $"SQL budget for {method} {path}: expected {queries}, got {counter.Count(key)}.\n{counter.Sql(key)}");
            if (method == "POST" && path == Sessions && status == 201)
                Check.That(counter.Sql(key).Contains("random()", StringComparison.OrdinalIgnoreCase) &&
                    counter.Sql(key).Contains("LIMIT", StringComparison.Ordinal), "Sampling is limited in PostgreSQL.");
            return response;
        }
        static async Task<T> Read<T>(HttpResponseMessage response)
        {
            using (response) return (await response.Content.ReadFromJsonAsync<T>(Json))!;
        }
        async Task<GrammarPracticeSessionDto> Create(string? pointCode = null, int count = 5, string path = Sessions) =>
            await Read<GrammarPracticeSessionDto>(await Send("POST", path, new { grammarPointCode = pointCode, questionCount = count }, 201, queries: 4));
        async Task<GrammarPracticeSessionPage> Page(string suffix = "") =>
            await Read<GrammarPracticeSessionPage>(await Send("GET", Sessions + suffix, queries: 1));

        foreach (var (method, path, body) in new (string, string, object?)[]
        {
            ("GET", Root + "/options", null), ("POST", Sessions, new { questionCount = 5 }),
            ("GET", Sessions, null), ("GET", Sessions + "/1", null),
            ("POST", Sessions + "/1/items/1/answer", new { selectedOptionIndex = 0, version = 1 }),
            ("POST", Sessions + "/1/abandon", new { version = 1 })
        })
            using (await Send(method, path, body, 401, "none", 0)) { }
        using (await Send("GET", Root + "/options", status: 404, user: "b", queries: 1)) { }
        using (await Send("GET", Sessions.Replace("7001", "8001"), status: 404, queries: 2)) { }
        using (await Send("POST", Sessions, new { questionCount = 5 }, 404, "b", 1)) { }
        using (await Send("POST", Sessions.Replace("7001", "7003"), new { questionCount = 5 }, 404, queries: 1)) { }
        var empty = await Read<GrammarPracticeSessionPage>(await Send("GET", Sessions, queries: 2));
        Check.That(empty.Items.Count == 0 && empty.NextCursor is null, "Owned empty history returns an empty page.");
        var options = await Read<GrammarPracticeOptionsDto>(await Send("GET", Root + "/options", queries: 1));
        Check.That(options.LanguageCode == "ja" && options.ExplanationLanguage == "vi" && options.AvailableQuestionCount == 100 &&
            options.GrammarPoints.Count == 10 && options.MinQuestionCount == 5 && options.MaxQuestionCount == 10,
            "Japanese options reflect the actual eligible seed questions and limits.");
        Check.That(options.GrammarPoints.All(point => point.TranslationLocale == "vi" && !point.IsTranslationFallback && point.AvailableQuestionCount == 10),
            "Point names follow the profile explanation language.");
        var germanOptions = await Read<GrammarPracticeOptionsDto>(await Send("GET", (Root + "/options").Replace("7001", "7002"), queries: 1));
        Check.That(germanOptions.LanguageCode == "de" && germanOptions.AvailableQuestionCount == 100, "The same APIs support German.");
        foreach (var body in new[] { "{\"questionCount\":4}", "{\"questionCount\":11}", "{\"questionCount\":null}",
            "{\"questionCount\":5,\"userId\":8000}", "{\"correctOptionIndex\":0}", "{\"grammarPointCode\":\"\"}",
            "{\"grammarPointCode\":\"!\"}", "{" })
            using (await Send("POST", Sessions, body, 400, queries: 0)) { }
        using (await Send("POST", Sessions, "{\"padding\":\"" + new string('x', 5000) + "\"}", 413, queries: 0)) { }
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "before=-1", "before=9223372036854775808", "before=1:2",
            "status=Unknown", "status=99", "grammarPointCode=%21" })
            using (await Send("GET", Sessions + "?" + query, status: 400, queries: 0)) { }
        using (await Send("POST", Sessions, new { grammarPointCode = "missing" }, 404, queries: 1)) { }
        using (await Send("POST", Sessions, new { grammarPointCode = "statement-v2" }, 404, queries: 1)) { }

        var createdResponse = await Send("POST", Sessions, "{}", 201, queries: 4);
        var location = createdResponse.Headers.Location;
        Check.That(createdResponse.Headers.CacheControl?.NoStore == true, "Private responses carry no-store.");
        var mixed = await Read<GrammarPracticeSessionDto>(createdResponse);
        Check.That(location?.AbsolutePath == SessionPath(mixed), "Created Location contains the shared /api/v1 prefix.");
        Check.That(mixed.Session.QuestionCount == 10 && mixed.Session.GrammarPointId is null && mixed.Session.Version == 1 &&
            mixed.Session.Status == GrammarPracticeSessionStatus.InProgress, "Default mixed ten-question practice.");
        AssertHidden(mixed);
        Check.That(mixed.Items.Select(item => item.GrammarExerciseId).Distinct().Count() == 10 &&
            mixed.Items.Select(item => item.Position).SequenceEqual(Enumerable.Range(0, 10)), "Sampling has distinct exercises and stable positions.");
        var scoped = await Create(" NOUN-COPULA ");
        Check.That(scoped.Session.GrammarPointId is not null && scoped.Items.All(item => item.GrammarPointId == scoped.Session.GrammarPointId),
            "Point scope is normalized and enforced for every item.");
        var seven = await Create("noun-copula", 7);
        Check.That(seven.Items.Count == 7, "All counts in the supported five-to-ten range work.");
        var german = await Create("formal-address", 10, Sessions.Replace("7001", "7002"));
        Check.That(german.Items.All(item => item.Options.Contains("Sie") && item.Options.Contains("sie")),
            "German case-sensitive alternatives survive sampling and JSON serialization.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var ids = mixed.Items.Select(item => long.Parse(item.GrammarExerciseId)).ToArray();
            var sources = await db.GrammarExercises.Where(exercise => ids.Contains(exercise.Id)).ToDictionaryAsync(exercise => exercise.Id);
            var sessionId = long.Parse(mixed.Session.Id);
            var snapshots = await db.GrammarPracticeSessionItems.Where(item => item.SessionId == sessionId).ToListAsync();
            Check.That(snapshots.All(item => item.Options[item.CorrectOptionIndex] == sources[item.GrammarExerciseId].Options[sources[item.GrammarExerciseId].CorrectOptionIndex]),
                "The persisted correct answer follows each shuffled option permutation.");
        }
        using (await Send("GET", SessionPath(scoped), status: 404, user: "b", queries: 1)) { }
        using (await Send("GET", SessionPath(scoped).Replace("7001", "8001"), status: 404, queries: 1)) { }
        using (await Send("GET", Sessions + "/9223372036854775807", status: 404, queries: 1)) { }
        var first = scoped.Items[0];
        foreach (var body in new[] { "{\"version\":1}", "{\"selectedOptionIndex\":0}", "{\"selectedOptionIndex\":4,\"version\":1}",
            "{\"selectedOptionIndex\":0,\"version\":0}", "{\"selectedOptionIndex\":0,\"version\":1,\"isCorrect\":true}" })
            using (await Send("POST", AnswerPath(scoped, first.Id), body, 400, queries: 0)) { }
        using (await Send("POST", AnswerPath(scoped, mixed.Items[0].Id), new { selectedOptionIndex = 0, version = 1 }, 404, queries: 1)) { }
        using (await Send("POST", AnswerPath(scoped, first.Id), new { selectedOptionIndex = 0, version = 1 }, 404, "b", 1)) { }
        var correct = await CorrectOption(app.Services, first.Id);
        var wrong = (correct + 1) % first.Options.Count;
        scoped = await Read<GrammarPracticeSessionDto>(await Send("POST", AnswerPath(scoped, first.Id), new { selectedOptionIndex = wrong, version = 1 }, queries: 3));
        Check.That(scoped.Session.Version == 2 && scoped.Session.AnsweredCount == 1 && scoped.Session.IncorrectCount == 1 &&
            scoped.Items[0].CorrectOptionIndex == correct && scoped.Items[0].IsCorrect == false && scoped.Items[0].Explanation is not null,
            "Answering reveals that item's feedback and increments aggregate version.");
        Check.That(scoped.Items.Skip(1).All(item => item.CorrectOptionIndex is null && item.Explanation is null && item.IsCorrect is null),
            "Answering one item keeps all pending answers hidden.");
        var reloaded = await Read<GrammarPracticeSessionDto>(await Send("GET", SessionPath(scoped), queries: 1));
        Check.That(reloaded.Items.SelectMany(item => item.Options).SequenceEqual(scoped.Items.SelectMany(item => item.Options)),
            "Refresh/resume preserves the issued option order.");
        using (await Send("POST", AnswerPath(scoped, first.Id), new { selectedOptionIndex = wrong, version = 1 }, 409, queries: 1)) { }
        scoped = await Read<GrammarPracticeSessionDto>(await Send("POST", AnswerPath(scoped, first.Id), new { selectedOptionIndex = wrong, version = 2 }, queries: 2));
        Check.That(scoped.Session.Version == 2, "Current-version duplicate answer is a no-op.");
        using (await Send("POST", AnswerPath(scoped, first.Id), new { selectedOptionIndex = correct, version = 2 }, 409, queries: 1)) { }
        foreach (var item in scoped.Items.Skip(1))
        {
            var answer = await CorrectOption(app.Services, item.Id);
            scoped = await Read<GrammarPracticeSessionDto>(await Send("POST", AnswerPath(scoped, item.Id),
                new { selectedOptionIndex = answer, version = scoped.Session.Version }, queries: 3));
        }
        Check.That(scoped.Session.Status == GrammarPracticeSessionStatus.Completed && scoped.Session.Version == 6 &&
            scoped.Session.CorrectCount == 4 && scoped.Session.AccuracyPercentage == 80m && scoped.Session.EndedAt.HasValue,
            "The last answer completes and scores the session in the same write.");
        using (await Send("POST", SessionPath(scoped) + "/abandon", new { version = 6 }, 409, queries: 1)) { }
        var abandoned = await Read<GrammarPracticeSessionDto>(await Send("POST", SessionPath(seven) + "/abandon", new { version = 1 }, queries: 2));
        Check.That(abandoned.Session.Status == GrammarPracticeSessionStatus.Abandoned && abandoned.Session.UnansweredCount == 7, "Abandon retains unanswered state.");
        AssertHidden(abandoned);
        abandoned = await Read<GrammarPracticeSessionDto>(await Send("POST", SessionPath(seven) + "/abandon", new { version = 2 }, queries: 2));
        using (await Send("POST", SessionPath(seven) + "/abandon", new { version = 1 }, 409, queries: 1)) { }
        using (await Send("POST", AnswerPath(seven, seven.Items[0].Id), new { selectedOptionIndex = 0, version = 2 }, 409, queries: 1)) { }
        using (await Send("POST", SessionPath(seven) + "/abandon", "{}", 400, queries: 0)) { }
        var completedPage = await Page("?status=Completed&grammarPointCode=noun-copula");
        Check.That(completedPage.Items.Count == 1 && completedPage.Items[0].Id == scoped.Session.Id &&
            completedPage.Items[0].CorrectCount == 4, "History filtering and aggregate counters agree with detail.");
        var page1 = await Page("?pageSize=2");
        var page2 = await Page("?pageSize=2&before=" + page1.NextCursor);
        Check.That(page1.NextCursor is not null && page2.Items.Count > 0 && page1.Items.Select(item => item.Id)
            .Intersect(page2.Items.Select(item => item.Id)).Count() == 0 &&
            page1.Items.Concat(page2.Items).Select(item => long.Parse(item.Id)).SequenceEqual(
                page1.Items.Concat(page2.Items).Select(item => long.Parse(item.Id)).OrderDescending()), "Cursor pages are descending and disjoint.");

        var race = await Create("noun-copula");
        var racing = await Task.WhenAll(race.Items.Take(2).Select(async item =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, AnswerPath(race, item.Id));
            request.Headers.Authorization = new("Bearer", "grammar-practice-test-a");
            request.Content = JsonContent.Create(new { selectedOptionIndex = 0, version = 1 });
            using var response = await client.SendAsync(request);
            return (int)response.StatusCode;
        }));
        Check.That(racing.Order().SequenceEqual(new[] { 200, 409 }), "Parallel answers have exactly one successful writer.");
        race = await Read<GrammarPracticeSessionDto>(await Send("GET", SessionPath(race), queries: 1));
        Check.That(race.Session.Version == 2 && race.Session.AnsweredCount == 1, "Concurrency cannot leave a second answer behind.");

        await UpdateLocale(app.Services, "fr");
        var fallbackOptions = await Read<GrammarPracticeOptionsDto>(await Send("GET", Root + "/options", queries: 1));
        Check.That(fallbackOptions.GrammarPoints.All(point => point.TranslationLocale == "en" && point.IsTranslationFallback), "Options names use English fallback.");
        var fallback = await Create("noun-copula");
        Check.That(fallback.Session.ExplanationLanguage == "fr" && fallback.Items.All(item => item.TranslationLocale == "en" && item.IsTranslationFallback),
            "Requested and effective explanation languages remain distinct in snapshots.");
        await UpdateLocale(app.Services, "vi");
        await SeedEdgeContent(app.Services);
        var noContent = await Read<GrammarPracticeOptionsDto>(await Send("GET", (Root + "/options").Replace("7001", "7003"), queries: 1));
        Check.That(noContent.LanguageCode == "fr" && noContent.AvailableQuestionCount == 0 && noContent.GrammarPoints.Count == 0,
            "An enabled language with no questions returns valid empty options.");
        using (await Send("POST", Sessions.Replace("7001", "7003"), new { questionCount = 5 }, 400, queries: 2)) { }
        var edgeOptions = await Read<GrammarPracticeOptionsDto>(await Send("GET", Root + "/options", queries: 1));
        Check.That(edgeOptions.GrammarPoints.Single(point => point.Code == "practice-small").AvailableQuestionCount == 5 &&
            edgeOptions.GrammarPoints.All(point => point.Code is not "practice-empty" and not "practice-untranslated"),
            "Availability counts only published exercises with a requested/English translation.");
        using (await Send("POST", Sessions, new { grammarPointCode = "practice-small", questionCount = 6 }, 400, queries: 2)) { }
        using (await Send("POST", Sessions, new { grammarPointCode = "practice-empty", questionCount = 5 }, 400, queries: 2)) { }
        using (await Send("POST", Sessions, new { grammarPointCode = "practice-untranslated", questionCount = 5 }, 400, queries: 2)) { }
        var small = await Create("practice-small");
        Check.That(small.Items.All(item => item.Options.Count == 2), "Two-option questions are supported.");
        using (await Send("POST", AnswerPath(small, small.Items[0].Id), new { selectedOptionIndex = 2, version = 1 }, 400, queries: 1)) { }
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var point = await db.GrammarPoints.SingleAsync(point => point.Code == "noun-copula" && point.LanguageId == long.Parse(options.LanguageId));
            point.Update(point.NativeName, point.Pattern, false, point.SortOrder, clock.GetUtcNow());
            await db.SaveChangesAsync();
        }
        using (await Send("POST", Sessions, new { grammarPointCode = "noun-copula" }, 404, queries: 1)) { }
        var preservedPage = await Page("?grammarPointCode=noun-copula");
        Check.That(preservedPage.Items.Any(item => item.Id == scoped.Session.Id), "Unpublishing content preserves history filters.");
        fallback = await Read<GrammarPracticeSessionDto>(await Send("POST", AnswerPath(fallback, fallback.Items[0].Id),
            new { selectedOptionIndex = 0, version = 1 }, queries: 3));
        Check.That(fallback.Items[0].Explanation is not null && fallback.Items[0].TranslationLocale == "en", "Existing snapshot can be answered after unpublishing and changing profile locale.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var language = await db.Languages.SingleAsync(language => language.Code == "de");
            language.Update(language.NativeName, false, language.SortOrder, clock.GetUtcNow());
            await db.SaveChangesAsync();
            Check.That(!await db.WritingSystemItemProgress.AnyAsync() &&
                !await db.VocabularyLearningProgress.AnyAsync() && !db.Database.HasPendingModelChanges(),
                "Grammar practice integrates without unrelated progress writes or a schema change.");
        }
        using (await Send("GET", (Root + "/options").Replace("7001", "7002"), status: 404, queries: 1)) { }
        using (await Send("POST", Sessions.Replace("7001", "7002"), new { questionCount = 5 }, 404, queries: 1)) { }
        var germanHistory = await Read<GrammarPracticeSessionDto>(await Send("GET", Sessions.Replace("7001", "7002") + "/" + german.Session.Id, queries: 1));
        Check.That(germanHistory.Items.Count == 10, "Disabling a language preserves access to issued practice history.");
        await RepositoryChecks.RunAsync(app.Services);
        await app.StopAsync();
        await using var limited = TestApiHost.Build(database, 1);
        await limited.StartAsync();
        using var limitedClient = TestApiHost.Client(limited);
        limitedClient.DefaultRequestHeaders.Authorization = new("Bearer", "grammar-practice-test-a");
        using var accepted = await limitedClient.GetAsync(Root + "/options");
        Check.That(accepted.IsSuccessStatusCode, "First request is accepted by the shared limiter.");
        using var blockedRequest = new HttpRequestMessage(HttpMethod.Get, Sessions);
        blockedRequest.Headers.Add("X-Test-Query", "rate-blocked");
        using var blocked = await limitedClient.SendAsync(blockedRequest);
        Check.That((int)blocked.StatusCode == 429 && blocked.Headers.RetryAfter is not null &&
            blocked.Headers.CacheControl?.NoStore == true && limited.Services.GetRequiredService<SqlCommandCounter>().Count("rate-blocked") == 0,
            "Rate limit rejection includes Retry-After and precedes feature SQL.");
        await limited.StopAsync();
    }

    private static string SessionPath(GrammarPracticeSessionDto session) => Sessions + "/" + session.Session.Id;
    private static string AnswerPath(GrammarPracticeSessionDto session, string itemId) => SessionPath(session) + "/items/" + itemId + "/answer";

    private static void AssertHidden(GrammarPracticeSessionDto session) => Check.That(session.Items.All(item =>
        item.SelectedOptionIndex is null && item.CorrectOptionIndex is null && item.IsCorrect is null && item.Explanation is null && item.AnsweredAt is null),
        "Pending answers and explanations are hidden.");

    private static async Task<int> CorrectOption(IServiceProvider services, string id)
    {
        await using var scope = services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().GrammarPracticeSessionItems
            .Where(item => item.Id == long.Parse(id)).Select(item => item.CorrectOptionIndex).SingleAsync();
    }

    private static async Task UpdateLocale(IServiceProvider services, string locale)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var profile = await db.LearningProfiles.SingleAsync(profile => profile.Id == 7001);
        profile.UpdatePreferences(profile.NativeLanguage, locale, profile.ProficiencyLevel, profile.PrimaryGoal, profile.GoalNote,
            services.GetRequiredService<TestClock>().GetUtcNow());
        await db.SaveChangesAsync();
    }

    private static async Task SeedEdgeContent(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var language = await db.Languages.SingleAsync(language => language.Code == "ja");
        var now = services.GetRequiredService<TestClock>().GetUtcNow();
        db.Languages.Add(Language.Create(7600, "fr", "Français", true, 0, now));
        foreach (var (id, code) in new[] { (7100L, "practice-small"), (7101L, "practice-empty"), (7102L, "practice-untranslated") })
            db.GrammarPoints.Add(GrammarPoint.Create(id, language.Id, code, code, "{blank}", true, 99, now));
        for (var index = 0; index < 5; index++)
        {
            db.GrammarExercises.Add(GrammarExercise.Create(7200 + index, 7100, "small-" + index, "{blank}です。", ["本", "を"], 0, true, index, now));
            db.GrammarExerciseTranslations.Add(GrammarExerciseTranslation.Create(7300 + index, 7200 + index, "vi", "Chọn danh từ.", "本 là danh từ.", now));
            db.GrammarExercises.Add(GrammarExercise.Create(7400 + index, 7102, "untranslated-" + index, "{blank}です。", ["本", "を"], 0, true, index, now));
            db.GrammarExerciseTranslations.Add(GrammarExerciseTranslation.Create(7500 + index, 7400 + index, "de", "Wähle ein Nomen.", "本 ist ein Nomen.", now));
        }
        await db.SaveChangesAsync();
    }
}
