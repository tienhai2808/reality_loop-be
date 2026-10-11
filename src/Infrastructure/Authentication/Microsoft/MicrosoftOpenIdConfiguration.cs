using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication.Microsoft;

internal sealed record MicrosoftOpenIdMetadata(string Issuer, IReadOnlyList<JsonWebKey> Keys);

internal sealed class MicrosoftOpenIdConfiguration(IHttpClientFactory clients, IMemoryCache cache,
    IOptions<MicrosoftOAuthOptions> options, TimeProvider clock)
{
    private const string CacheKey = "microsoft-oidc-metadata";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private long _nextRefreshAtTicks;

    public async Task<MicrosoftOpenIdMetadata> GetAsync(bool forceRefresh, CancellationToken ct)
    {
        if (TryCached(forceRefresh, out var cached)) return cached!;
        await _gate.WaitAsync(ct);
        try
        {
            if (TryCached(forceRefresh, out cached)) return cached!;
            // Also throttle unsuccessful refresh attempts while a valid cache entry exists.
            Volatile.Write(ref _nextRefreshAtTicks, clock.GetUtcNow().AddMinutes(1).UtcTicks);
            using var client = clients.CreateClient("MicrosoftOAuth");
            using var discoveryResponse = await client.GetAsync(
                options.Value.Authority + "/v2.0/.well-known/openid-configuration", ct);
            discoveryResponse.EnsureSuccessStatusCode();
            var discovery = await discoveryResponse.Content.ReadFromJsonAsync<DiscoveryResponse>(ct);
            if (discovery is null || !IsTrustedIssuer(discovery.Issuer) || !IsTrustedKeysUri(discovery.JwksUri))
                throw new ExternalServiceException("Microsoft returned invalid OpenID configuration.");

            using var keysResponse = await client.GetAsync(discovery.JwksUri, ct);
            keysResponse.EnsureSuccessStatusCode();
            var keys = new JsonWebKeySet(await keysResponse.Content.ReadAsStringAsync(ct)).Keys
                .Where(key => key.Kty == "RSA" && key.Use == "sig" &&
                    (string.IsNullOrEmpty(key.Alg) || key.Alg == SecurityAlgorithms.RsaSha256)).ToArray();
            if (keys.Length == 0) throw new ExternalServiceException("Microsoft returned no usable signing keys.");
            var result = new MicrosoftOpenIdMetadata(discovery!.Issuer!, keys);
            var maxAge = keysResponse.Headers.CacheControl?.MaxAge ?? TimeSpan.FromHours(1);
            if (maxAge > TimeSpan.FromHours(6)) maxAge = TimeSpan.FromHours(6);
            if (maxAge <= TimeSpan.Zero) maxAge = TimeSpan.FromMinutes(1);
            cache.Set(CacheKey, result, maxAge);
            return result;
        }
        finally { _gate.Release(); }
    }

    private bool TryCached(bool forceRefresh, out MicrosoftOpenIdMetadata? metadata)
    {
        // Unknown kids cannot force an outbound JWKS request more than once per minute.
        return cache.TryGetValue(CacheKey, out metadata) && metadata is not null &&
            (!forceRefresh || clock.GetUtcNow().UtcTicks < Volatile.Read(ref _nextRefreshAtTicks));
    }

    private bool IsTrustedIssuer(string? issuer)
    {
        if (issuer == MicrosoftOAuthOptions.IssuerTemplate) return true;
        const string prefix = "https://login.microsoftonline.com/";
        const string suffix = "/v2.0";
        return issuer is not null && issuer.Length == prefix.Length + 36 + suffix.Length &&
            issuer.StartsWith(prefix, StringComparison.Ordinal) &&
            issuer.EndsWith(suffix, StringComparison.Ordinal) &&
            Guid.TryParseExact(issuer[prefix.Length..^suffix.Length], "D", out var tenant) &&
            tenant != Guid.Empty && options.Value.AllowsTenant(tenant);
    }

    private static bool IsTrustedKeysUri(string? value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
            uri.Host != "login.microsoftonline.com" || !uri.IsDefaultPort || uri.UserInfo.Length != 0 ||
            uri.Query.Length != 0 || uri.Fragment.Length != 0) return false;
        var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 4 && parts[1] == "discovery" && parts[2] == "v2.0" && parts[3] == "keys" &&
            (parts[0] is "common" or "organizations" or "consumers" || Guid.TryParseExact(parts[0], "D", out _));
    }

    private sealed record DiscoveryResponse(
        [property: JsonPropertyName("issuer")] string? Issuer,
        [property: JsonPropertyName("jwks_uri")] string? JwksUri);
}
