using Application.AI.Models;
using Application.AI.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;

namespace Application.AI.Commands.VerifyAiConnection;

public sealed class VerifyAiConnectionCommandHandler(ICurrentUser current, IAiConnectionRepository connections,
    AiProviderRegistry providers, IAiKeyProtector keys, TimeProvider clock)
{
    public async Task<AiConnectionDto> Handle(VerifyAiConnectionCommand command, CancellationToken ct)
    {
        AiSetupValidation.Version(command.Version);
        var connection = await connections.FindOwnedAsync(current.UserId, command.Id, ct)
            ?? throw new NotFoundException("AI connection was not found.");
        if (connection.Version != command.Version) throw new ConflictException("The AI connection changed. Reload it before verifying.");
        AiFailureCode? failure = null;
        DateTimeOffset? retry = null;
        try
        {
            await providers.Get(connection.Provider).VerifyAsync(
                keys.Unprotect(connection.EncryptedApiKey, current.UserId, connection.Id), connection.ModelId, ct);
        }
        catch (AiProviderException ex) { failure = ex.Code; retry = ex.RetryAfterAt; }
        var now = clock.GetUtcNow();
        if (retry is null && failure is AiFailureCode.ProviderUnavailable or AiFailureCode.RateLimited)
            retry = now.AddSeconds(30);
        if (retry <= now) retry = now.AddSeconds(1);
        connection.RecordVerification(failure, retry, now);
        await connections.SaveAsync(connection, ct);
        return AiConnectionDto.From(connection);
    }
}
