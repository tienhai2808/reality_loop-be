namespace Application.Grammar.Models;

public sealed record GrammarPointSummaryDto(string Id, string LanguageCode, string Code, string NativeName,
    string Pattern, string Name, string? TranslationLocale, bool IsTranslationFallback, int SortOrder,
    long Version, DateTimeOffset UpdatedAt);
