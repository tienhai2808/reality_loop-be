using Domain.AI;

namespace Application.AI.Queries.GetAiModels;

public sealed record GetAiModelsQuery(AiProvider Provider, string ApiKey)
{
    public override string ToString() => "GetAiModelsQuery (API key redacted)";
}
