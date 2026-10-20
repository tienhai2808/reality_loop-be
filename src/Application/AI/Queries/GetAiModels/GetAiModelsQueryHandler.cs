using Application.AI.Models;
using Application.AI.Services;
using Domain.AI;

namespace Application.AI.Queries.GetAiModels;

public sealed class GetAiModelsQueryHandler(AiProviderRegistry providers)
{
    public Task<AiModelPage> Handle(GetAiModelsQuery query, CancellationToken ct) =>
        providers.Get(query.Provider).GetModelsAsync(AiSetupValidation.Key(query.ApiKey), ct);
}
