using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
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

namespace Infrastructure.Authentication.Google;

internal sealed class GoogleOAuthClient(IHttpClientFactory clients, GoogleSigningKeys signingKeys,
    IDataProtectionProvider protection, IOptions<GoogleOAuthOptions> options, TimeProvider clock) : IGoogleOAuthClient
{
    private readonly IDataProtector _protector = protection.CreateProtector("RealityLoop.GoogleOAuth.State.v1");

    public GoogleAuthorization CreateAuthorization()
    {
        var state = RandomValue();
        var nonce = RandomValue();
        var verifier = RandomValue();
        var challenge = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
        var settings = options.Value;
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = settings.ClientId,
            ["redirect_uri"] = settings.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["state"] = state,
            ["nonce"] = nonce,
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["prompt"] = "select_account"
        };
        var url = "https://accounts.google.com/o/oauth2/v2/auth?" + string.Join("&", parameters.Select(
            x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
        var protectedState = _protector.Protect(JsonSerializer.Serialize(
            new LoginState(state, nonce, verifier, clock.GetUtcNow().AddMinutes(10))));
        return new GoogleAuthorization(url, state, protectedState);
    }

    public async Task<GoogleIdentity> ExchangeCodeAsync(string code, string state, string protectedState, CancellationToken ct)
    {
        var login = ReadState(state, protectedState);
        try
        {
            var settings = options.Value;
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = settings.ClientId,
                ["client_secret"] = settings.ClientSecret,
                ["redirect_uri"] = settings.RedirectUri,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["code_verifier"] = login.Verifier
            });
            using var response = await clients.CreateClient("GoogleOAuth")
                .PostAsync("https://oauth2.googleapis.com/token", content, ct);
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthenticationException("Google rejected the authorization code. Start a new login.");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>(ct)
                ?? throw new AuthenticationException("Google did not return an ID token.");
            if (string.IsNullOrWhiteSpace(result.IdToken) || result.IdToken.Length > 16384)
                throw new AuthenticationException("Google did not return a valid ID token.");

            ClaimsPrincipal principal;
            try { principal = await ValidateIdTokenAsync(result.IdToken, false, ct); }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                // Google rotates signing keys; refresh the cached JWKS once for an unknown key.
                principal = await ValidateIdTokenAsync(result.IdToken, true, ct);
            }

            if (!FixedTimeEquals(principal.FindFirst("nonce")?.Value, login.Nonce))
                throw new AuthenticationException("Invalid Google nonce.");
            var authorizedParty = principal.FindFirst("azp")?.Value;
            if (authorizedParty is not null && authorizedParty != settings.ClientId)
                throw new AuthenticationException("Invalid Google authorized party.");
            if (!bool.TryParse(principal.FindFirst("email_verified")?.Value, out var verified) || !verified)
                throw new AuthenticationException("Google email must be verified.");
            var subject = principal.FindFirst("sub")?.Value;
            var email = principal.FindFirst("email")?.Value;
            if (string.IsNullOrWhiteSpace(subject) || subject.Length > 255 ||
                string.IsNullOrWhiteSpace(email) || email.Length > 255)
                throw new AuthenticationException("Google identity is incomplete.");
            var firstName = principal.FindFirst("given_name")?.Value ?? principal.FindFirst("name")?.Value ?? "Google user";
            var lastName = principal.FindFirst("family_name")?.Value ?? "";
            var picture = principal.FindFirst("picture")?.Value;
            if (picture?.Length > 255) picture = null;
            return new GoogleIdentity(subject, email.Trim().ToLowerInvariant(), LimitName(firstName), LimitName(lastName), picture);
        }
        catch (SecurityTokenException) { throw new AuthenticationException("Invalid Google ID token."); }
        catch (HttpRequestException ex) { throw new ExternalServiceException("Google authentication is unavailable. Try again later.", ex); }
        catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
        { throw new ExternalServiceException("Google authentication timed out. Try again later.", ex); }
        catch (JsonException) { throw new ExternalServiceException("Google returned an invalid authentication response."); }
        catch (ArgumentException) { throw new AuthenticationException("Invalid Google ID token."); }
    }

    private async Task<ClaimsPrincipal> ValidateIdTokenAsync(string token, bool forceRefresh, CancellationToken ct)
    {
        var keys = await signingKeys.GetAsync(forceRefresh, ct);
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        return handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
            ValidateAudience = true,
            ValidAudience = options.Value.ClientId,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys,
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
                throw new AuthenticationException("Google login state is invalid or expired. Start a new login.");
            return login;
        }
        catch (CryptographicException) { throw new AuthenticationException("Invalid Google login state."); }
        catch (JsonException) { throw new AuthenticationException("Invalid Google login state."); }
    }

    private static bool FixedTimeEquals(string? first, string second) => first is not null &&
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(first), Encoding.UTF8.GetBytes(second));
    private static string RandomValue() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
    private static string LimitName(string name) => name.Length <= 150 ? name : name[..150];
    private sealed record LoginState(string State, string Nonce, string Verifier, DateTimeOffset ExpiresAt);
    private sealed record TokenResponse([property: JsonPropertyName("id_token")] string? IdToken);
}
