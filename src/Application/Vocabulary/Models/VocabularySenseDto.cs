using Domain.Vocabulary;

namespace Application.Vocabulary.Models;

public sealed record VocabularySenseDto(string Id, string Code, PartOfSpeech PartOfSpeech, string? Meaning,
    string? UsageNote, string? TranslationLocale, bool IsTranslationFallback, int SortOrder,
    long Version, DateTimeOffset UpdatedAt);
