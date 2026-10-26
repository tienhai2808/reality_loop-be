using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.WritingSystems;

public sealed record GetWritingSystemItemsRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 100;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
