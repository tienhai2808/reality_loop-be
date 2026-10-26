namespace Application.WritingSystems.Models;

public sealed record WritingSystemDto(string Id, string LanguageCode, string Code, string NativeName, string Name,
    string? Description, string? TranslationLocale, bool IsTranslationFallback, int SortOrder,
    long Version, DateTimeOffset UpdatedAt);
