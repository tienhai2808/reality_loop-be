using System.Text.Json;
using Application.AI.Models;
using Application.AI.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;

namespace Application.AI.Commands.CreateAiConnection;

public sealed class CreateAiConnectionCommandHandler(ICurrentUser current, IAiConnectionRepository connections,
    AiProviderRegistry providers, IAiKeyProtector keys, IIdGenerator ids, TimeProvider clock)
{
    public async Task<AiConnectionDto> Handle(CreateAiConnectionCommand command, CancellationToken ct)
    {
        if (command.RequestId == Guid.Empty) throw new RequestValidationException("A non-empty Idempotency-Key UUID is required.");
        var provider = providers.Get(command.Provider);
        var key = AiSetupValidation.Key(command.ApiKey);
        var model = AiSetupValidation.Model(command.ModelId);
        var name = AiSetupValidation.Name(command.DisplayName, command.Provider);
        var userId = current.UserId;
        var fingerprint = keys.Fingerprint(JsonSerializer.Serialize(new { userId, command.Provider, key, model, name }));
        var lease = Guid.NewGuid();
        var reservation = await connections.ReserveCreationAsync(userId, command.RequestId, fingerprint, lease, clock.GetUtcNow(), ct);
        if (reservation.Completed is not null) return reservation.Completed;
        if (!reservation.Acquired) throw new AiCreationInProgressException();
        try
        {
            await provider.VerifyAsync(key, model, ct);
            var id = ids.NewId();
            // Default order zero plus Snowflake ID gives stable insertion order without a MAX/COUNT query.
            var connection = AiConnection.Create(id, userId, command.Provider, name,
                keys.Protect(key, userId, id), key[^4..], model, 0, clock.GetUtcNow());
            await connections.CompleteCreationAsync(connection, command.RequestId, lease, clock.GetUtcNow(), ct);
            return AiConnectionDto.From(connection);
        }
        catch
        {
            // Bound cleanup independently of a disconnected caller. A failed cleanup is recoverable by lease expiry.
            using var cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try { await connections.ReleaseCreationAsync(userId, command.RequestId, lease, cleanup.Token); }
            catch (Exception) { /* Retain the original failure; the durable lease expires. */ }
            throw;
        }
    }
}
