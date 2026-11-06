namespace Application.Grammar.Models;

public sealed record GrammarPointDto(string Id, string LanguageCode, string Code, string NativeName,
    string Pattern, string Name, string? Explanation, string? UsageNote, string? TranslationLocale,
    bool IsTranslationFallback, int SortOrder, long Version, DateTimeOffset UpdatedAt);
