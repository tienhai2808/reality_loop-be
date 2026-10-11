using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication.Tokens;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";
    [Required] public string Issuer { get; init; } = "";
    [Required] public string Audience { get; init; } = "";
    [Required] public string SigningKey { get; init; } = "";
    [Range(1, 60)] public int AccessTokenLifetimeMinutes { get; init; } = 15;
}
