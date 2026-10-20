using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;

namespace Application.AI.Services;

public sealed class AiProviderRegistry(IEnumerable<IAiProviderClient> clients)
{
    private readonly IReadOnlyDictionary<AiProvider, IAiProviderClient> _clients = clients.ToDictionary(x => x.Provider);
    public IAiProviderClient Get(AiProvider provider) => _clients.TryGetValue(provider, out var client)
        ? client : throw new RequestValidationException("This AI provider is not supported.");
}
