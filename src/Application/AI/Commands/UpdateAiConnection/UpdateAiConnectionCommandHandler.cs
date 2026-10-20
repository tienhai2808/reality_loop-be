using Application.AI.Models;
using Application.AI.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.AI.Commands.UpdateAiConnection;

public sealed class UpdateAiConnectionCommandHandler(ICurrentUser current, IAiConnectionRepository connections,
    AiProviderRegistry providers, IAiKeyProtector keys, TimeProvider clock)
{
    public async Task<AiConnectionDto> Handle(UpdateAiConnectionCommand command, CancellationToken ct)
    {
        AiSetupValidation.Version(command.Version);
        var connection = await connections.FindOwnedAsync(current.UserId, command.Id, ct)
            ?? throw new NotFoundException("AI connection was not found.");
        if (connection.Version != command.Version) throw new ConflictException("The AI connection changed. Reload it before updating.");
        var name = command.DisplayName is null ? connection.DisplayName : AiSetupValidation.Name(command.DisplayName, connection.Provider);
        var model = command.ModelId is null ? connection.ModelId : AiSetupValidation.Model(command.ModelId);
        var replacement = command.ApiKey is null ? null : AiSetupValidation.Key(command.ApiKey);
        var verify = replacement is not null || model != connection.ModelId;
        if (verify)
            await providers.Get(connection.Provider).VerifyAsync(replacement ?? keys.Unprotect(connection.EncryptedApiKey,
                current.UserId, connection.Id), model, ct);

        var now = clock.GetUtcNow();
        connection.UpdateSettings(name, model, connection.SortOrder, command.IsEnabled ?? connection.IsEnabled, now);
        if (replacement is not null) connection.ReplaceApiKey(keys.Protect(replacement, current.UserId, connection.Id), replacement[^4..], now);
        if (verify) connection.RecordVerification(null, null, now);
        await connections.SaveAsync(connection, ct);
        return AiConnectionDto.From(connection);
    }
}
