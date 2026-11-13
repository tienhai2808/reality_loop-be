namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeSessionPage(IReadOnlyList<GrammarPracticeSessionSummaryDto> Items, string? NextCursor);
