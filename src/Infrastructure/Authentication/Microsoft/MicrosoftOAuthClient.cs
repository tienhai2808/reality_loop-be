using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthenticationException = Application.Common.Exceptions.AuthenticationException;

namespace Infrastructure.Authentication.Microsoft;

internal sealed class MicrosoftOAuthClient(IHttpClientFactory clients, MicrosoftOpenIdConfiguration configuration,
    IDataProtectionProvider protection, IOptions<MicrosoftOAuthOptions> options, TimeProvider clock) : IMicrosoftOAuthClient
{
    private readonly IDataProtector _protector = protection.CreateProtector("RealityLoop.MicrosoftOAuth.State.v1");

    public MicrosoftAuthorization CreateAuthorization()
    {
        var state = RandomValue();
        var nonce = RandomValue();
        var verifier = RandomValue();
        var settings = options.Value;
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = settings.ClientId,
            ["redirect_uri"] = settings.RedirectUri,
            ["response_type"] = "code",
            ["response_mode"] = "query",
            ["scope"] = "openid email profile",
            ["state"] = state,
            ["nonce"] = nonce,
            ["code_challenge"] = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier))),
            ["code_challenge_method"] = "S256",
            ["prompt"] = "select_account"
        };
        var url = settings.Authority + "/oauth2/v2.0/authorize?" + string.Join("&", parameters.Select(
            x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
        var protectedState = _protector.Protect(JsonSerializer.Serialize(
            new LoginState(state, nonce, verifier, clock.GetUtcNow().AddMinutes(10))));
        return new MicrosoftAuthorization(url, state, protectedState);
    }

    public async Task<OAuthIdentity> ExchangeCodeAsync(string code, string state, string protectedState, CancellationToken ct)
    {
        var login = ReadState(state, protectedState);
        try
        {
            var settings = options.Value;
            using var client = clients.CreateClient("MicrosoftOAuth");
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = settings.ClientId,
                ["client_secret"] = settings.ClientSecret,
                ["redirect_uri"] = settings.RedirectUri,
                ["grant_type"] = "authorization_code",
                ["scope"] = "openid email profile",
                ["code"] = code,
                ["code_verifier"] = login.Verifier
            });
            using var response = await client.PostAsync(settings.Authority + "/oauth2/v2.0/token", content, ct);
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthenticationException("Microsoft rejected the authorization code. Start a new login.");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>(ct);
            if (string.IsNullOrWhiteSpace(result?.IdToken) || result.IdToken.Length > 16384)
                throw new AuthenticationException("Microsoft did not return a valid ID token.");

            ClaimsPrincipal principal;
            try { principal = await ValidateIdTokenAsync(result.IdToken, false, ct); }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                principal = await ValidateIdTokenAsync(result.IdToken, true, ct);
            }
            if (!FixedTimeEquals(principal.FindFirst("nonce")?.Value, login.Nonce))
                throw new AuthenticationException("Invalid Microsoft nonce.");
            var party = principal.FindFirst("azp")?.Value;
            if (party is not null && party != settings.ClientId)
                throw new AuthenticationException("Invalid Microsoft authorized party.");
            if (!Guid.TryParseExact(principal.FindFirst("oid")?.Value, "D", out var objectId) || objectId == Guid.Empty ||
                string.IsNullOrWhiteSpace(principal.FindFirst("sub")?.Value))
                throw new AuthenticationException("Microsoft identity is incomplete.");
            var tenant = Guid.Parse(principal.FindFirst("tid")!.Value);

            // Microsoft email is optional and not proof of ownership. Never identify/link by this claim.
            var emailClaim = principal.FindFirst("email")?.Value?.Trim();
            var email = emailClaim is { Length: <= 255 } && MailAddress.TryCreate(emailClaim, out var address) &&
                address.Address == emailClaim ? emailClaim.ToLowerInvariant() : null;
            var firstName = principal.FindFirst("given_name")?.Value ?? principal.FindFirst("name")?.Value ?? "Microsoft user";
            var lastName = principal.FindFirst("family_name")?.Value ?? "";
            // Photos require a separate Graph request and permission; ID tokens have no trusted photo claim.
            return new OAuthIdentity($"{tenant:D}:{objectId:D}", email, LimitName(firstName), LimitName(lastName), null);
        }
        catch (SecurityTokenException) { throw new AuthenticationException("Invalid Microsoft ID token."); }
        catch (HttpRequestException ex) { throw new ExternalServiceException("Microsoft authentication is unavailable. Try again later.", ex); }
        catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
        { throw new ExternalServiceException("Microsoft authentication timed out. Try again later.", ex); }
        catch (JsonException) { throw new ExternalServiceException("Microsoft returned an invalid authentication response."); }
        catch (ArgumentException) { throw new AuthenticationException("Invalid Microsoft ID token."); }
    }

    private async Task<ClaimsPrincipal> ValidateIdTokenAsync(string token, bool forceRefresh, CancellationToken ct)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var jwt = handler.ReadJwtToken(token);
        // Unverified tid only selects an issuer/key from trusted metadata; no token-derived URL is fetched.
        if (jwt.Claims.FirstOrDefault(x => x.Type == "ver")?.Value != "2.0" ||
            !Guid.TryParseExact(jwt.Claims.FirstOrDefault(x => x.Type == "tid")?.Value, "D", out var tenant) ||
            tenant == Guid.Empty || !options.Value.AllowsTenant(tenant) || string.IsNullOrEmpty(jwt.Header.Kid))
            throw new AuthenticationException("Invalid Microsoft tenant or token version.");
        var issuer = $"https://login.microsoftonline.com/{tenant:D}/v2.0";
        var metadata = await configuration.GetAsync(forceRefresh, ct);
        if (jwt.Issuer != issuer || metadata.Issuer.Replace("{tenantid}", tenant.ToString("D"), StringComparison.Ordinal) != issuer)
            throw new AuthenticationException("Invalid Microsoft issuer.");

        var matchingKeys = metadata.Keys.Where(key => key.Kid == jwt.Header.Kid).ToArray();
        if (matchingKeys.Length == 0) throw new SecurityTokenSignatureKeyNotFoundException("Unknown Microsoft signing key.");
        var scopedKeys = matchingKeys.Where(key => KeyIssuer(key)?.Replace("{tenantid}", tenant.ToString("D"),
            StringComparison.Ordinal) == issuer).Cast<SecurityKey>().ToArray();
        if (scopedKeys.Length == 0) throw new AuthenticationException("Microsoft signing key issuer does not match the tenant.");

        return handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = options.Value.ClientId,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = scopedKeys,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ClockSkew = TimeSpan.FromSeconds(30)
        }, out _);
    }

    private LoginState ReadState(string state, string protectedState)
    {
        try
        {
            var login = JsonSerializer.Deserialize<LoginState>(_protector.Unprotect(protectedState));
            if (login is null || login.ExpiresAt <= clock.GetUtcNow() || !FixedTimeEquals(login.State, state))
                throw new AuthenticationException("Microsoft login state is invalid or expired. Start a new login.");
            return login;
        }
        catch (CryptographicException) { throw new AuthenticationException("Invalid Microsoft login state."); }
        catch (JsonException) { throw new AuthenticationException("Invalid Microsoft login state."); }
    }

    private static string? KeyIssuer(JsonWebKey key) => key.AdditionalData.TryGetValue("issuer", out var value)
        ? value is JsonElement element && element.ValueKind == JsonValueKind.String ? element.GetString() : value as string
        : null;
    private static bool FixedTimeEquals(string? first, string second) => first is not null &&
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(first), Encoding.UTF8.GetBytes(second));
    private static string RandomValue() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
    private static string LimitName(string name) => name.Length <= 150 ? name : name[..150];
    private sealed record LoginState(string State, string Nonce, string Verifier, DateTimeOffset ExpiresAt);
    private sealed record TokenResponse([property: JsonPropertyName("id_token")] string? IdToken);
}
