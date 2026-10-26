namespace Application.Pronunciation.Models;

public sealed record PronunciationExamplePage(IReadOnlyList<PronunciationExampleDto> Items, string? NextCursor);
