using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Application;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.LearningProfiles;
using Domain.LearningProgress;
using Domain.Users;
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
using Npgsql;

var database = Environment.GetEnvironmentVariable("LEARNING_PROGRESS_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("Skipped integration checks. Set LEARNING_PROGRESS_TEST_DATABASE to an isolated database named learning_progress_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "learning_progress_tests")
    throw new InvalidOperationException("Only an isolated database named learning_progress_tests is allowed. The database must be empty.");
var checks = 0;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = "Development", Args = ["--urls", "http://127.0.0.1:0"]
});
var apiSettings = Path.GetFullPath("../../../../../src/Api/appsettings.json", AppContext.BaseDirectory);
builder.Configuration.AddJsonFile(apiSettings).AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:Database"] = database,
    ["SnowflakeId:GeneratorId"] = "1022",
    ["Authentication:Google:ClientId"] = "test.apps.googleusercontent.com",
    ["Authentication:Google:ClientSecret"] = "test-only",
    ["Authentication:Microsoft:ClientId"] = "11111111-1111-1111-1111-111111111111",
    ["Authentication:Microsoft:ClientSecret"] = "test-only",
    ["Authentication:Facebook:AppId"] = "123456789",
    ["Authentication:Facebook:AppSecret"] = "test-only",
    ["Authentication:Jwt:SigningKey"] = "local-test-only-signing-key-with-more-than-32-bytes",
    ["AI:KeyProtection:Keys:primary"] = Convert.ToBase64String(new byte[32]),
    ["AI:KeyProtection:FingerprintKey"] = Convert.ToBase64String(Enumerable.Repeat((byte)1, 32).ToArray()),
    ["Storage:R2:ServiceUrl"] = "https://11111111111111111111111111111111.r2.cloudflarestorage.com",
    ["Storage:R2:BucketName"] = "progress-tests",
    ["Storage:R2:AccessKeyId"] = "test-only",
    ["Storage:R2:SecretAccessKey"] = "test-only",
    ["RateLimiting:LearningProfiles:PermitLimit"] = "10000"
});
builder.Logging.ClearProviders().AddSimpleConsole().SetMinimumLevel(LogLevel.Warning)
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None)
    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Services.AddControllers().AddApplicationPart(typeof(Api.Controllers.LearningProgressController).Assembly);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ProgressTests";
    options.DefaultChallengeScheme = "ProgressTests";
}).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("ProgressTests", _ => { });
builder.Services.AddSingleton<TestClock>();
builder.Services.AddSingleton<TimeProvider>(services => services.GetRequiredService<TestClock>());
builder.Services.AddSingleton<SqlCommandCounter>();
builder.Services.AddDbContext<ApplicationDbContext>((services, options) =>
    options.AddInterceptors(services.GetRequiredService<SqlCommandCounter>()));
