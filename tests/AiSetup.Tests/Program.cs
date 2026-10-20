using Infrastructure.Persistence.Entities;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application;
using Application.AI.Commands.CreateAiConnection;
using Application.AI.Commands.UpdateAiConnection;
using Application.AI.Commands.VerifyAiConnection;
using Application.AI.Services;
using Application.AI.Models;
using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;
using Domain.Users;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var checks = 0;
var database = Environment.GetEnvironmentVariable("AI_TEST_DATABASE");
if (database is not null && !(new Npgsql.NpgsqlConnectionStringBuilder(database).Database?.Contains("ai_tests", StringComparison.Ordinal) ?? false))
    throw new InvalidOperationException("AI_TEST_DATABASE must refer to an isolated database whose name contains ai_tests.");
var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:Database"] = database ?? "Host=localhost;Port=1;Database=ai_tests_unused;Username=test;Password=not-used",
    ["SnowflakeId:GeneratorId"] = "17",
    ["AI:KeyProtection:ActiveKeyId"] = "test",
    ["AI:KeyProtection:Keys:test"] = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)),
    ["AI:KeyProtection:FingerprintKey"] = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
}).Build();
var http = new FakeHttp();
var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddLogging();
services.AddInfrastructure(configuration);
services.AddApplication();
services.AddSingleton<IHttpClientFactory>(http);
await using var root = services.BuildServiceProvider();
var keys = root.GetRequiredService<IAiKeyProtector>();
const string rawKey = "test-api-key-abcdefghijklmnop";
var cipher = keys.Protect(rawKey, 10, 20);
Check(keys.Unprotect(cipher, 10, 20) == rawKey, "AES roundtrip");
Check(!cipher.Contains(rawKey) && keys.Protect(rawKey, 10, 20) != cipher, "Ciphertext is randomized");
Throws<InvalidOperationException>(() => keys.Unprotect(cipher, 11, 20), "Owner binding");
Throws<InvalidOperationException>(() => keys.Unprotect(cipher, 10, 21), "Connection binding");
Throws<InvalidOperationException>(() => keys.Unprotect(cipher.Replace("v1.test.", "v1.missing."), 10, 20), "Unknown encryption key");
Check(keys.Fingerprint(rawKey) == keys.Fingerprint(rawKey) && keys.Fingerprint(rawKey) != keys.Fingerprint(rawKey + "x"), "Stable keyed fingerprint");
await using var providerScope = root.CreateAsyncScope();
var clients = providerScope.ServiceProvider.GetServices<IAiProviderClient>().ToArray();
Check(clients.Length == 3, "Three production provider adapters registered");
foreach (var client in clients)
{
    http.Responder = request =>
    {
        Check(!request.RequestUri!.ToString().Contains(rawKey), "No credential in provider URL");
        if (request.Method == HttpMethod.Post)
            return Json(client.Provider switch
            {
                AiProvider.OpenAI => new { @object = "response", status = "completed" },
                AiProvider.Anthropic => (object)new { type = "message" },
                _ => new { candidates = new[] { new { content = new { } } } }
            });
        return Json(client.Provider switch
        {
            AiProvider.OpenAI => new { data = new[] { new { id = "gpt-test" }, new { id = "text-embedding-test" } } },
            AiProvider.Anthropic => (object)new { data = new[] { new { id = "claude-test", display_name = "Claude test" } }, has_more = false },
            _ => new { models = new[] { new { name = "models/gemini-test", displayName = "Gemini test", supportedGenerationMethods = new[] { "generateContent" } } } }
        });
    };
    var models = await client.GetModelsAsync(rawKey, default);
    Check(models.Items.Count == 1 && !models.IsFallback, "Provider catalog mapping and filtering");
    await client.VerifyAsync(rawKey, models.Items[0].Id, default);
    Check(true, "Provider probe mapping");
    http.Responder = _ => Json(new { error = new { type = "authentication_error" } }, HttpStatusCode.Unauthorized);
    await ThrowsAsync<AiProviderException>(() => client.VerifyAsync(rawKey, "test-model", default),
        ex => ex.Code == AiFailureCode.InvalidApiKey && !ex.Message.Contains(rawKey), "Invalid key is an AI failure, not app authentication");
    http.Responder = _ => Json(new { error = new { code = "insufficient_quota" } }, HttpStatusCode.TooManyRequests);
    await ThrowsAsync<AiProviderException>(() => client.VerifyAsync(rawKey, "test-model", default),
        ex => ex.Code == AiFailureCode.QuotaExceeded && ex.RetryAfterAt is null, "Billing is not a short rate-limit cooldown");
    http.Responder = _ => Json(new { error = new { type = "rate_limit_error" } }, HttpStatusCode.TooManyRequests);
    await ThrowsAsync<AiProviderException>(() => client.VerifyAsync(rawKey, "test-model", default),
        ex => ex.Code == AiFailureCode.RateLimited && ex.RetryAfterAt > DateTimeOffset.UtcNow, "Rate-limit retry time");
    http.Responder = _ => Json(new[] { "invalid response" });
    await ThrowsAsync<AiProviderException>(() => client.VerifyAsync(rawKey, "test-model", default),
        ex => ex.Code == AiFailureCode.ProviderUnavailable, "Malformed success response rejected");
}

