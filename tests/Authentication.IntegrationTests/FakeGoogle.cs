using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

internal sealed class FakeGoogle : HttpMessageHandler, IHttpClientFactory
{
    private readonly RSA _rsa = RSA.Create(2048);
    private readonly ConcurrentDictionary<string, Identity> _codes = new();
    public int ExchangeCount { get; private set; }

    public HttpClient CreateClient(string name) => new(this, disposeHandler: false);

    public (string Code, string State) CreateCode(string authorizationUrl, string subject,
        string? issuer = null, string? audience = null, string? nonce = null, bool expired = false,
        bool forgedSignature = false, bool verifiedEmail = true)
    {
        var query = QueryHelpers.ParseQuery(new Uri(authorizationUrl).Query);
        Check(query["code_challenge_method"] == "S256", "PKCE S256 is required");
        Check(query["scope"] == "openid email profile", "Only login scopes requested");
        var code = Guid.NewGuid().ToString("N");
        _codes[code] = new Identity(subject, issuer ?? "https://accounts.google.com", audience ?? "test-client",
            nonce ?? query["nonce"].ToString(), query["code_challenge"].ToString(), expired, forgedSignature, verifiedEmail);
        return (code, query["state"].ToString());
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.RequestUri!.AbsoluteUri == "https://www.googleapis.com/oauth2/v3/certs")
        {
            var key = _rsa.ExportParameters(false);
            return Json(new { keys = new[] { new { kty = "RSA", use = "sig", alg = "RS256", kid = "test-key",
                n = Base64UrlEncoder.Encode(key.Modulus!), e = Base64UrlEncoder.Encode(key.Exponent!) } } });
        }
        Check(request.RequestUri.AbsoluteUri == "https://oauth2.googleapis.com/token", "Only expected Google URLs used");
        var form = QueryHelpers.ParseQuery(await request.Content!.ReadAsStringAsync(ct));
        Check(form["client_secret"] == "test-secret", "Google client secret stays on the server");
        Check(form["redirect_uri"] == "http://localhost/api/v1/auth/google/callback", "Redirect URI matches the start step");
        Check(form["grant_type"] == "authorization_code", "Authorization code grant used");
        ExchangeCount++;
        if (!_codes.TryRemove(form["code"].ToString(), out var identity))
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = JsonContent.Create(new { error = "invalid_grant" }) };
        var challenge = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(form["code_verifier"].ToString())));
        Check(challenge == identity.Challenge, "PKCE verifier matches challenge");
        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim("sub", identity.Subject),
            new Claim("nonce", identity.Nonce),
            new Claim("email", identity.Subject + "@example.com"),
            new Claim("email_verified", identity.VerifiedEmail ? "true" : "false", ClaimValueTypes.Boolean),
            new Claim("given_name", "Test"), new Claim("family_name", "User"),
            new Claim("picture", "https://example.com/avatar.png")
        };
        using var wrongKey = identity.ForgedSignature ? RSA.Create(2048) : null;
        var signingKey = new RsaSecurityKey(wrongKey ?? _rsa) { KeyId = "test-key" };
        var token = new JwtSecurityToken(identity.Issuer, identity.Audience, claims,
            now.AddMinutes(-10), identity.Expired ? now.AddMinutes(-5) : now.AddMinutes(5),
            new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256));
        return Json(new { id_token = new JwtSecurityTokenHandler().WriteToken(token), access_token = "google-only-token" });
    }

    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK) { Content = JsonContent.Create(value) };
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed record Identity(string Subject, string Issuer, string Audience, string Nonce, string Challenge,
        bool Expired, bool ForgedSignature, bool VerifiedEmail);
}
