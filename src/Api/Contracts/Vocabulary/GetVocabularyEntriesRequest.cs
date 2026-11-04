using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Vocabulary;

namespace Api.Contracts.Vocabulary;

public sealed record GetVocabularyEntriesRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
    [EnumDataType(typeof(PartOfSpeech))] public PartOfSpeech? PartOfSpeech { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
