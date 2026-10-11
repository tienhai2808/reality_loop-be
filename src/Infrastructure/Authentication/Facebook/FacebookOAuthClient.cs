using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
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

namespace Infrastructure.Authentication.Facebook;

internal sealed class FacebookOAuthClient(IHttpClientFactory clients, IDataProtectionProvider protection,
    IOptions<FacebookOAuthOptions> options, TimeProvider clock) : IFacebookOAuthClient
{
    private readonly IDataProtector _protector = protection.CreateProtector("RealityLoop.FacebookOAuth.State.v1");

    public FacebookAuthorization CreateAuthorization()
    {
        var state = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        var settings = options.Value;
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = settings.AppId,
            ["redirect_uri"] = settings.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = "public_profile,email",
            ["state"] = state
        };
        var protectedState = _protector.Protect(JsonSerializer.Serialize(
            new LoginState(state, clock.GetUtcNow().AddMinutes(10))));
        return new FacebookAuthorization(AddQuery(settings.AuthorizationEndpoint, parameters), state, protectedState);
    }

    public async Task<OAuthIdentity> ExchangeCodeAsync(string code, string state, string protectedState, CancellationToken ct)
    {
        ValidateState(state, protectedState);
        try
        {
            var settings = options.Value;
            using var client = clients.CreateClient("FacebookOAuth");
            // Keep the app secret and authorization code out of the request URI.
            using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, settings.GraphEndpoint + "/oauth/access_token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = settings.AppId,
                    ["client_secret"] = settings.AppSecret,
                    ["redirect_uri"] = settings.RedirectUri,
                    ["grant_type"] = "authorization_code",
                    ["code"] = code
                })
            };
            var token = await SendAsync<TokenResponse>(client, tokenRequest, ct);
            if (string.IsNullOrWhiteSpace(token.AccessToken) || token.AccessToken.Length > 16384 ||
                !string.Equals(token.TokenType, "bearer", StringComparison.OrdinalIgnoreCase) || token.ExpiresIn <= 0)
                throw new AuthenticationException("Facebook did not return a valid user access token.");

            using var debugRequest = new HttpRequestMessage(HttpMethod.Get, AddQuery(settings.GraphEndpoint + "/debug_token",
                new Dictionary<string, string> { ["input_token"] = token.AccessToken }));
            // An app access token authorizes inspection; never send it to the browser or persist it.
            debugRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{settings.AppId}|{settings.AppSecret}");
            var debug = (await SendAsync<DebugResponse>(client, debugRequest, ct)).Data;
            var now = clock.GetUtcNow().ToUnixTimeSeconds();
            if (debug is null || !debug.IsValid || debug.AppId != settings.AppId || debug.Type != "USER" ||
                !IsUserId(debug.UserId) || debug.ExpiresAt <= now ||
                (debug.DataAccessExpiresAt is { } dataExpiry && dataExpiry != 0 && dataExpiry <= now) ||
                debug.Scopes?.Contains("public_profile", StringComparer.Ordinal) != true)
                throw new AuthenticationException("Invalid Facebook user token, app, or permissions. Start a new login.");

            // Fetch all profile fields (including picture) in one Graph request after token inspection.
            var proof = Convert.ToHexStringLower(HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(settings.AppSecret), Encoding.UTF8.GetBytes(token.AccessToken)));
            using var profileRequest = new HttpRequestMessage(HttpMethod.Get, AddQuery(settings.GraphEndpoint + "/me",
                new Dictionary<string, string>
                {
                    ["fields"] = "id,name,first_name,last_name,email,picture.type(large)",
                    ["appsecret_proof"] = proof
                }));
            profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var profile = await SendAsync<ProfileResponse>(client, profileRequest, ct);
            if (profile.Id != debug.UserId)
                throw new AuthenticationException("Facebook profile does not match the inspected user token.");

            // Email is optional. Account identity is the app-scoped user ID, never an email address.
            var emailClaim = profile.Email?.Trim();
            var email = emailClaim is { Length: <= 255 } && MailAddress.TryCreate(emailClaim, out var address) &&
                address.Address == emailClaim ? emailClaim.ToLowerInvariant() : null;
            var firstName = profile.FirstName ?? profile.Name ?? "Facebook user";
            var lastName = profile.LastName ?? "";
            var picture = ProfilePicture(profile.Picture?.Data);
            return new OAuthIdentity($"{settings.AppId}:{profile.Id}", email, LimitName(firstName), LimitName(lastName), picture);
        }
        catch (HttpRequestException ex)
        { throw new ExternalServiceException("Facebook authentication is unavailable. Try again later.", ex); }
        catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
        { throw new ExternalServiceException("Facebook authentication timed out. Try again later.", ex); }
        catch (JsonException)
        { throw new ExternalServiceException("Facebook returned an invalid authentication response."); }
        catch (ArgumentException)
        { throw new AuthenticationException("Facebook returned an invalid user access token."); }
    }

    private static async Task<T> SendAsync<T>(HttpClient client, HttpRequestMessage request, CancellationToken ct)
        where T : GraphResponse
    {
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var response = await client.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500)
            throw new ExternalServiceException("Facebook authentication is unavailable. Try again later.");
        var result = await response.Content.ReadFromJsonAsync<T>(ct)
            ?? throw new ExternalServiceException("Facebook returned an empty authentication response.");
        if (result.Error is { IsTransient: true } || result.Error?.Code is 1 or 2 or 4 or 17 or 32 or 341 or 613)
            throw new ExternalServiceException("Facebook authentication is unavailable. Try again later.");
        if (!response.IsSuccessStatusCode || result.Error is not null)
        {
            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden ||
                result.Error?.Code is 100 or 102 or 190 or 200)
                throw new AuthenticationException("Facebook rejected the login code, token, or permissions. Start a new login.");
            throw new ExternalServiceException("Facebook authentication is unavailable. Try again later.");
        }
        return result;
    }

    private void ValidateState(string state, string protectedState)
    {
        try
        {
            var login = JsonSerializer.Deserialize<LoginState>(_protector.Unprotect(protectedState));
            if (login is null || login.ExpiresAt <= clock.GetUtcNow() || !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(login.State), Encoding.UTF8.GetBytes(state)))
                throw new AuthenticationException("Facebook login state is invalid or expired. Start a new login.");
        }
        catch (CryptographicException) { throw new AuthenticationException("Invalid Facebook login state."); }
        catch (JsonException) { throw new AuthenticationException("Invalid Facebook login state."); }
    }

    private static string AddQuery(string url, IReadOnlyDictionary<string, string> parameters) => url + "?" +
        string.Join("&", parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
    private static bool IsUserId(string? value) => value is { Length: > 0 and <= 64 } &&
        value[0] != '0' && value.All(char.IsAsciiDigit);
    private static string LimitName(string name) => name.Length <= 150 ? name : name[..150];

    private static string? ProfilePicture(PictureData? picture)
    {
        if (picture is null || picture.IsSilhouette || picture.Url is not { Length: <= 255 } url ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
            uri.UserInfo.Length != 0 || uri.Fragment.Length != 0) return null;
        // Do not expose a URL containing Facebook credentials in a profile response.
        if (url.Contains("access_token", StringComparison.OrdinalIgnoreCase) ||
            url.Contains("appsecret_proof", StringComparison.OrdinalIgnoreCase)) return null;
        return url;
    }

    private sealed record LoginState(string State, DateTimeOffset ExpiresAt);
    private abstract record GraphResponse
    {
        [JsonPropertyName("error")] public GraphError? Error { get; init; }
    }
    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken,
        [property: JsonPropertyName("token_type")] string? TokenType,
        [property: JsonPropertyName("expires_in")] long ExpiresIn) : GraphResponse;
    private sealed record DebugResponse([property: JsonPropertyName("data")] TokenMetadata? Data) : GraphResponse;
    private sealed record TokenMetadata(
        [property: JsonPropertyName("app_id")] string? AppId,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("is_valid")] bool IsValid,
        [property: JsonPropertyName("user_id")] string? UserId,
        [property: JsonPropertyName("expires_at")] long ExpiresAt,
        [property: JsonPropertyName("data_access_expires_at")] long? DataAccessExpiresAt,
        [property: JsonPropertyName("scopes")] string[]? Scopes);
    private sealed record ProfileResponse(
        [property: JsonPropertyName("id")] string? Id,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("first_name")] string? FirstName,
        [property: JsonPropertyName("last_name")] string? LastName,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("picture")] PictureResponse? Picture) : GraphResponse;
    private sealed record PictureResponse([property: JsonPropertyName("data")] PictureData? Data);
    private sealed record PictureData([property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("is_silhouette")] bool IsSilhouette);
    private sealed record GraphError([property: JsonPropertyName("code")] int Code,
        [property: JsonPropertyName("is_transient")] bool IsTransient);
}
