namespace Application.Pronunciation.Models;

public sealed record PronunciationRuleSummaryDto(string Id, string LanguageCode, string Code, string NativeName,
    string Name, string? TranslationLocale, bool IsTranslationFallback, int SortOrder,
    long Version, DateTimeOffset UpdatedAt);
