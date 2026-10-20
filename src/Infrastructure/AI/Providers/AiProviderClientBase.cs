using Application.AI.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI.Providers;

internal abstract class AiProviderClientBase(AiHttpTransport transport, IOptions<AiProviderOptions> options) : IAiProviderClient
{
    protected AiHttpTransport Transport { get; } = transport;
    public abstract AiProvider Provider { get; }

    public async Task<AiModelPage> GetModelsAsync(string apiKey, CancellationToken ct)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(TimeSpan.FromSeconds(45));
        try
        {
            var items = await ListAsync(apiKey, deadline.Token);
            return new AiModelPage(items.Where(x => x.Id.Length is > 0 and <= 200 && !x.Id.Any(char.IsControl) && !x.Id.Any(char.IsWhiteSpace))
                .DistinctBy(x => x.Id, StringComparer.Ordinal).OrderBy(x => x.Name, StringComparer.Ordinal).ToArray(), false);
        }
        catch (AiProviderException ex) when (ex.ModelListingForbidden)
        {
            var fallback = options.Value.Fallback(Provider);
            if (fallback.Length == 0) throw new AiProviderException(AiFailureCode.ModelUnavailable,
                "This key cannot list models. Configure this provider's fallback models or grant model-list access.");
            return new AiModelPage(fallback.Select(x => new AiModelDto(x, x)).DistinctBy(x => x.Id).ToArray(), true);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new AiProviderException(AiFailureCode.ProviderUnavailable, "Listing AI models timed out.", isTimeout: true); }
    }

    protected abstract Task<IReadOnlyList<AiModelDto>> ListAsync(string key, CancellationToken ct);
    public abstract Task VerifyAsync(string apiKey, string modelId, CancellationToken ct);
    protected static HttpRequestMessage Request(HttpMethod method, string uri, string header, string key)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Add(header, key);
        request.Headers.Accept.ParseAdd("application/json");
        return request;
    }
}
