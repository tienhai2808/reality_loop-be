using Domain.AI;

namespace Application.AI.Commands.CreateAiConnection;

public sealed record CreateAiConnectionCommand(Guid RequestId, AiProvider Provider, string ApiKey, string ModelId, string? DisplayName)
{
    public override string ToString() => "CreateAiConnectionCommand (API key redacted)";
}