var now = DateTimeOffset.UtcNow;
var entity = AiConnection.Create(20, 10, AiProvider.OpenAI, null, cipher, rawKey[^4..], "gpt-test", 0, now);
entity.RecordVerification(null, null, now);
Check(entity.Version == 2, "Ready-to-Ready verification still advances concurrency token");
entity.UpdateSettings("Renamed", entity.ModelId, 0, false, now);
entity.RecordVerification(null, null, now);
Check(!entity.IsEnabled && !entity.CanUse(now), "Verification preserves user-disabled state");
entity.UpdateSettings(entity.DisplayName, entity.ModelId, 0, true, now);
entity.MarkCoolingDown(AiFailureCode.RateLimited, now.AddSeconds(20), now);
Check(!entity.CanUse(now) && entity.CanUse(now.AddSeconds(21)), "Cooldown expires without a write");
Throws<ArgumentOutOfRangeException>(() => entity.MarkCoolingDown(AiFailureCode.InvalidApiKey, now.AddSeconds(20), now), "Credential failure cannot use cooldown");
await using (var scope = root.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var model = db.Model.FindEntityType(typeof(AiConnection))!;
    Check(model.FindProperty(nameof(AiConnection.Version))!.IsConcurrencyToken, "EF concurrency token");
    Check(model.GetIndexes().Count() == 1, "One composite connection index, no redundant owner index");
    Check(db.Model.FindEntityType(typeof(AiConnectionCreation))!.GetKeys().Single().Properties.Count == 2, "User-scoped idempotency key");
}

var current = new FakeUser(10);
var fake = new FakeProvider();
var registry = new AiProviderRegistry([fake]);
var memory = new MemoryConnections();
var ids = root.GetRequiredService<IIdGenerator>();
var create = new CreateAiConnectionCommandHandler(current, memory, registry, keys, ids, TimeProvider.System);
var command = new CreateAiConnectionCommand(Guid.NewGuid(), AiProvider.OpenAI, rawKey, "gpt-test", null);
var created = await create.Handle(command, default);
var replay = await create.Handle(command, default);
Check(created == replay && fake.Calls == 1, "Completed replay does not probe or create twice");
await ThrowsAsync<ConflictException>(() => create.Handle(command with { ModelId = "other-model" }, default), _ => true, "Idempotency payload mismatch");
var pending = command with { RequestId = Guid.NewGuid() };
var probeCount = fake.Calls;
fake.BeforeComplete = () => Throws<AiCreationInProgressException>(() => create.Handle(pending, default).GetAwaiter().GetResult(),
    "Concurrent retry is fenced before another probe");
await create.Handle(pending, default);
Check(fake.Calls == probeCount + 1, "Pending retry does not duplicate paid probe");
fake.BeforeComplete = null;
var verify = new VerifyAiConnectionCommandHandler(current, memory, registry, keys, TimeProvider.System);
var checkedDto = await verify.Handle(new(long.Parse(created.Id), created.Version), default);
Check(checkedDto.Version > created.Version, "Verify handler performs a versioned write");
fake.Error = new AiProviderException(AiFailureCode.QuotaExceeded, "No credit.");
var quota = await verify.Handle(new(long.Parse(created.Id), checkedDto.Version), default);
Check(quota.Status == AiConnectionStatus.NeedsAttention && quota.RetryAfterAt is null, "Quota verification persisted as attention");
fake.Error = new AiProviderException(AiFailureCode.ProviderUnavailable, "Temporarily unavailable.");
var unavailable = await verify.Handle(new(long.Parse(created.Id), quota.Version), default);
Check(unavailable.Status == AiConnectionStatus.CoolingDown, "Transient provider failure becomes cooldown");
var update = new UpdateAiConnectionCommandHandler(current, memory, registry, keys, TimeProvider.System);
var before = memory.Items[long.Parse(created.Id)];
await ThrowsAsync<AiProviderException>(() => update.Handle(new(before.Id, before.Version, null, "changed-model", "replacement-key-abcdefghijkl", null), default),
    _ => true, "Failed replacement rejected");
