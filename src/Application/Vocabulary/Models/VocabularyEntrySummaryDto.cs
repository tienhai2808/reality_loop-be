namespace Application.Vocabulary.Models;

public sealed record VocabularyEntrySummaryDto(string Id, string LanguageCode, string Code, string Term,
    string? Reading, string? AudioUrl, VocabularySenseSummaryDto? PrimarySense, int SortOrder,
    long Version, DateTimeOffset UpdatedAt);
