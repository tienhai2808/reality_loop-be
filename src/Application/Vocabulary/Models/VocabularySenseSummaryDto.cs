using Domain.Vocabulary;

namespace Application.Vocabulary.Models;

public sealed record VocabularySenseSummaryDto(string Id, string Code, PartOfSpeech PartOfSpeech, string? Meaning,
    string? TranslationLocale, bool IsTranslationFallback, int SortOrder, long Version, DateTimeOffset UpdatedAt);
