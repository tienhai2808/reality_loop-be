namespace Application.WritingSystems.Models;

public sealed record WritingSystemPage(IReadOnlyList<WritingSystemDto> Items, string? NextCursor);
