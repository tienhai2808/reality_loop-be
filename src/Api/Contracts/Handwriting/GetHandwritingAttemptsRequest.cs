using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Handwriting;

public sealed record GetHandwritingAttemptsRequest
{
    [StringLength(64, MinimumLength = 1)] public string? SystemCode { get; init; }
    public bool? Passed { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19, MinimumLength = 1)] public string? Before { get; init; }
}
