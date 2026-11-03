namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeSessionPage(IReadOnlyList<WritingSystemPracticeSessionSummaryDto> Items,
    string? NextCursor);