Check(memory.Items[before.Id].ModelId == before.ModelId && memory.Items[before.Id].EncryptedApiKey == before.EncryptedApiKey, "Failed replacement keeps original settings");
current.UserId = 11;
await ThrowsAsync<NotFoundException>(() => verify.Handle(new(before.Id, before.Version), default), _ => true, "Cross-user ownership hidden");
current.UserId = 10;
fake.Error = null;
fake.BeforeComplete = () =>
{
    var stored = memory.Items[before.Id];
    stored.UpdateSettings("Concurrent change", stored.ModelId, 0, false, DateTimeOffset.UtcNow);
};
await ThrowsAsync<ConflictException>(() => verify.Handle(new(before.Id, before.Version), default), _ => true, "Stale probe result rejected");
Check(!memory.Items[before.Id].IsEnabled, "Stale verification does not overwrite concurrent setting");

if (database is not null)
{
    // Real PostgreSQL checks are explicitly opt-in and never target a normal application database.
    fake.BeforeComplete = null;
    http.Responder = _ => Json(new { @object = "response", status = "completed" });
    long userId;
    await using (var scope = root.CreateAsyncScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        userId = ids.NewId();
        db.Users.Add(User.Create(userId, $"ai-{userId}@example.test", "AI", "Test", null, DateTimeOffset.UtcNow));
        await db.SaveChangesAsync();
    }
    try
    {
        var dbCommand = command with { RequestId = Guid.NewGuid() };
        async Task<AiConnectionDto> DbCreate()
        {
            await using var scope = root.CreateAsyncScope();
            var handler = new CreateAiConnectionCommandHandler(new FakeUser(userId), scope.ServiceProvider.GetRequiredService<IAiConnectionRepository>(),
                scope.ServiceProvider.GetRequiredService<AiProviderRegistry>(), keys, ids, TimeProvider.System);
            return await handler.Handle(dbCommand, default);
        }
        var dbCreated = await DbCreate();
        Check((await DbCreate()) == dbCreated, "PostgreSQL idempotency replay");
        await using var inspect = root.CreateAsyncScope();
        var db = inspect.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Check(await db.AiConnections.CountAsync(x => x.UserId == userId) == 1, "Exactly one connection committed");
        Check(await db.AiConnectionCreations.AnyAsync(x => x.UserId == userId && x.ResponseJson != null && x.LeaseToken == null), "Receipt committed atomically");
        await using var scopeA = root.CreateAsyncScope();
        await using var scopeB = root.CreateAsyncScope();
        var a = scopeA.ServiceProvider.GetRequiredService<IAiConnectionRepository>();
        var b = scopeB.ServiceProvider.GetRequiredService<IAiConnectionRepository>();
        var first = (await a.FindOwnedAsync(userId, long.Parse(dbCreated.Id), default))!;
        var stale = (await b.FindOwnedAsync(userId, first.Id, default))!;
        first.RecordVerification(null, null, DateTimeOffset.UtcNow);
        await a.SaveAsync(first, default);
        stale.RecordVerification(null, null, DateTimeOffset.UtcNow);
        await ThrowsAsync<ConflictException>(() => b.SaveAsync(stale, default), _ => true, "PostgreSQL stale update fencing");
        await a.DeleteAsync(userId, first.Id, first.Version, default);
        Check((await DbCreate()).Id == dbCreated.Id, "Deleted connection replay returns original receipt without recreating");
        Check(!await db.AiConnections.AnyAsync(x => x.UserId == userId), "Deleted connection stays deleted");
    }
    finally
    {
        await using var scope = root.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Users.Where(x => x.Id == userId).ExecuteDeleteAsync();
    }
}
Console.WriteLine($"PASS {checks} checks. PostgreSQL checks: {(database is null ? "SKIPPED (AI_TEST_DATABASE not set)" : "completed")}.");

