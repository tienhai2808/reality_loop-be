namespace Application.Vocabulary.Models;

public sealed record VocabularyEntryPage(IReadOnlyList<VocabularyEntrySummaryDto> Items, string? NextCursor);
