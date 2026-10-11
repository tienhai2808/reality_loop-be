using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Api.Controllers;
using Application;
using Application.Authentication.Models;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Domain.Users;

var connection = Environment.GetEnvironmentVariable("AUTH_TEST_DATABASE")
    ?? throw new InvalidOperationException("Set AUTH_TEST_DATABASE to an isolated PostgreSQL database whose name contains auth_tests.");
if (!(new NpgsqlConnectionStringBuilder(connection).Database?.Contains("auth_tests", StringComparison.Ordinal) ?? false))
    throw new InvalidOperationException("Integration checks only run against a database whose name contains auth_tests.");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Logging.ClearProviders();
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:Database"] = connection,
    ["SnowflakeId:GeneratorId"] = "7",
    ["Authentication:Jwt:Issuer"] = "RealityLoop.Tests",
    ["Authentication:Jwt:Audience"] = "RealityLoop.Tests.Api",
    ["Authentication:Jwt:SigningKey"] = "test-signing-key-only-01234567890123456789",
    ["Authentication:Google:ClientId"] = "test-client",
    ["Authentication:Google:ClientSecret"] = "test-secret",
    ["Authentication:Google:RedirectUri"] = "http://localhost/api/v1/auth/google/callback",
    ["Authentication:Microsoft:ClientId"] = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    ["Authentication:Microsoft:ClientSecret"] = "test-microsoft-secret",
    ["Authentication:Microsoft:TenantId"] = "common",
    ["Authentication:Microsoft:RedirectUri"] = "http://localhost/api/v1/auth/microsoft/callback",
    ["Authentication:Facebook:AppId"] = "123456789012345",
    ["Authentication:Facebook:AppSecret"] = "test-facebook-secret",
    ["Authentication:Facebook:RedirectUri"] = "http://localhost/api/v1/auth/facebook/callback",
    ["Authentication:Facebook:GraphApiVersion"] = "v26.0",
    ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
    ["RateLimiting:Authentication:PermitLimit"] = "500"
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Services.AddSingleton<QueryCounter>();
builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
    options.AddInterceptors(provider.GetRequiredService<QueryCounter>()));
builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
var google = new FakeGoogle();
builder.Services.AddSingleton<IHttpClientFactory>(google);
await using var app = builder.Build();
app.UseApi();
await using (var scope = app.Services.CreateAsyncScope())
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
await app.StartAsync();
var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
var cookies = new CookieContainer();
using var client = new HttpClient(new HttpClientHandler { CookieContainer = cookies, AllowAutoRedirect = false })
{ BaseAddress = new Uri(address) };
var queryCounter = app.Services.GetRequiredService<QueryCounter>();
var checks = 0;
var subjectPrefix = "check-" + Guid.NewGuid().ToString("N");