void Check(bool condition, string label) { if (!condition) throw new InvalidOperationException(label); checks++; }
void Throws<T>(Action action, string label) where T : Exception
{
    try { action(); } catch (T) { Check(true, label); return; }
    throw new InvalidOperationException(label);
}
async Task ThrowsAsync<T>(Func<Task> action, Func<T, bool> predicate, string label) where T : Exception
{
    try { await action(); } catch (T ex) when (predicate(ex)) { Check(true, label); return; }
    throw new InvalidOperationException(label);
}
static HttpResponseMessage Json(object body, HttpStatusCode status = HttpStatusCode.OK) => new(status) { Content = JsonContent.Create(body) };

sealed class FakeHttp : HttpMessageHandler, IHttpClientFactory
{
    public Func<HttpRequestMessage, HttpResponseMessage> Responder { get; set; } = _ => throw new InvalidOperationException("Unexpected provider call.");
    public HttpClient CreateClient(string name) => new(this, disposeHandler: false);
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => Task.FromResult(Responder(request));
}
sealed class FakeUser(long id) : ICurrentUser
{
    public long UserId { get; set; } = id;
    public long SessionId => 1;
    public UserDto User => new(UserId.ToString(), "test@example.test", "AI", "Test", null, "Active", DateTimeOffset.UtcNow);
}
sealed class FakeProvider : IAiProviderClient
{
    public AiProvider Provider => AiProvider.OpenAI;
    public int Calls { get; private set; }
    public AiProviderException? Error { get; set; }
    public Action? BeforeComplete { get; set; }
    public Task<AiModelPage> GetModelsAsync(string apiKey, CancellationToken ct) => Task.FromResult(new AiModelPage([], false));
    public Task VerifyAsync(string apiKey, string modelId, CancellationToken ct)
    {
        Calls++; BeforeComplete?.Invoke();
        if (Error is not null) throw Error;
        return Task.CompletedTask;
    }
}
sealed class MemoryConnections : IAiConnectionRepository
{
    public Dictionary<long, AiConnection> Items { get; } = [];
    private readonly Dictionary<AiConnection, long> _versions = [];
    private readonly Dictionary<(long, Guid), (string Fingerprint, Guid Lease, AiConnectionDto? Response)> _receipts = [];
    public Task<AiConnection?> FindOwnedAsync(long userId, long id, CancellationToken ct)
    {
        if (!Items.TryGetValue(id, out var entity) || entity.UserId != userId) return Task.FromResult<AiConnection?>(null);
        var copy = (AiConnection)typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(entity, null)!;
        _versions[copy] = entity.Version;
        return Task.FromResult<AiConnection?>(copy);
    }
    public Task<AiConnectionPage> GetPageAsync(long userId, int size, AiConnectionCursor? cursor, CancellationToken ct) => throw new NotSupportedException();
    public Task<AiCreationReservation> ReserveCreationAsync(long userId, Guid requestId, string fp, Guid lease, DateTimeOffset now, CancellationToken ct)
    {
        if (_receipts.TryGetValue((userId, requestId), out var value))
        {
            if (value.Fingerprint != fp) throw new ConflictException("Payload mismatch.");
            return Task.FromResult(new AiCreationReservation(false, value.Response));
        }
        _receipts[(userId, requestId)] = (fp, lease, null);
        return Task.FromResult(new AiCreationReservation(true, null));
    }
    public Task CompleteCreationAsync(AiConnection connection, Guid requestId, Guid lease, DateTimeOffset now, CancellationToken ct)
    {
        Items[connection.Id] = connection;
        var receipt = _receipts[(connection.UserId, requestId)];
        _receipts[(connection.UserId, requestId)] = (receipt.Fingerprint, lease, AiConnectionDto.From(connection));
        return Task.CompletedTask;
    }
    public Task ReleaseCreationAsync(long userId, Guid requestId, Guid lease, CancellationToken ct)
    { _receipts.Remove((userId, requestId)); return Task.CompletedTask; }
    public Task SaveAsync(AiConnection connection, CancellationToken ct)
    {
        if (Items[connection.Id].Version != _versions[connection]) throw new ConflictException("Version mismatch.");
        Items[connection.Id] = connection;
        return Task.CompletedTask;
    }
    public Task DeleteAsync(long userId, long id, long version, CancellationToken ct) => throw new NotSupportedException();
}
