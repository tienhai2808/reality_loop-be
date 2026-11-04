namespace Application.Vocabulary.Models;

public sealed record VocabularySensePage(IReadOnlyList<VocabularySenseDto> Items, string? NextCursor);
