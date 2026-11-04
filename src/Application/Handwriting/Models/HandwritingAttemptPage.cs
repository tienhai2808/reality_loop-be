namespace Application.Handwriting.Models;

public sealed record HandwritingAttemptPage(IReadOnlyList<HandwritingAttemptSummaryDto> Items, string? NextCursor);
