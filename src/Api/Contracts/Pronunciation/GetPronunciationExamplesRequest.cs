using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Pronunciation;

public sealed record GetPronunciationExamplesRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
