using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api;
using Application;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WritingSystemPractice.Models;
using Domain.LearningProfiles;
using Domain.Users;
using Domain.WritingSystemPractice;
using Domain.WritingSystems;
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

namespace WritingSystemPractice.Tests;

internal static class ApiChecks
{
    public static async Task RunAsync(string database)
    {
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development", Args = ["--urls", "http://127.0.0.1:0"]
        });
        builder.Configuration.AddJsonFile(Path.GetFullPath("../../../../../src/Api/appsettings.json", AppContext.BaseDirectory))
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database, ["SnowflakeId:GeneratorId"] = "1021",
                ["Authentication:Google:ClientId"] = "test.apps.googleusercontent.com",
                ["Authentication:Google:ClientSecret"] = "test-only",
                ["Authentication:Microsoft:ClientId"] = "11111111-1111-1111-1111-111111111111",
                ["Authentication:Microsoft:ClientSecret"] = "test-only",
                ["Authentication:Facebook:AppId"] = "123456789", ["Authentication:Facebook:AppSecret"] = "test-only",
                ["Authentication:Jwt:SigningKey"] = "local-practice-test-only-signing-key-at-least-32-bytes",
                ["AI:KeyProtection:Keys:primary"] = Convert.ToBase64String(new byte[32]),
                ["AI:KeyProtection:FingerprintKey"] = Convert.ToBase64String(Enumerable.Repeat((byte)1, 32).ToArray()),
                ["Storage:R2:ServiceUrl"] = "https://11111111111111111111111111111111.r2.cloudflarestorage.com",
                ["Storage:R2:BucketName"] = "practice-tests", ["Storage:R2:AccessKeyId"] = "test-only",
                ["Storage:R2:SecretAccessKey"] = "test-only", ["RateLimiting:LearningProfiles:PermitLimit"] = "10000"
            });
        builder.Logging.ClearProviders().AddSimpleConsole().SetMinimumLevel(LogLevel.Warning)
            .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None).AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApi(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(Api.Controllers.WritingSystemPracticeController).Assembly);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "PracticeTests";
            options.DefaultChallengeScheme = "PracticeTests";
        }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("PracticeTests", _ => { });
        builder.Services.AddSingleton<TestClock>();
        builder.Services.AddSingleton<TimeProvider>(services => services.GetRequiredService<TestClock>());
        builder.Services.AddSingleton<SqlCommandCounter>();
        builder.Services.AddDbContext<ApplicationDbContext>((services, options) => options.AddInterceptors(services.GetRequiredService<SqlCommandCounter>()));
        await using var app = builder.Build();
        app.UseApi();
        var clock = app.Services.GetRequiredService<TestClock>();
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        const long profileId = 3001;
        const string root = "/api/v1/learning-profiles/3001";
        const string create = root + "/writing-systems/hiragana/practice-sessions";
        const string history = root + "/practice-sessions";
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = clock.GetUtcNow();
            db.Users.AddRange(User.Create(3000, "a@practice-api.invalid", "A", "Test", null, now), User.Create(4000, "b@practice-api.invalid", "B", "Test", null, now));
            db.LearningProfiles.AddRange(
                LearningProfile.Create(profileId, 3000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
                LearningProfile.Create(3002, 3000, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
                LearningProfile.Create(4001, 4000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, now));
            await db.SaveChangesAsync();
            var language = await db.Languages.SingleAsync(x => x.Code == "ja");
            db.WritingSystems.AddRange(
                WritingSystem.Create(5000, language.Id, "practice-two", "Two", true, 0, now),
                WritingSystem.Create(5003, language.Id, "practice-empty", "Empty", true, 0, now),
                WritingSystem.Create(5005, language.Id, "practice-unreadable", "Unreadable", true, 0, now));
            db.WritingSystemItems.AddRange(
                WritingSystemItem.Create(5001, 5000, "a", "あ", 0, null, null, now),
                WritingSystemItem.Create(5002, 5000, "i", "い", 1, null, null, now),
                WritingSystemItem.Create(5006, 5005, "u", "う", 0, null, null, now));
            db.WritingSystemItemReadings.AddRange(
                WritingSystemItemReading.Create(5101, 5001, "main", "a", "hepburn", null, 0, now),
                WritingSystemItemReading.Create(5102, 5001, "alternate", "aa", "hepburn", null, 1, now),
                WritingSystemItemReading.Create(5103, 5002, "main", "i", "hepburn", null, 0, now),
                WritingSystemItemReading.Create(5104, 5001, "other", "one", "other", null, 0, now));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = new HttpClient { BaseAddress = new Uri(app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single()) };
        async Task<HttpResponseMessage> Send(string method, string path, object? body = null, int status = 200,
            string user = "a", int? queries = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "practice-test-" + user);
            var key = Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            if (body is string raw) request.Content = new StringContent(raw, Encoding.UTF8, "application/json");
            else if (body is not null) request.Content = JsonContent.Create(body);
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode != status)
                throw new InvalidOperationException($"{method} {path}: expected {status}, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            Check.That(true, $"HTTP status {method} {path}.");
            if (queries.HasValue) Check.That(counter.Count(key) == queries.Value, $"SQL budget for {method} {path}: expected {queries}, got {counter.Count(key)}.");
            return response;
        }
        async Task<WritingSystemPracticeSessionDto> Detail(HttpResponseMessage response) =>
            (await response.Content.ReadFromJsonAsync<WritingSystemPracticeSessionDto>(json))!;
        async Task<WritingSystemPracticeSessionPage> Page(HttpResponseMessage response) =>
            (await response.Content.ReadFromJsonAsync<WritingSystemPracticeSessionPage>(json))!;
        static string SessionPath(WritingSystemPracticeSessionDto detail) => history + "/" + detail.Session.Id;
        static string AnswerPath(WritingSystemPracticeSessionDto detail, string itemId) => SessionPath(detail) + "/items/" + itemId + "/answer";

        await Send("GET", history, user: "none", status: 401, queries: 0);
        await Send("POST", create, new { questionCount = 2 }, user: "none", status: 401, queries: 0);
        var empty = await Page(await Send("GET", history, queries: 2));
        Check.That(empty.Items.Count == 0 && empty.NextCursor is null, "Empty owned history.");
        await Send("GET", "/api/v1/learning-profiles/4001/practice-sessions", status: 404, queries: 2);
        await Send("POST", create, new { questionCount = 2 }, user: "b", status: 404, queries: 1);
        await Send("POST", create.Replace("3001", "3002"), new { questionCount = 2 }, status: 404, queries: 1);
        await Send("POST", create.Replace("hiragana", "missing"), new { questionCount = 2 }, status: 404, queries: 1);
        foreach (var body in new[] { "{\"questionCount\":0}", "{\"questionCount\":21}", "{\"questionCount\":null}", "{\"questionCount\":1,\"userId\":4000}", "{\"questionCount\":1,\"correctOptionIndex\":0}" })
            await Send("POST", create, body, status: 400, queries: 0);
        await Send("POST", create, "{\"padding\":\"" + new string('x', 5000) + "\"}", status: 413, queries: 0);
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "before=-1", "before=9223372036854775808", "before=1:2", "status=Unknown", "status=99", "systemCode=%21" })
            await Send("GET", history + "?" + query, status: 400, queries: 0);

        var response = await Send("POST", create, "{}", status: 201, queries: 3);
        var session = await Detail(response);
        Check.That(response.Headers.Location?.AbsolutePath == SessionPath(session), "Created Location includes /api/v1 and profile/session IDs.");
        Check.That(response.Headers.CacheControl?.NoStore == true, "Private practice response is no-store.");
        Check.That(session.Session.QuestionCount == 10 && session.Session.Version == 1 && session.Session.Status == WritingSystemPracticeSessionStatus.InProgress, "Default ten-question session.");
        Check.That(session.Items.Count == 10 && session.Items.Select(x => x.WritingSystemItemId).Distinct().Count() == 10, "Distinct sampled targets.");
        Check.That(session.Items.Select(x => x.Position).SequenceEqual(Enumerable.Range(0, 10)), "Stable question order.");
        Check.That(session.Items.All(x => x.CorrectOptionIndex is null && x.IsCorrect is null && x.SelectedOptionIndex is null && x.AnsweredAt is null), "No answers disclosed on create.");
        Check.That(session.Items.All(x => x.Options.Count == 4 && x.Options.Distinct(StringComparer.OrdinalIgnoreCase).Count() == 4), "Distinct shuffled four-option questions.");
        Check.That(session.Items.Select(x => x.TranscriptionSystem).Distinct().Count() == 1, "One transcription system per session.");
        Check.That(session.Session.AccuracyPercentage is null && session.Session.UnansweredCount == 10, "Initial score semantics.");
        var reloaded = await Detail(await Send("GET", SessionPath(session), queries: 1));
        Check.That(JsonSerializer.Serialize(session, json) == JsonSerializer.Serialize(reloaded, json), "Reload retains exact question/option order.");
        await Send("GET", SessionPath(session), user: "b", status: 404, queries: 1);
        await Send("GET", SessionPath(session).Replace("3001", "3002"), status: 404, queries: 1);
        var originalItemId = session.Items[0].Id;
        foreach (var body in new[] { "{}", "{\"version\":1}", "{\"selectedOptionIndex\":0}", "{\"selectedOptionIndex\":-1,\"version\":1}", "{\"selectedOptionIndex\":4,\"version\":1}", "{\"selectedOptionIndex\":0,\"version\":0}", "{\"selectedOptionIndex\":0,\"version\":1,\"isCorrect\":true}" })
            await Send("POST", AnswerPath(session, originalItemId), body, status: 400, queries: 0);
        await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 0, version = 1 }, user: "b", status: 404, queries: 1);
        await Send("POST", AnswerPath(session, "999999"), new { selectedOptionIndex = 0, version = 1 }, status: 404, queries: 1);
        await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 0, version = 2 }, status: 409, queries: 1);
        session = await Detail(await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 0, version = 1 }, queries: 3));
        var answered = session.Items[0];
        Check.That(session.Session.Version == 2 && session.Session.AnsweredCount == 1 && answered.CorrectOptionIndex.HasValue && answered.IsCorrect.HasValue, "Only submitted question gets feedback.");
        Check.That(session.Items.Skip(1).All(x => x.CorrectOptionIndex is null && x.IsCorrect is null), "Remaining answers stay hidden after another question is answered.");
        var answerTime = answered.AnsweredAt;
        clock.Advance();
        var same = await Detail(await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 0, version = 2 }, queries: 2));
        Check.That(same.Session.Version == 2 && same.Items[0].AnsweredAt == answerTime, "Same-option/current-version retry is a no-op.");
        await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 0, version = 1 }, status: 409, queries: 1);
        await Send("POST", AnswerPath(session, originalItemId), new { selectedOptionIndex = 1, version = 2 }, status: 409, queries: 1);
        while (session.Items.Any(x => x.SelectedOptionIndex is null))
        {
            var item = session.Items.First(x => x.SelectedOptionIndex is null);
            session = await Detail(await Send("POST", AnswerPath(session, item.Id), new { selectedOptionIndex = 0, version = session.Session.Version }, queries: 3));
        }
        Check.That(session.Session.Status == WritingSystemPracticeSessionStatus.Completed && session.Session.EndedAt.HasValue && session.Session.Version == 11, "Last answer completes exactly once.");
        Check.That(session.Session.CorrectCount == session.Items.Count(x => x.IsCorrect == true) && session.Session.IncorrectCount + session.Session.CorrectCount == 10, "Score matches persisted feedback.");
        await Send("POST", SessionPath(session) + "/abandon", new { version = session.Session.Version }, status: 409, queries: 1);
        await Send("POST", SessionPath(session) + "/abandon", "{}", status: 400, queries: 0);

        var abandoned = await Detail(await Send("POST", create, new { questionCount = 2 }, status: 201, queries: 3));
        var other = await Detail(await Send("POST", create.Replace("3001", "4001"), new { questionCount = 2 }, user: "b", status: 201, queries: 3));
        await Send("POST", AnswerPath(abandoned, other.Items[0].Id), new { selectedOptionIndex = 0, version = 1 }, status: 404, queries: 1);
        abandoned = await Detail(await Send("POST", AnswerPath(abandoned, abandoned.Items[0].Id), new { selectedOptionIndex = 0, version = 1 }, queries: 3));
        abandoned = await Detail(await Send("POST", SessionPath(abandoned) + "/abandon", new { version = 2 }, queries: 2));
        Check.That(abandoned.Session.Status == WritingSystemPracticeSessionStatus.Abandoned && abandoned.Session.UnansweredCount == 1 && abandoned.Items[1].CorrectOptionIndex is null, "Abandon preserves submitted answers and hides unsubmitted answers.");
        await Send("POST", AnswerPath(abandoned, abandoned.Items[1].Id), new { selectedOptionIndex = 0, version = 3 }, status: 409, queries: 1);
        var repeatAbandon = await Detail(await Send("POST", SessionPath(abandoned) + "/abandon", new { version = 3 }, queries: 2));
        Check.That(repeatAbandon.Session.Version == 3 && repeatAbandon.Session.EndedAt == abandoned.Session.EndedAt, "Repeated abandon does not rewrite state.");
        await Send("POST", SessionPath(abandoned) + "/abandon", new { version = 2 }, status: 409, queries: 1);
        await Send("POST", SessionPath(abandoned) + "/abandon", new { version = 3 }, user: "b", status: 404, queries: 1);

        var concurrent = await Detail(await Send("POST", create, new { questionCount = 2 }, status: 201, queries: 3));
        using (var left = new HttpRequestMessage(HttpMethod.Post, AnswerPath(concurrent, concurrent.Items[0].Id)))
        using (var right = new HttpRequestMessage(HttpMethod.Post, AnswerPath(concurrent, concurrent.Items[1].Id)))
        {
            left.Headers.Authorization = right.Headers.Authorization = new("Bearer", "practice-test-a");
            left.Content = JsonContent.Create(new { selectedOptionIndex = 0, version = 1 });
            right.Content = JsonContent.Create(new { selectedOptionIndex = 0, version = 1 });
            var results = await Task.WhenAll(client.SendAsync(left), client.SendAsync(right));
            Check.That(results.Count(x => (int)x.StatusCode == 200) == 1 && results.Count(x => (int)x.StatusCode == 409) == 1, "Concurrent different-question answers have one winner.");
        }
        concurrent = await Detail(await Send("GET", SessionPath(concurrent), queries: 1));
        Check.That(concurrent.Session.Version == 2 && concurrent.Session.AnsweredCount == 1, "Concurrent loser cannot partially record its answer.");

        var fullPage = await Page(await Send("GET", history, queries: 1));
        Check.That(fullPage.Items.Count == 3 && fullPage.Items.All(x => x.LearningProfileId == "3001"), "History excludes other users.");
        Check.That(fullPage.Items.Select(x => long.Parse(x.Id)).SequenceEqual(fullPage.Items.Select(x => long.Parse(x.Id)).OrderDescending()), "History uses descending Snowflake IDs.");
        var firstPage = await Page(await Send("GET", history + "?pageSize=1", queries: 1));
        var secondPage = await Page(await Send("GET", history + "?pageSize=1&before=" + firstPage.NextCursor, queries: 1));
        Check.That(firstPage.Items[0].Id != secondPage.Items[0].Id && firstPage.NextCursor == firstPage.Items[0].Id, "Cursor yields no repeated session.");
        var completed = await Page(await Send("GET", history + "?status=Completed&systemCode=HIRAGANA", queries: 1));
        Check.That(completed.Items.Count == 1 && completed.Items[0].Id == session.Session.Id && completed.Items[0].CorrectCount == session.Session.CorrectCount, "Filtered history and aggregate scores.");
        Check.That((await Page(await Send("GET", history + "?systemCode=katakana", queries: 2))).Items.Count == 0, "Empty filter is not a 404.");
        Check.That((await Page(await Send("GET", history + "?before=1", queries: 2))).NextCursor is null, "Cursor end is an empty page.");

        var smallCreate = create.Replace("hiragana", "practice-two");
        await Send("POST", smallCreate, new { questionCount = 3 }, status: 400, queries: 1);
        await Send("POST", create.Replace("hiragana", "practice-empty"), new { questionCount = 1 }, status: 400, queries: 1);
        await Send("POST", create.Replace("hiragana", "practice-unreadable"), new { questionCount = 1 }, status: 400, queries: 1);
        var small = await Detail(await Send("POST", smallCreate, new { questionCount = 2 }, status: 201, queries: 3));
        var smallA = small.Items.Single(x => x.WritingSystemItemId == "5001");
        Check.That(smallA.Options.Count == 2 && smallA.Options.Contains("a") && smallA.Options.Contains("i") && !smallA.Options.Contains("aa"), "Alternate valid reading cannot become a distractor.");
        Check.That(small.Items.All(x => x.TranscriptionSystem == "hepburn"), "Choose the transcription system with greatest eligible coverage.");
        await Send("POST", AnswerPath(small, smallA.Id), new { selectedOptionIndex = 2, version = 1 }, status: 400, queries: 1);
        Check.That(small.Items.Single(x => x.WritingSystemItemId == "5002").Options.Count == 3, "Option count adapts to available valid distractors.");

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Check.That(await db.LearningProfiles.Where(x => x.Id == profileId).Select(x => x.Version).SingleAsync() == 1 &&
                !await db.WritingSystemItemProgress.AnyAsync(), "HTTP practice never changes learning progress/profile version.");
            var system = await db.WritingSystems.SingleAsync(x => x.Code == "hiragana");
            system.Update(system.NativeName, false, system.SortOrder, clock.GetUtcNow());
            await db.SaveChangesAsync();
        }
        await Send("POST", create, new { questionCount = 2 }, status: 404, queries: 1);
        concurrent = await Detail(await Send("GET", SessionPath(concurrent), queries: 1));
        var pending = concurrent.Items.Single(x => x.SelectedOptionIndex is null);
        concurrent = await Detail(await Send("POST", AnswerPath(concurrent, pending.Id), new { selectedOptionIndex = 0, version = 2 }, queries: 3));
        Check.That(concurrent.Session.Status == WritingSystemPracticeSessionStatus.Completed, "Existing snapshot can be completed after catalog is unpublished.");
        await Send("GET", history, queries: 1);
        await RepositoryChecksAsync(app.Services, clock);
        await app.StopAsync();
    }

    private static async Task RepositoryChecksAsync(IServiceProvider services, TestClock clock)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IWritingSystemPracticeRepository>();
        var ids = scope.ServiceProvider.GetRequiredService<IIdGenerator>();
        var now = clock.GetUtcNow();
        var profile = await db.LearningProfiles.SingleAsync(x => x.Id == 3001);
        var language = await db.Languages.SingleAsync(x => x.Code == "ja");
        var system = WritingSystem.Create(ids.NewId(), language.Id, "practice-small", "Small", true, 0, now);
        var a = WritingSystemItem.Create(ids.NewId(), system.Id, "a", "あ", 0, null, null, now);
        var b = WritingSystemItem.Create(ids.NewId(), system.Id, "b", "い", 1, null, null, now);
        var ar = WritingSystemItemReading.Create(ids.NewId(), a.Id, "main", "a", "hepburn", null, 0, now);
        var br = WritingSystemItemReading.Create(ids.NewId(), b.Id, "main", "i", "hepburn", null, 0, now);
        db.WritingSystems.Add(system); db.WritingSystemItems.AddRange(a, b); db.WritingSystemItemReadings.AddRange(ar, br);
        await db.SaveChangesAsync();
        WritingSystemPracticeSession NewSession() => WritingSystemPracticeSession.Create(ids.NewId(), profile, system, language,
            [WritingSystemPracticeQuestion.Create(ids.NewId(), a, ar, [ar], ["a", "i"]),
             WritingSystemPracticeQuestion.Create(ids.NewId(), b, br, [br], ["a", "i"])], now);
        var forbidden = NewSession();
        await Check.ThrowsAsync<NotFoundException>(() => repository.CreateAsync(4000, forbidden, default), "Create rechecks owner at the write.");
        Check.That(!await db.WritingSystemPracticeSessions.AnyAsync(x => x.Id == forbidden.Id), "Forbidden create leaves no row.");
        system.Update(system.NativeName, false, 0, now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<NotFoundException>(() => repository.CreateAsync(3000, forbidden, default), "Publication race is rechecked at insertion.");
        system.Update(system.NativeName, true, 0, now);
        await db.SaveChangesAsync();
        var session = NewSession();
        await repository.CreateAsync(3000, session, default);
        db.ChangeTracker.Clear();
        var snapshot = (await repository.FindOwnedAsync(3000, 3001, session.Id, default))!;
        snapshot.Answer(snapshot.Items.First().Id, 0, now);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(4000, snapshot, snapshot.Items.First(), 1, default), "Answer rechecks owner at CAS.");
        var winner = (await repository.FindOwnedAsync(3000, 3001, session.Id, default))!;
        var staleNoOp = (await repository.FindOwnedAsync(3000, 3001, session.Id, default))!;
        winner.Answer(winner.Items.First().Id, 0, now);
        await repository.SaveAnswerAsync(3000, winner, winner.Items.First(), 1, default);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(3000, staleNoOp, staleNoOp.Items.First(), 1, default), "Stale no-op is rejected at storage.");
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(3000, snapshot, snapshot.Items.First(), 1, default), "Stale root answer CAS rejected.");
        var abandon = (await repository.FindOwnedAsync(3000, 3001, session.Id, default))!;
        abandon.Abandon(now);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAbandonAsync(4000, abandon, 2, default), "Abandon rechecks owner at storage.");

        // Deliberately simulate a rogue child-only writer: a failed child CAS must roll back a successful root CAS.
        var childRace = NewSession();
        await repository.CreateAsync(3000, childRace, default);
        db.ChangeTracker.Clear();
        var childSnapshot = (await repository.FindOwnedAsync(3000, 3001, childRace.Id, default))!;
        var child = childSnapshot.Items.First();
        childSnapshot.Answer(child.Id, 0, now);
        await db.WritingSystemPracticeSessionItems.Where(x => x.Id == child.Id).ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.SelectedOptionIndex, (int?)1).SetProperty(x => x.AnsweredAt, (DateTimeOffset?)now)
            .SetProperty(x => x.UpdatedAt, now).SetProperty(x => x.Version, 2L));
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(3000, childSnapshot, child, 1, default), "Child CAS failure rejects entire write.");
        Check.That(await db.WritingSystemPracticeSessions.Where(x => x.Id == childRace.Id).Select(x => x.Version).SingleAsync() == 1, "Child CAS failure rolls root version back.");

        var fakeItem = WritingSystemItem.Create(ids.NewId(), system.Id, "missing", "う", 2, null, null, now);
        var fakeReading = WritingSystemItemReading.Create(ids.NewId(), fakeItem.Id, "main", "u", "hepburn", null, 0, now);
        var broken = WritingSystemPracticeSession.Create(ids.NewId(), profile, system, language,
            [WritingSystemPracticeQuestion.Create(ids.NewId(), fakeItem, fakeReading, [fakeReading], ["u", "a"])], now);
        await Check.ThrowsAsync<NotFoundException>(() => repository.CreateAsync(3000, broken, default), "Missing catalog item rejects child insertion.");
        Check.That(!await db.WritingSystemPracticeSessions.AnyAsync(x => x.Id == broken.Id), "Failed child insert rolls back session creation.");
    }
}
