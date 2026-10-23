namespace Application.LearningProfiles.Models;

public sealed record LearningProfilePage(IReadOnlyList<LearningProfileDto> Items, string? NextCursor);
