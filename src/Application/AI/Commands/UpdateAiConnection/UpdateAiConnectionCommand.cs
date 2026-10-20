namespace Application.AI.Commands.UpdateAiConnection;

public sealed record UpdateAiConnectionCommand(long Id, long Version, string? DisplayName, string? ModelId, string? ApiKey, bool? IsEnabled)
{
    public override string ToString() => "UpdateAiConnectionCommand (API key redacted)";
}
