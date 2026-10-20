using Application.AI.Models;
using Domain.AI;

namespace Application.Common.Interfaces;

public interface IAiProviderClient
{
    AiProvider Provider { get; }
    Task<AiModelPage> GetModelsAsync(string apiKey, CancellationToken ct);
    Task VerifyAsync(string apiKey, string modelId, CancellationToken ct);
}