try
{
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized);
    using (var redirect = await client.GetAsync("/api/v1/auth/google"))
    {
        Check(redirect.StatusCode == HttpStatusCode.Redirect, "Browser start redirects to Google");
        Check(redirect.Headers.Location!.Host == "accounts.google.com", "Redirect destination is Google");
    }
    using (var preflight = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/refresh"))
    {
        preflight.Headers.Add("Origin", "http://localhost:5173");
        preflight.Headers.Add("Access-Control-Request-Method", "POST");
        preflight.Headers.Add("Access-Control-Request-Headers", "content-type,authorization");
        using var response = await client.SendAsync(preflight);
        Check(response.Headers.GetValues("Access-Control-Allow-Origin").Single() == "http://localhost:5173", "Configured CORS origin allowed");
        Check(response.Headers.GetValues("Access-Control-Allow-Credentials").Single() == "true", "Correlation cookie allowed by CORS");
    }
    using (var blocked = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/refresh"))
    {
        blocked.Headers.Add("Origin", "https://untrusted.example");
        blocked.Headers.Add("Access-Control-Request-Method", "POST");
        using var response = await client.SendAsync(blocked);
        Check(!response.Headers.Contains("Access-Control-Allow-Origin"), "Unlisted CORS origin rejected");
    }
    using (var response = await client.GetAsync("/api/v1/openapi/v1.json"))
    {
        Check(response.StatusCode == HttpStatusCode.OK, "OpenAPI available in Development");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Check(json.RootElement.GetProperty("paths").EnumerateObject().All(x => x.Name.StartsWith("/api/v1/")), "All controller routes have global prefix");
    }

    var login = await Login("primary");
    Check(login.User.FirstName == "Test" && login.User.LastName == "User", "Google profile returned");
    Check(login.TokenType == "Bearer" && login.User.Id.Length > 10, "JWT and string Snowflake IDs returned");
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: login.AccessToken);
    await Inspect(async db =>
    {
        var hash = await db.RefreshTokens.Where(x => x.SessionId == long.Parse(login.SessionId)).Select(x => x.TokenHash).SingleAsync();
        Check(hash.Length == 64 && hash != login.RefreshToken, "Only SHA-256 hash persisted");
    });
    var refreshed = await Refresh(login.RefreshToken);
    Check(refreshed.RefreshToken != login.RefreshToken && refreshed.AccessToken != login.AccessToken, "Both tokens replaced on refresh");
    await Inspect(async db =>
    {
        var history = await db.RefreshTokens.Where(x => x.SessionId == long.Parse(login.SessionId)).ToListAsync();
        Check(history.Count == 2 && history.Count(x => x.ConsumedAt != null) == 1 && history.Single(x => x.ParentTokenId != null).ParentTokenId == history.Single(x => x.ConsumedAt != null).Id,
            "Rotation history and parent chain persisted");
    });
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = login.RefreshToken });
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = refreshed.RefreshToken });
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: refreshed.AccessToken);
    await Inspect(async db => Check(await db.RefreshTokens.Where(x => x.SessionId == long.Parse(login.SessionId)).AllAsync(x => x.RevokedAt != null), "Replay revokes every token in the session"));

    var secondLogin = await Login("primary", useGetCallback: true);
    Check(secondLogin.User.Id == login.User.Id && secondLogin.SessionId != login.SessionId, "Returning Google user gets a new session");
    await Expect(HttpMethod.Post, "/api/v1/auth/logout", HttpStatusCode.NoContent, bearer: secondLogin.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: secondLogin.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = secondLogin.RefreshToken });

    var revokeLogin = await Login("revoke");
    await Expect(HttpMethod.Post, "/api/v1/auth/revoke", HttpStatusCode.NoContent, new { refreshToken = revokeLogin.RefreshToken });
    await Expect(HttpMethod.Post, "/api/v1/auth/revoke", HttpStatusCode.NoContent, new { refreshToken = revokeLogin.RefreshToken });
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: revokeLogin.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/revoke", HttpStatusCode.NoContent, new { refreshToken = "unknown-token" });
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = "unknown-token" });
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.BadRequest, new { refreshToken = "" });
    await Expect(HttpMethod.Post, "/api/v1/auth/google/callback", HttpStatusCode.Unauthorized, new { code = "unknown", state = new string('x',43) });

    foreach (var invalid in new[] { "issuer", "audience", "nonce", "expired", "signature", "email" })
    {
        var start = await Begin();
        var code = google.CreateCode(start, subjectPrefix + invalid, issuer: invalid == "issuer" ? "https://evil.example" : null,
            audience: invalid == "audience" ? "foreign-client" : null, nonce: invalid == "nonce" ? "wrong-nonce" : null,
            expired: invalid == "expired", forgedSignature: invalid == "signature", verifiedEmail: invalid != "email");
        await Expect(HttpMethod.Post, "/api/v1/auth/google/callback", HttpStatusCode.Unauthorized, new { code = code.Code, state = code.State });
    }
    var cancelled = google.CreateCode(await Begin(), subjectPrefix + "cancelled");
    var exchangesBeforeCancel = google.ExchangeCount;
    await Expect(HttpMethod.Get, $"/api/v1/auth/google/callback?error=access_denied&state={cancelled.State}", HttpStatusCode.Unauthorized);
    Check(google.ExchangeCount == exchangesBeforeCancel, "Consent denial does not exchange a code");

    var concurrent = await Login("concurrent");
    var racing = await Task.WhenAll(Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = concurrent.RefreshToken }),
        Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = concurrent.RefreshToken }));
    Check(racing.Count(x => x.StatusCode == HttpStatusCode.OK) == 1 && racing.Count(x => x.StatusCode == HttpStatusCode.Unauthorized) == 1,
        "Concurrent refresh serialized: one success, one replay rejection");
    var racedTokens = await racing.Single(x => x.IsSuccessStatusCode).Content.ReadFromJsonAsync<AuthenticationResult>();
    foreach (var response in racing) response.Dispose();
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: racedTokens!.AccessToken);

    var competing = await Login("refresh-revoke");
    var operations = await Task.WhenAll(Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = competing.RefreshToken }),
        Send(HttpMethod.Post, "/api/v1/auth/revoke", new { refreshToken = competing.RefreshToken }));
    Check(operations[1].StatusCode == HttpStatusCode.NoContent && operations[0].StatusCode is HttpStatusCode.OK or HttpStatusCode.Unauthorized,
        "Concurrent revoke/refresh complete without lost revocation");
    foreach (var response in operations) response.Dispose();
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: competing.AccessToken);

    var suspended = await Login("suspended");
    await Inspect(async db =>
    {
        var user = await db.Users.SingleAsync(x => x.Id == long.Parse(suspended.User.Id));
        db.Entry(user).Property(x => x.Status).CurrentValue = UserStatus.Suspended;
        await db.SaveChangesAsync();
    });
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: suspended.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Forbidden, new { refreshToken = suspended.RefreshToken });
    await Inspect(async db => Check(await db.Sessions.Where(x => x.Id == long.Parse(suspended.SessionId)).AllAsync(x => x.RevokedAt != null), "Suspended account refresh revokes session"));

    var expired = await Login("expired-session");
    await Inspect(async db =>
    {
        var session = await db.Sessions.SingleAsync(x => x.Id == long.Parse(expired.SessionId));
        db.Entry(session).Property(x => x.ExpiresAt).CurrentValue = DateTimeOffset.UtcNow.AddMinutes(-1);
        await db.SaveChangesAsync();
    });
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = expired.RefreshToken });
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: expired.AccessToken);

    // One primary-DB query also supplies get-me's profile; no duplicate user fetch.
    var devices = await Login("devices");
    var device2 = await Login("devices");
    var device3 = await Login("devices");
    var foreign = await Login("foreign");
    using (var response = await Send(HttpMethod.Get, "/api/v1/auth/me", bearer: devices.AccessToken))
    {
        Check(response.StatusCode == HttpStatusCode.OK, "Get-me succeeds");
        Check(Commands(response) == 1, "Get-me executes exactly one SQL command including authentication");
    }
    using (var response = await Send(HttpMethod.Get, "/api/v1/auth/sessions?pageSize=2", bearer: devices.AccessToken))
    {
        Check(response.StatusCode == HttpStatusCode.OK, "Session list succeeds");
        var page = (await response.Content.ReadFromJsonAsync<SessionPage>())!;
        Check(page.Items.Count == 2 && page.NextCursor != null, "Session list uses bounded cursor pagination");
        Check(page.Items.All(x => x.Id != foreign.SessionId), "Session list only returns the authenticated user's sessions");
        Check(Commands(response) == 2, "Session list executes two SQL commands including authentication, without COUNT");
        using var next = await Send(HttpMethod.Get, "/api/v1/auth/sessions?pageSize=2&before=" + page.NextCursor, bearer: devices.AccessToken);
        var nextPage = (await next.Content.ReadFromJsonAsync<SessionPage>())!;
        Check(nextPage.Items.Count == 1 && nextPage.NextCursor == null && nextPage.Items[0].IsCurrent,
            "Cursor page has no duplicates and marks current session");
    }
    await Expect(HttpMethod.Get, "/api/v1/auth/sessions?pageSize=101", HttpStatusCode.BadRequest, bearer: devices.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/sessions?before=invalid", HttpStatusCode.BadRequest, bearer: devices.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.BadRequest, new { sessionIds = Array.Empty<string>() }, devices.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.BadRequest, new { sessionIds = new[] { "-1" } }, devices.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.NotFound,
        new { sessionIds = new[] { device2.SessionId, foreign.SessionId } }, devices.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: device2.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: foreign.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.NotFound,
        new { sessionIds = new[] { device2.SessionId, "9223372036854775807" } }, devices.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: device2.AccessToken);
    using (var response = await Send(HttpMethod.Post, "/api/v1/auth/logout/sessions",
        new { sessionIds = new[] { device2.SessionId, device3.SessionId, device2.SessionId } }, devices.AccessToken))
    {
        Check(response.StatusCode == HttpStatusCode.NoContent, "Selected sessions revoked with duplicate IDs normalized");
        Check(Commands(response) == 4, "Batch logout executes four SQL commands regardless of target count");
        Check(MaterializedTokens(response) == 0, "Batch logout does not load token history");
    }
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: device2.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: device3.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = device2.RefreshToken });
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: devices.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.NoContent,
        new { sessionIds = new[] { device2.SessionId } }, devices.AccessToken);

    var another = await Login("devices");
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/others", HttpStatusCode.NoContent, bearer: devices.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: another.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: devices.AccessToken);
    var freshDevice = await Login("devices");
    using (var response = await Send(HttpMethod.Post, "/api/v1/auth/logout/all", bearer: devices.AccessToken))
    {
        Check(response.StatusCode == HttpStatusCode.NoContent, "All sessions revoked including caller");
        Check(Commands(response) == 4 && MaterializedTokens(response) == 0, "Logout-all performs one bulk mutation without materializing tokens");
    }
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: devices.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: freshDevice.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: foreign.AccessToken);
    await Inspect(async db =>
    {
        var ids = await db.Sessions.Where(x => x.UserId == long.Parse(devices.User.Id)).Select(x => x.Id).ToListAsync();
        Check(await db.RefreshTokens.Where(x => ids.Contains(x.SessionId)).AllAsync(x => x.RevokedAt != null),
            "Logout-all revokes both consumed and current tokens across every owned session");
    });
    var afterAll = await Login("devices");
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: afterAll.AccessToken);
    await Expect(HttpMethod.Post, "/api/v1/auth/logout/sessions", HttpStatusCode.NoContent,
        new { sessionIds = new[] { afterAll.SessionId } }, afterAll.AccessToken);
    await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: afterAll.AccessToken);

    var history = await Login("history");
    for (var i = 0; i < 4; i++) history = await Refresh(history.RefreshToken);
    using (var response = await Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = history.RefreshToken }))
    {
        Check(response.StatusCode == HttpStatusCode.OK, "Refresh with long token history succeeds");
        Check(Commands(response) == 4, "Refresh executes four SQL commands independent of history length");
        Check(MaterializedTokens(response) == 1, "Refresh loads only the requested token");
        history = (await response.Content.ReadFromJsonAsync<AuthenticationResult>())!;
    }
    using (var response = await Send(HttpMethod.Post, "/api/v1/auth/logout", bearer: history.AccessToken))
    {
        Check(response.StatusCode == HttpStatusCode.NoContent && Commands(response) == 4,
            "Current-session logout executes four SQL commands including authentication");
        Check(MaterializedTokens(response) == 0, "Current logout never loads token history");
    }
    await Inspect(async db => Check(await db.RefreshTokens.Where(x => x.SessionId == long.Parse(history.SessionId)).AllAsync(x => x.RevokedAt != null),
        "Single-session logout revokes the full rotation history"));

    // Block one session; another device of the SAME user must still refresh concurrently.
    var parallelA = await Login("parallel-devices");
    var parallelB = await Login("parallel-devices");
    await using (var blocker = new NpgsqlConnection(connection))
    {
        await blocker.OpenAsync();
        await using var held = await blocker.BeginTransactionAsync();
        await using var command = new NpgsqlCommand("SELECT id FROM sessions WHERE id = @id FOR UPDATE", blocker, held);
        command.Parameters.AddWithValue("id", long.Parse(parallelA.SessionId));
        await command.ExecuteScalarAsync();
        var blockedRefresh = Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = parallelA.RefreshToken });
        await WaitForLock("FOR UPDATE OF s");
        var otherRefresh = Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = parallelB.RefreshToken });
        var completed = await Task.WhenAny(otherRefresh, Task.Delay(TimeSpan.FromSeconds(5)));
        Check(completed == otherRefresh, "Independent devices refresh in parallel despite a blocked session");
        using var otherResponse = await otherRefresh;
        Check(otherResponse.StatusCode == HttpStatusCode.OK, "Other device refresh completed under a shared user lock");
        await held.CommitAsync();
        using var blockedResponse = await blockedRefresh;
        Check(blockedResponse.StatusCode == HttpStatusCode.OK, "Blocked refresh completes after session lock release");
    }

    var allRace = await Login("all-refresh-race");
    var allOperations = await Task.WhenAll(Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = allRace.RefreshToken }),
        Send(HttpMethod.Post, "/api/v1/auth/logout/all", bearer: allRace.AccessToken));
    Check(allOperations[1].StatusCode == HttpStatusCode.NoContent && allOperations[0].StatusCode is HttpStatusCode.OK or HttpStatusCode.Unauthorized,
        "Refresh racing logout-all cannot preserve access");
    if (allOperations[0].StatusCode == HttpStatusCode.OK)
    {
        var issued = (await allOperations[0].Content.ReadFromJsonAsync<AuthenticationResult>())!;
        await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: issued.AccessToken);
        await Expect(HttpMethod.Post, "/api/v1/auth/refresh", HttpStatusCode.Unauthorized, new { refreshToken = issued.RefreshToken });
    }
    foreach (var response in allOperations) response.Dispose();

    // Queue logout-all before a new login, behind an exclusive user lock.
    var loginRace = await Login("all-login-race");
    await using (var blocker = new NpgsqlConnection(connection))
    {
        await blocker.OpenAsync();
        await using var held = await blocker.BeginTransactionAsync();
        await using var command = new NpgsqlCommand("SELECT id FROM users WHERE id = @id FOR UPDATE", blocker, held);
        command.Parameters.AddWithValue("id", long.Parse(loginRace.User.Id));
        await command.ExecuteScalarAsync();
        var logoutTask = Send(HttpMethod.Post, "/api/v1/auth/logout/all", bearer: loginRace.AccessToken);
        await WaitForLock("FOR UPDATE OF u");
        var newLoginTask = Login("all-login-race");
        await WaitForLock("FOR SHARE OF u");
        await held.CommitAsync();
        using var logoutResponse = await logoutTask;
        Check(logoutResponse.StatusCode == HttpStatusCode.NoContent, "Queued logout-all completes");
        var newLogin = await newLoginTask;
        await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.OK, bearer: newLogin.AccessToken);
        await Expect(HttpMethod.Get, "/api/v1/auth/me", HttpStatusCode.Unauthorized, bearer: loginRace.AccessToken);
        Check(newLogin.SessionId != loginRace.SessionId, "Login after logout-all creates a fresh valid session");
    }

    // Database constraints prevent multiple unconsumed/unrevoked tokens in one session.
    var constrained = await Login("constraint");
    await Inspect(async db =>
    {
        var existingToken = await db.RefreshTokens.SingleAsync(x => x.SessionId == long.Parse(constrained.SessionId));
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO refresh_tokens (id, session_id, token_hash, created_at, expires_at)
                VALUES ({-long.Parse(constrained.SessionId)}, {existingToken.SessionId}, {Guid.NewGuid().ToString("N")},
                    {existingToken.CreatedAt}, {existingToken.ExpiresAt})
                """);
            Check(false, "Unique partial index must reject a second unconsumed token");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        { Check(ex.ConstraintName == "ux_refresh_tokens_session_unconsumed", "Database enforces one unconsumed token per session"); }
    });

    // A separate API instance sharing the DB must observe revocation immediately.
    var crossInstance = await Login("cross-instance");
    var secondBuilder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
    secondBuilder.WebHost.UseUrls("http://127.0.0.1:0");
    secondBuilder.Logging.ClearProviders();
    secondBuilder.Configuration.AddInMemoryCollection(builder.Configuration.AsEnumerable());
    secondBuilder.Services.AddApplication();
    secondBuilder.Services.AddInfrastructure(secondBuilder.Configuration);
    secondBuilder.Services.AddApi(secondBuilder.Configuration);
    secondBuilder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
    secondBuilder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
    await using (var second = secondBuilder.Build())
    {
        second.UseApi();
        await second.StartAsync();
        var secondAddress = second.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var secondClient = new HttpClient { BaseAddress = new Uri(secondAddress) };
        secondClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", crossInstance.AccessToken);
        using (var response = await secondClient.GetAsync("/api/v1/auth/me"))
            Check(response.StatusCode == HttpStatusCode.OK, "JWT accepted by another API instance");
        await Expect(HttpMethod.Post, "/api/v1/auth/logout/all", HttpStatusCode.NoContent, bearer: crossInstance.AccessToken);
        using (var response = await secondClient.GetAsync("/api/v1/auth/me"))
            Check(response.StatusCode == HttpStatusCode.Unauthorized, "Revocation visible on another API instance without a cache delay");
        await second.StopAsync();
    }

    var loadUser = await Login("load");
    var watch = System.Diagnostics.Stopwatch.StartNew();
    var simultaneousReads = await Task.WhenAll(Enumerable.Range(0, 64).Select(_ =>
        Send(HttpMethod.Get, "/api/v1/auth/me", bearer: loadUser.AccessToken)));
    watch.Stop();
    Check(simultaneousReads.All(x => x.StatusCode == HttpStatusCode.OK && Commands(x) == 1),
        "64 simultaneous get-me requests succeed with exactly one SQL command per request");
    foreach (var response in simultaneousReads) response.Dispose();
    Console.WriteLine($"Concurrency smoke check: 64 get-me requests in {watch.ElapsedMilliseconds} ms (local environment, not a capacity benchmark).");

    var loadDevices = new List<AuthenticationResult> { loadUser };
    for (var i = 0; i < 15; i++) loadDevices.Add(await Login("load"));
    var simultaneousRefreshes = await Task.WhenAll(loadDevices.Select(x =>
        Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = x.RefreshToken })));
    Check(simultaneousRefreshes.All(x => x.StatusCode == HttpStatusCode.OK && Commands(x) == 4),
        "16 devices of the same user refresh in parallel with exactly four SQL commands each");
    foreach (var response in simultaneousRefreshes) response.Dispose();

    var limited = false;
    for (var i = 0; i < 501 && !limited; i++)
    {
        using var response = await client.GetAsync("/api/v1/auth/google/authorize");
        limited = response.StatusCode == HttpStatusCode.TooManyRequests;
        if (limited) Check(response.Headers.RetryAfter is not null, "Rate limiting returns Retry-After");
    }
    Check(limited, "Authentication endpoints enforce a rate limit");
    using (var blockedRead = await Send(HttpMethod.Get, "/api/v1/auth/me", bearer: loadUser.AccessToken))
        Check(blockedRead.StatusCode == HttpStatusCode.TooManyRequests && Commands(blockedRead) == 0,
            "Rate-limited requests are rejected before authentication database queries");
    Console.WriteLine($"PASS: {checks} authentication integration checks (real PostgreSQL, simulated Google, production API pipeline).");
}
finally { await app.StopAsync(); }

void Check(bool value, string message)
{
    if (!value) throw new InvalidOperationException("FAIL: " + message);
    checks++;
}

async Task<HttpResponseMessage> Send(HttpMethod method, string path, object? body = null, string? bearer = null)
{
    using var request = new HttpRequestMessage(method, path);
    request.Headers.Add("X-Test-Request", Guid.NewGuid().ToString("N"));
    if (body is not null) request.Content = JsonContent.Create(body);
    if (bearer is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
    return await client.SendAsync(request);
}

async Task Expect(HttpMethod method, string path, HttpStatusCode status, object? body = null, string? bearer = null)
{
    using var response = await Send(method, path, body, bearer);
    Check(response.StatusCode == status, $"{method} {path.Split('?')[0]} expected {(int)status}, got {(int)response.StatusCode}");
}

async Task<string> Begin()
{
    using var response = await client.GetAsync("/api/v1/auth/google/authorize");
    Check(response.StatusCode == HttpStatusCode.OK, "Authorization URL returned");
    Check(response.Headers.GetValues("Set-Cookie").Any(x => x.Contains("httponly", StringComparison.OrdinalIgnoreCase)), "Correlation cookie is HttpOnly");
    Check(response.Headers.CacheControl?.NoStore == true, "Authentication responses are not cached");
    using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Check(!json.RootElement.TryGetProperty("protectedState", out _), "Private OAuth state not exposed in JSON");
    return json.RootElement.GetProperty("authorizationUrl").GetString()!;
}

async Task<AuthenticationResult> Login(string subject, bool useGetCallback = false)
{
    var code = google.CreateCode(await Begin(), subjectPrefix + subject);
    using var response = useGetCallback
        ? await Send(HttpMethod.Get, $"/api/v1/auth/google/callback?code={code.Code}&state={code.State}")
        : await Send(HttpMethod.Post, "/api/v1/auth/google/callback", new { code = code.Code, state = code.State });
    Check(response.StatusCode == HttpStatusCode.OK, "Google login succeeds: " + (int)response.StatusCode);
    return (await response.Content.ReadFromJsonAsync<AuthenticationResult>())!;
}

async Task<AuthenticationResult> Refresh(string token)
{
    using var response = await Send(HttpMethod.Post, "/api/v1/auth/refresh", new { refreshToken = token });
    Check(response.StatusCode == HttpStatusCode.OK, "Refresh succeeds");
    return (await response.Content.ReadFromJsonAsync<AuthenticationResult>())!;
}

async Task Inspect(Func<ApplicationDbContext, Task> inspect)
{
    await using var scope = app.Services.CreateAsyncScope();
    await inspect(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
}

int Commands(HttpResponseMessage response) => queryCounter.Commands(response.RequestMessage!.Headers.GetValues("X-Test-Request").Single());
int MaterializedTokens(HttpResponseMessage response) => queryCounter.Tokens(response.RequestMessage!.Headers.GetValues("X-Test-Request").Single());

async Task WaitForLock(string sqlFragment)
{
    await using var connectionForWait = new NpgsqlConnection(connection);
    await connectionForWait.OpenAsync();
    await using var command = new NpgsqlCommand("""
        SELECT EXISTS (SELECT 1 FROM pg_stat_activity
            WHERE datname = current_database() AND wait_event_type = 'Lock' AND query LIKE @pattern
                AND pid <> pg_backend_pid())
        """, connectionForWait);
    command.Parameters.AddWithValue("pattern", "%" + sqlFragment + "%");
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    while (!(bool)(await command.ExecuteScalarAsync(timeout.Token))!)
        await Task.Delay(20, timeout.Token);
}
