namespace Application.Pronunciation.Models;

public sealed record PronunciationRulePage(IReadOnlyList<PronunciationRuleSummaryDto> Items, string? NextCursor);
