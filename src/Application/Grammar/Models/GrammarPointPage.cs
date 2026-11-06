namespace Application.Grammar.Models;

public sealed record GrammarPointPage(IReadOnlyList<GrammarPointSummaryDto> Items, string? NextCursor);
