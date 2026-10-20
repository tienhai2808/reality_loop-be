using System.Net.Http.Json;
using System.Text.Json;
using Application.AI.Models;
using Application.Common.Exceptions;
using Domain.AI;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI.Providers;

internal sealed class AnthropicProviderClient(AiHttpTransport transport, IOptions<AiProviderOptions> options)
    : AiProviderClientBase(transport, options)
{
    public override AiProvider Provider => AiProvider.Anthropic;
    protected override async Task<IReadOnlyList<AiModelDto>> ListAsync(string key, CancellationToken ct)
    {
        var items = new List<AiModelDto>();
        string? after = null;
        for (var page = 0; page < 10; page++)
        {
            using var request = Create(HttpMethod.Get, "https://api.anthropic.com/v1/models?limit=1000" +
                (after is null ? "" : "&after_id=" + Uri.EscapeDataString(after)), key);
            using var json = await Transport.SendAsync(request, ct);
            if (!json.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
                throw new AiProviderException(AiFailureCode.ProviderUnavailable, "Anthropic returned an invalid model catalog.");
            foreach (var item in data.EnumerateArray())
            {
                var id = AiHttpTransport.String(item, "id");
                if (id is not null && AiHttpTransport.String(item, "lifecycle") != "retired")
                    items.Add(new AiModelDto(id, AiHttpTransport.String(item, "display_name") ?? id));
            }
            if (!json.RootElement.TryGetProperty("has_more", out var more) || more.ValueKind != JsonValueKind.True) return items;
            var next = AiHttpTransport.String(json.RootElement, "last_id");
            if (string.IsNullOrEmpty(next) || next == after) break;
            after = next;
        }
        throw new AiProviderException(AiFailureCode.ProviderUnavailable, "The Anthropic model catalog exceeded the pagination limit.");
    }

    public override async Task VerifyAsync(string apiKey, string modelId, CancellationToken ct)
    {
        using var request = Create(HttpMethod.Post, "https://api.anthropic.com/v1/messages", apiKey);
        request.Content = JsonContent.Create(new { model = modelId, max_tokens = 8,
            messages = new[] { new { role = "user", content = "Reply with OK." } } });
        using var json = await Transport.SendAsync(request, ct);
        if (AiHttpTransport.String(json.RootElement, "type") != "message")
            throw new AiProviderException(AiFailureCode.ProviderUnavailable, "Anthropic did not confirm a usable model response.");
    }

    private static HttpRequestMessage Create(HttpMethod method, string uri, string key)
    {
        var request = Request(method, uri, "x-api-key", key);
        request.Headers.Add("anthropic-version", "2023-06-01");
        return request;
    }
}
