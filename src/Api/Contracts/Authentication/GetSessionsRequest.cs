using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Authentication;

public sealed record GetSessionsRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19)] public string? Before { get; init; }
}
