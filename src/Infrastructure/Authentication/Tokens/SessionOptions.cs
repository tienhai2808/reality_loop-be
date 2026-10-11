using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication.Tokens;

public sealed class SessionOptions
{
    public const string SectionName = "Authentication:Session";
    [Range(1, 90)] public int RefreshTokenLifetimeDays { get; init; } = 7;
    [Range(1, 365)] public int AbsoluteLifetimeDays { get; init; } = 30;
}

