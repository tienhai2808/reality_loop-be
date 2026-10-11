using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Authentication;

public sealed record RefreshTokenRequest
{
    [Required, StringLength(512)] public string RefreshToken { get; init; } = "";
}
