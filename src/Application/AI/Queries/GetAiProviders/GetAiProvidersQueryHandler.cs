using Application.AI.Models;
using Application.Common.Interfaces;

namespace Application.AI.Queries.GetAiProviders;

public sealed class GetAiProvidersQueryHandler(IEnumerable<IAiProviderClient> clients)
{
    public IReadOnlyList<AiProviderDto> Handle(GetAiProvidersQuery query) =>
        clients.OrderBy(x => x.Provider).Select(x => new AiProviderDto(x.Provider, x.Provider.ToString())).ToArray();
}
