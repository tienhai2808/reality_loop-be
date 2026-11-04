namespace Application.Vocabulary.Models;

public sealed record VocabularyEntryDto(string Id, string LanguageCode, string Code, string Term,
    string? Reading, string? AudioUrl, int SortOrder, long Version, DateTimeOffset UpdatedAt);
