using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.AI;

namespace Infrastructure.AI.Providers;

internal sealed class AiHttpTransport(IHttpClientFactory clients, TimeProvider clock)
{
    public async Task<JsonDocument> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        try
        {
            using var client = clients.CreateClient("AiProviders");
            using var response = await client.SendAsync(request, ct);
            JsonDocument? json = null;
            try
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(ct);
                if (bytes.Length > 0) json = JsonDocument.Parse(bytes);
            }
            catch (JsonException) { /* HTTP failures may be HTML from a gateway; do not retain the body. */ }
            if (!response.IsSuccessStatusCode)
            {
                using (json) throw Failure(response, json?.RootElement);
            }
            if (json?.RootElement.ValueKind == JsonValueKind.Object && json.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.Object)
            {
                using (json) throw Failure(response, json.RootElement);
            }
            if (json is null || json.RootElement.ValueKind != JsonValueKind.Object)
            {
                json?.Dispose();
                throw new AiProviderException(AiFailureCode.ProviderUnavailable, "The AI provider returned an invalid response.", clock.GetUtcNow().AddSeconds(30));
            }
            return json;
        }
        catch (HttpRequestException)
        { throw new AiProviderException(AiFailureCode.ProviderUnavailable, "The AI provider is unavailable. Try again later.", clock.GetUtcNow().AddSeconds(30)); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new AiProviderException(AiFailureCode.ProviderUnavailable, "The AI provider timed out. Try again later.", clock.GetUtcNow().AddSeconds(30), isTimeout: true); }
    }

    private AiProviderException Failure(HttpResponseMessage response, JsonElement? body)
    {
        var now = clock.GetUtcNow();
        var raw = body?.ToString() ?? "";
        // Only inspect provider data to classify it; never log, store or return it.
        var error = body is { ValueKind: JsonValueKind.Object } root && root.TryGetProperty("error", out var value) ? value : default;
        var code = String(error, "code") ?? String(error, "type") ?? String(error, "status") ?? "";
        if (response.StatusCode == HttpStatusCode.Unauthorized || raw.Contains("API_KEY_INVALID", StringComparison.Ordinal))
            return new(AiFailureCode.InvalidApiKey, "The AI API key is invalid, expired, or revoked.");
        if (response.StatusCode == HttpStatusCode.PaymentRequired || code is "insufficient_quota" or "billing_error" or
            "organization_spend_limit_exceeded" or "project_spend_limit_exceeded" or "organization_usage_limit_exceeded" or "billing_hard_limit_reached" ||
            raw.Contains("credit balance", StringComparison.OrdinalIgnoreCase) || raw.Contains("spend limit", StringComparison.OrdinalIgnoreCase) ||
            raw.Contains("PerDay", StringComparison.OrdinalIgnoreCase))
            return new(AiFailureCode.QuotaExceeded, "The AI account has no available credit or quota. Check the provider account.");
        if (response.StatusCode == HttpStatusCode.TooManyRequests || code is "rate_limit_error" or "rate_limit_exceeded")
        {
            var delay = response.Headers.RetryAfter?.Delta ?? (response.Headers.RetryAfter?.Date is { } date ? date - now : TimeSpan.FromSeconds(30));
            delay = TimeSpan.FromSeconds(Math.Clamp(delay.TotalSeconds, 1, 3600));
            return new(AiFailureCode.RateLimited, "The AI provider temporarily limited requests.", now.Add(delay));
        }
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new(AiFailureCode.ModelUnavailable, "This AI key does not have the required permissions.", modelListingForbidden: true);
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.UnprocessableEntity)
            return new(AiFailureCode.ModelUnavailable, "The selected model cannot be used with this connection.");
        return new(AiFailureCode.ProviderUnavailable, "The AI provider could not complete the request.", now.AddSeconds(30),
            isTimeout: response.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout);
    }

    internal static string? String(JsonElement value, string name) => value.ValueKind == JsonValueKind.Object &&
        value.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String ? property.GetString() : null;
}
