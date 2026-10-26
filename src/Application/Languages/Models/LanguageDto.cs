namespace Application.Languages.Models;

public sealed record LanguageDto(string Id, string Code, string NativeName, string Name, string? Description,
    string? TranslationLocale, bool IsTranslationFallback, int SortOrder, long Version, DateTimeOffset UpdatedAt);
