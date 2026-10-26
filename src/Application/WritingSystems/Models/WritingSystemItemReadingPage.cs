namespace Application.WritingSystems.Models;

public sealed record WritingSystemItemReadingPage(IReadOnlyList<WritingSystemItemReadingDto> Items, string? NextCursor);
