using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Authentication.Models;
using Application.Common.Interfaces;
using Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication.Tokens;

internal sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    public IssuedAccessToken CreateAccessToken(User user, Session session, DateTimeOffset now)
    {
        var settings = options.Value;
        var expiresAt = now.AddMinutes(settings.AccessTokenLifetimeMinutes);
        if (expiresAt > session.ExpiresAt) expiresAt = session.ExpiresAt;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim("sid", session.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
        };
        var jwt = new JwtSecurityToken(settings.Issuer, settings.Audience, claims, now.UtcDateTime,
            expiresAt.UtcDateTime, new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)), SecurityAlgorithms.HmacSha256));
        return new IssuedAccessToken(new JwtSecurityTokenHandler().WriteToken(jwt), expiresAt);
    }

    public IssuedRefreshToken CreateRefreshToken()
    {
        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        return new IssuedRefreshToken(token, HashRefreshToken(token));
    }

    public string HashRefreshToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
