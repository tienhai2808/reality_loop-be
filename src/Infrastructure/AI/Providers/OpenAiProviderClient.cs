using System.Net.Http.Json;
using Application.AI.Models;
using Application.Common.Exceptions;
using Domain.AI;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI.Providers;

internal sealed class OpenAiProviderClient(AiHttpTransport transport, IOptions<AiProviderOptions> options)
    : AiProviderClientBase(transport, options)
{
    public override AiProvider Provider => AiProvider.OpenAI;
    protected override async Task<IReadOnlyList<AiModelDto>> ListAsync(string key, CancellationToken ct)
    {
        using var request = Request(HttpMethod.Get, "https://api.openai.com/v1/models", "Authorization", "Bearer " + key);
        using var json = await Transport.SendAsync(request, ct);
        if (!json.RootElement.TryGetProperty("data", out var data) || data.ValueKind != System.Text.Json.JsonValueKind.Array)
            throw new AiProviderException(AiFailureCode.ProviderUnavailable, "OpenAI returned an invalid model catalog.");
        return data.EnumerateArray().Select(x => AiHttpTransport.String(x, "id")).OfType<string>()
            .Where(IsTextModel).Select(x => new AiModelDto(x, x)).ToArray();
    }

    private static bool IsTextModel(string id) =>
        (id.StartsWith("gpt-", StringComparison.Ordinal) || id.StartsWith("o1", StringComparison.Ordinal) ||
         id.StartsWith("o3", StringComparison.Ordinal) || id.StartsWith("o4", StringComparison.Ordinal) || id.StartsWith("ft:", StringComparison.Ordinal)) &&
        !new[] { "audio", "realtime", "image", "transcribe", "tts", "search", "instruct", "gpt-3.5" }
            .Any(x => id.Contains(x, StringComparison.OrdinalIgnoreCase));

    public override async Task VerifyAsync(string apiKey, string modelId, CancellationToken ct)
    {
        using var request = Request(HttpMethod.Post, "https://api.openai.com/v1/responses", "Authorization", "Bearer " + apiKey);
        request.Content = JsonContent.Create(new { model = modelId, input = "Reply with OK.", max_output_tokens = 32, store = false });
        using var json = await Transport.SendAsync(request, ct);
        if (AiHttpTransport.String(json.RootElement, "object") != "response" ||
            AiHttpTransport.String(json.RootElement, "status") is not ("completed" or "incomplete"))
            throw new AiProviderException(AiFailureCode.ProviderUnavailable, "OpenAI did not confirm a usable model response.");
    }
}