await using var app = builder.Build();
app.UseApi();
var clock = app.Services.GetRequiredService<TestClock>();
var counter = app.Services.GetRequiredService<SqlCommandCounter>();
const long profileId = 1001;
long a, i, u, katakanaA;
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    if (await db.Users.AnyAsync() || await db.WritingSystemItemProgress.AnyAsync())
        throw new InvalidOperationException("The integration database must not contain users or progress.");
    var now = clock.GetUtcNow();
    db.Users.AddRange(User.Create(1000, "a@progress.invalid", "A", "Test", null, now),
        User.Create(2000, "b@progress.invalid", "B", "Test", null, now));
    db.LearningProfiles.AddRange(
        LearningProfile.Create(profileId, 1000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now),
        LearningProfile.Create(2001, 2000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, now),
        LearningProfile.Create(1002, 1000, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, now));
    await db.SaveChangesAsync();
    var systems = await db.WritingSystems.ToDictionaryAsync(x => x.Code, x => x.Id);
    a = await db.WritingSystemItems.Where(x => x.WritingSystemId == systems["hiragana"] && x.Code == "kana-r0-c0").Select(x => x.Id).SingleAsync();
    i = await db.WritingSystemItems.Where(x => x.WritingSystemId == systems["hiragana"] && x.Code == "kana-r0-c1").Select(x => x.Id).SingleAsync();
    u = await db.WritingSystemItems.Where(x => x.WritingSystemId == systems["hiragana"] && x.Code == "kana-r0-c2").Select(x => x.Id).SingleAsync();
    katakanaA = await db.WritingSystemItems.Where(x => x.WritingSystemId == systems["katakana"] && x.Code == "kana-r0-c0").Select(x => x.Id).SingleAsync();
}
await app.StartAsync();
try
{
    var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
    using var http = new HttpClient { BaseAddress = new Uri(address), Timeout = TimeSpan.FromSeconds(30) };
    const string root = "/api/v1/learning-profiles/1001";
    var list = root + "/writing-systems/hiragana/progress";
    string Item(long id) => root + $"/writing-systems/hiragana/items/{id}/progress";
    var summary = await Send(HttpMethod.Get, root + "/writing-system-progress", expectedQueries: 1);
    Check(summary.GetProperty("writingSystems").GetArrayLength() == 2, "Two published systems in summary");
    Check(summary.GetProperty("writingSystems")[0].GetProperty("notStartedItems").GetInt32() == 46, "Summary counts missing progress as not started");
    var page = await Send(HttpMethod.Get, list, expectedQueries: 1);
    Check(page.GetProperty("items").GetArrayLength() == 46 && page.GetProperty("nextCursor").ValueKind == JsonValueKind.Null, "Full kana progress page");
    Check(page.GetProperty("items")[0].GetProperty("status").GetString() == "NotStarted", "NotStarted is serialized as a string");
    Check((await Send(HttpMethod.Get, Item(a), expectedQueries: 1)).GetProperty("version").GetInt64() == 0, "Missing row has version zero");
    Check((await Send(HttpMethod.Get, "/api/v1/learning-profiles/1002/writing-system-progress", expectedQueries: 1)).GetProperty("writingSystems").GetArrayLength() == 0, "Supported profile without catalog returns an empty summary");
    await ExpectRows(0);
    var seen = new HashSet<string>();
    string? cursor = null;
    do
    {
        var part = await Send(HttpMethod.Get, list + "?pageSize=7" + (cursor is null ? "" : "&after=" + Uri.EscapeDataString(cursor)), expectedQueries: 1);
        foreach (var row in part.GetProperty("items").EnumerateArray()) Check(seen.Add(row.GetProperty("writingSystemItemId").GetString()!), "Cursor has no duplicates");
        cursor = part.GetProperty("nextCursor").GetString();
    } while (cursor is not null);
    Check(seen.Count == 46, "Cursor covers the full chart");
    await Send(HttpMethod.Get, root + "/writing-system-progress", status: HttpStatusCode.Unauthorized, actor: null, expectedQueries: 0);
    await Send(HttpMethod.Get, root + "/writing-system-progress", status: HttpStatusCode.NotFound, actor: "b", expectedQueries: 1);
    await Send(HttpMethod.Get, list, status: HttpStatusCode.NotFound, actor: "b", expectedQueries: 2);
    await Send(HttpMethod.Get, Item(a), status: HttpStatusCode.NotFound, actor: "b", expectedQueries: 1);
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 0 }, HttpStatusCode.NotFound, "b", 1);
    await Send(HttpMethod.Get, Item(katakanaA), status: HttpStatusCode.NotFound, expectedQueries: 1);
    await Send(HttpMethod.Get, $"/api/v1/learning-profiles/1002/writing-systems/hiragana/items/{a}/progress", status: HttpStatusCode.NotFound, expectedQueries: 1);
    await Send(HttpMethod.Get, list + "?pageSize=101", status: HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Get, list + "?after=bad", status: HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Get, list + "?status=99", status: HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Study" }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { version = 0 }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = -1 }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Unknown", version = 0 }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 0, userId = "2000" }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = (long?)null }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = 999, version = 0 }, HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Get, root + "/writing-systems/hira_gana/progress", status: HttpStatusCode.BadRequest, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 0, padding = new string('x', 5000) }, HttpStatusCode.RequestEntityTooLarge, expectedQueries: 0);
    await Send(HttpMethod.Post, Item(a), new { action = "Restart", version = 0 }, HttpStatusCode.BadRequest, expectedQueries: 1);
    var study = await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 0 }, expectedQueries: 2);
    Check(study.GetProperty("status").GetString() == "Learning" && study.GetProperty("version").GetInt64() == 1, "Study creates learning state");
    await ExpectRows(1);
    Check((await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 1 }, expectedQueries: 2)).GetProperty("version").GetInt64() == 1,
        "Study at the same server timestamp is a no-op");
    var otherView = await Send(HttpMethod.Get, $"/api/v1/learning-profiles/2001/writing-systems/hiragana/items/{a}/progress", actor: "b", expectedQueries: 1);
    Check(otherView.GetProperty("status").GetString() == "NotStarted", "Other profiles do not inherit progress");
    await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 0 }, HttpStatusCode.Conflict, expectedQueries: 1);
    clock.Advance();
    var complete = await Send(HttpMethod.Post, Item(a), new { action = "Complete", version = 1 }, expectedQueries: 2);
    Check(complete.GetProperty("status").GetString() == "Completed" && complete.GetProperty("version").GetInt64() == 2, "Complete advances version");
    var noop = await Send(HttpMethod.Post, Item(a), new { action = "Complete", version = 2 }, expectedQueries: 2);
    Check(noop.GetProperty("updatedAt").GetString() == complete.GetProperty("updatedAt").GetString() && noop.GetProperty("version").GetInt64() == 2, "No-op completion preserves timestamp and version");
    clock.Advance();
    var review = await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 2 }, expectedQueries: 2);
    Check(review.GetProperty("status").GetString() == "Completed" && review.GetProperty("completedAt").GetString() == complete.GetProperty("completedAt").GetString(), "Studying completed content preserves completion");
    clock.Advance();
    var restart = await Send(HttpMethod.Post, Item(a), new { action = "Restart", version = 3 }, expectedQueries: 2);
    Check(restart.GetProperty("status").GetString() == "Learning" && restart.GetProperty("completedAt").ValueKind == JsonValueKind.Null, "Restart clears completion");
    Check((await Send(HttpMethod.Get, list + "?status=Learning", expectedQueries: 1)).GetProperty("items").GetArrayLength() == 1, "Filter learning");
    Check((await Send(HttpMethod.Get, list + "?status=Completed", expectedQueries: 2)).GetProperty("items").GetArrayLength() == 0, "Empty status filter is not a missing system");
    Check((await Send(HttpMethod.Get, list + "?status=NotStarted", expectedQueries: 1)).GetProperty("items").GetArrayLength() == 45, "Filter not started includes absent rows");
    var directComplete = await Send(HttpMethod.Post, Item(i), new { action = "Complete", version = 0 }, expectedQueries: 2);
    Check(directComplete.GetProperty("version").GetInt64() == 2, "First action can complete without a separate Study request");
    var totals = (await Send(HttpMethod.Get, root + "/writing-system-progress", expectedQueries: 1)).GetProperty("writingSystems")[0];
    Check(totals.GetProperty("completedItems").GetInt32() == 1 && totals.GetProperty("learningItems").GetInt32() == 1 && totals.GetProperty("notStartedItems").GetInt32() == 44, "Summary counts and user isolation");
    Check(totals.GetProperty("completionPercentage").GetDecimal() == 2.17m, "Completion percentage is derived from catalog count");
    clock.Advance();
    var createRace = await Task.WhenAll(Race(Item(u), 0), Race(Item(u), 0));
    Check(createRace.Count(x => x == HttpStatusCode.OK) == 1 && createRace.Count(x => x == HttpStatusCode.Conflict) == 1, "Concurrent create has one winner");
    clock.Advance();
    var updateRace = await Task.WhenAll(Race(Item(a), 4), Race(Item(a), 4));
    Check(updateRace.Count(x => x == HttpStatusCode.OK) == 1 && updateRace.Count(x => x == HttpStatusCode.Conflict) == 1, "Concurrent update has one winner");
    await ExpectRows(3);

    await using (var left = app.Services.CreateAsyncScope())
    await using (var right = app.Services.CreateAsyncScope())
    {
        var one = left.ServiceProvider.GetRequiredService<IWritingSystemItemProgressRepository>();
        var two = right.ServiceProvider.GetRequiredService<IWritingSystemItemProgressRepository>();
        var c1 = (await one.FindContextAsync(1000, profileId, "hiragana", a, default))!;
        var c2 = (await two.FindContextAsync(1000, profileId, "hiragana", a, default))!;
        var expectedVersion = c1.Progress!.Version;
        clock.Advance();
        c1.Progress.RecordStudy(clock.GetUtcNow());
        c2.Progress!.MarkCompleted(clock.GetUtcNow());
        await one.SaveAsync(1000, "hiragana", c1.Progress, expectedVersion, default);
        await Conflict(() => two.SaveAsync(1000, "hiragana", c2.Progress, expectedVersion, default));
        await Conflict(() => one.SaveAsync(2000, "hiragana", c1.Progress, c1.Progress.Version, default));
        var noOp = (await one.FindContextAsync(1000, profileId, "hiragana", i, default))!;
        var other = (await two.FindContextAsync(1000, profileId, "hiragana", i, default))!;
        var oldVersion = noOp.Progress!.Version;
        noOp.Progress.MarkCompleted(clock.GetUtcNow());
        clock.Advance();
        other.Progress!.RecordStudy(clock.GetUtcNow());
        await two.SaveAsync(1000, "hiragana", other.Progress, oldVersion, default);
        await Conflict(() => one.SaveAsync(1000, "hiragana", noOp.Progress, oldVersion, default));
    }
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IWritingSystemItemProgressRepository>();
        var beforeHide = (await repository.FindContextAsync(1000, profileId, "hiragana", a, default))!;
        var beforeHideVersion = beforeHide.Progress!.Version;
        clock.Advance();
        beforeHide.Progress.RecordStudy(clock.GetUtcNow());
        await db.WritingSystems.Where(x => x.Id == beforeHide.WritingSystem.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, false));
        await Conflict(() => repository.SaveAsync(1000, "hiragana", beforeHide.Progress, beforeHideVersion, default));
        await db.WritingSystems.Where(x => x.Id == beforeHide.WritingSystem.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, true));
        var now = clock.GetUtcNow();
        db.Users.Add(User.Create(3000, "delete@progress.invalid", "Delete", "Test", null, now));
        db.LearningProfiles.Add(LearningProfile.Create(3001, 3000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Other, null, now));
        await db.SaveChangesAsync();
        var beforeDelete = (await repository.FindContextAsync(3000, 3001, "hiragana", a, default))!;
        var pending = WritingSystemItemProgress.Create(scope.ServiceProvider.GetRequiredService<IIdGenerator>().NewId(),
            beforeDelete.Profile, beforeDelete.Item, beforeDelete.WritingSystem, beforeDelete.Language, now);
        await Conflict(() => repository.SaveAsync(1000, "hiragana", pending, 0, default));
        await db.LearningProfiles.Where(x => x.Id == 3001).ExecuteDeleteAsync();
        await Conflict(() => repository.SaveAsync(3000, "hiragana", pending, 0, default));
        Check(await db.LearningProfiles.Where(x => x.Id == profileId).Select(x => x.Version).SingleAsync() == 1, "Progress never changes profile version");
        var hiraganaId = await db.WritingSystems.Where(x => x.Code == "hiragana").Select(x => x.Id).SingleAsync();
        await db.WritingSystems.Where(x => x.Id == hiraganaId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, false));
        await Send(HttpMethod.Get, Item(a), status: HttpStatusCode.NotFound, expectedQueries: 1);
        await Send(HttpMethod.Post, Item(a), new { action = "Study", version = 6 }, HttpStatusCode.NotFound, expectedQueries: 1);
        Check((await Send(HttpMethod.Get, root + "/writing-system-progress", expectedQueries: 1)).GetProperty("writingSystems").GetArrayLength() == 1, "Unpublished systems are excluded from summary");
        await db.WritingSystems.Where(x => x.Id == hiraganaId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, true));
        var japaneseId = await db.Languages.Where(x => x.Code == "ja").Select(x => x.Id).SingleAsync();
        await db.Languages.Where(x => x.Id == japaneseId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, false));
        Check((await Send(HttpMethod.Get, root + "/writing-system-progress", expectedQueries: 1)).GetProperty("writingSystems").GetArrayLength() == 0, "Disabled language preserves owned profile but hides catalog");
        await Send(HttpMethod.Get, list, status: HttpStatusCode.NotFound, expectedQueries: 2);
    }
    Console.WriteLine($"Passed {checks} learning progress checks: HTTP contracts, authorization, cursor/filter/counts, query budgets and concurrent writes.");

    async Task<JsonElement> Send(HttpMethod method, string url, object? body = null, HttpStatusCode status = HttpStatusCode.OK,
        string? actor = "a", int? expectedQueries = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (actor is not null) request.Headers.Authorization = new("Bearer", "progress-test-" + actor);
        var trace = Guid.NewGuid().ToString("N");
        request.Headers.Add("X-Test-Query", trace);
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        Check(response.StatusCode == status, $"{method} {url}: expected {(int)status}, received {(int)response.StatusCode}: {text}");
        Check(response.Headers.CacheControl?.NoStore == true, "Private progress response is not cacheable");
        if (expectedQueries.HasValue) Check(counter.Count(trace) == expectedQueries, $"Query budget {method} {url}: {counter.Count(trace)} vs {expectedQueries}");
        return string.IsNullOrEmpty(text) ? default : JsonDocument.Parse(text).RootElement.Clone();
    }
    async Task<HttpStatusCode> Race(string url, long version)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        { Content = JsonContent.Create(new { action = "Study", version }) };
        request.Headers.Authorization = new("Bearer", "progress-test-a");
        using var response = await http.SendAsync(request);
        return response.StatusCode;
    }
}
finally { await app.StopAsync(); }

void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    checks++;
}
async Task ExpectRows(int count)
{
    await using var scope = app.Services.CreateAsyncScope();
    Check(await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().WritingSystemItemProgress.CountAsync() == count,
        "Reads do not create progress and mutations do not duplicate rows");
}
async Task Conflict(Func<Task> action)
{
    try { await action(); }
    catch (ConflictException) { checks++; return; }
    throw new InvalidOperationException("A stale or unauthorized write was accepted.");
}
