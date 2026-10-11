using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication.Google;

internal sealed class GoogleSigningKeys(IHttpClientFactory clients, IMemoryCache cache)
{
    private const string CacheKey = "google-oidc-signing-keys";
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IList<SecurityKey>> GetAsync(bool forceRefresh, CancellationToken ct)
    {
        if (!forceRefresh && cache.TryGetValue(CacheKey, out IList<SecurityKey>? cached) && cached is not null)
            return cached;
        await _gate.WaitAsync(ct);
        try
        {
            if (!forceRefresh && cache.TryGetValue(CacheKey, out cached) && cached is not null) return cached;
            using var response = await clients.CreateClient("GoogleOAuth").GetAsync("https://www.googleapis.com/oauth2/v3/certs", ct);
            response.EnsureSuccessStatusCode();
            var keys = new JsonWebKeySet(await response.Content.ReadAsStringAsync(ct)).GetSigningKeys();
            var maxAge = response.Headers.CacheControl?.MaxAge ?? TimeSpan.FromHours(1);
            if (maxAge > TimeSpan.FromHours(6)) maxAge = TimeSpan.FromHours(6);
            cache.Set(CacheKey, keys, maxAge);
            return keys;
        }
        finally { _gate.Release(); }
    }
}
