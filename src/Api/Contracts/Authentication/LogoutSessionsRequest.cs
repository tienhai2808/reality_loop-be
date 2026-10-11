using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Authentication;

public sealed record LogoutSessionsRequest
{
    [Required, MinLength(1), MaxLength(100)] public string[] SessionIds { get; init; } = [];
}
