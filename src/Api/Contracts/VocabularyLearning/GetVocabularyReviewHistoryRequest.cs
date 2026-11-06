using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.VocabularyLearning;

public sealed record GetVocabularyReviewHistoryRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19, MinimumLength = 1)] public string? Before { get; init; }
}
