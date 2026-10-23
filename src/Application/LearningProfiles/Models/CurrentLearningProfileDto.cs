namespace Application.LearningProfiles.Models;

public sealed record CurrentLearningProfileDto(LearningProfileDto Profile, DateTimeOffset SelectionUpdatedAt);
