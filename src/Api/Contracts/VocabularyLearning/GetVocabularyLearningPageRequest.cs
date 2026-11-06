using System.ComponentModel.DataAnnotations;
using Domain.VocabularyLearning;

namespace Api.Contracts.VocabularyLearning;

public sealed record GetVocabularyLearningPageRequest
{
    public bool DueOnly { get; init; } = true;
    [EnumDataType(typeof(VocabularyLearningStatus))] public VocabularyLearningStatus? Status { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(59, MinimumLength = 1)] public string? After { get; init; }
    [StringLength(35, MinimumLength = 2)] public string? Locale { get; init; }
}
