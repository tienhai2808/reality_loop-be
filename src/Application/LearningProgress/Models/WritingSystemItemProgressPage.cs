namespace Application.LearningProgress.Models;

public sealed record WritingSystemItemProgressPage(IReadOnlyList<WritingSystemItemProgressDto> Items, string? NextCursor);
