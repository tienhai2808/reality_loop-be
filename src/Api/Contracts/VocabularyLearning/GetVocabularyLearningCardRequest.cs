using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.VocabularyLearning;

public sealed record GetVocabularyLearningCardRequest
{
    [StringLength(35, MinimumLength = 2)] public string? Locale { get; init; }
}
