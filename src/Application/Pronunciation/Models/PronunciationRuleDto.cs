namespace Application.Pronunciation.Models;

public sealed record PronunciationRuleDto(string Id, string LanguageCode, string Code, string NativeName,
    string Name, string? Explanation, string? UsageNote, string? TranslationLocale,
    bool IsTranslationFallback, int SortOrder, long Version, DateTimeOffset UpdatedAt);
