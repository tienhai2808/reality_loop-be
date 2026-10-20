using System.Net.Http.Json;
using System.Text.Json;
using Application.AI.Models;
using Application.Common.Exceptions;
using Domain.AI;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI.Providers;

internal sealed class GoogleAiProviderClient(AiHttpTransport transport, IOptions<AiProviderOptions> options)
    : AiProviderClientBase(transport, options)
{
    public override AiProvider Provider => AiProvider.Google;
    protected override async Task<IReadOnlyList<AiModelDto>> ListAsync(string key, CancellationToken ct)
    {
        var items = new List<AiModelDto>();
        string? token = null;
        for (var page = 0; page < 10; page++)
        {
            using var request = Request(HttpMethod.Get, "https://generativelanguage.googleapis.com/v1beta/models?pageSize=1000" +
                (token is null ? "" : "&pageToken=" + Uri.EscapeDataString(token)), "x-goog-api-key", key);
            using var json = await Transport.SendAsync(request, ct);
            if (!json.RootElement.TryGetProperty("models", out var models) || models.ValueKind != JsonValueKind.Array)
                throw new AiProviderException(AiFailureCode.ProviderUnavailable, "Google returned an invalid model catalog.");
            foreach (var item in models.EnumerateArray())
            {
                var name = AiHttpTransport.String(item, "name");
                if (name?.StartsWith("models/", StringComparison.Ordinal) != true ||
                    !item.TryGetProperty("supportedGenerationMethods", out var methods) || methods.ValueKind != JsonValueKind.Array ||
                    !methods.EnumerateArray().Any(x => x.ValueKind == JsonValueKind.String && x.GetString() == "generateContent")) continue;
                items.Add(new AiModelDto(name[7..], AiHttpTransport.String(item, "displayName") ?? name[7..]));
            }
            var next = AiHttpTransport.String(json.RootElement, "nextPageToken");
            if (string.IsNullOrEmpty(next)) return items;
            if (next == token) break;
            token = next;
        }
        throw new AiProviderException(AiFailureCode.ProviderUnavailable, "The Google model catalog exceeded the pagination limit.");
    }

    public override async Task VerifyAsync(string apiKey, string modelId, CancellationToken ct)
    {
        if (modelId.Any(c => !char.IsAsciiLetterOrDigit(c) && c is not ('-' or '_' or '.' or ':')))
            throw new AiProviderException(AiFailureCode.ModelUnavailable, "Invalid Google model ID.");
        using var request = Request(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/" +
            Uri.EscapeDataString(modelId) + ":generateContent", "x-goog-api-key", apiKey);
        request.Content = JsonContent.Create(new { contents = new[] { new { parts = new[] { new { text = "Reply with OK." } } } },
            generationConfig = new { maxOutputTokens = 8 } });
        using var json = await Transport.SendAsync(request, ct);
        if (!json.RootElement.TryGetProperty("candidates", out var candidates) || candidates.ValueKind != JsonValueKind.Array || candidates.GetArrayLength() == 0)
            throw new AiProviderException(AiFailureCode.ModelUnavailable, "Google did not confirm a usable model response.");
    }
}
