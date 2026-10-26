namespace Application.WritingSystems.Models;

public sealed record WritingSystemItemPage(IReadOnlyList<WritingSystemItemDto> Items, string? NextCursor);
