using Domain.LearningProfiles;

namespace Application.LearningProfiles.Commands.CreateLearningProfile;

public sealed record CreateLearningProfileCommand(string TargetLanguage, string NativeLanguage, string ExplanationLanguage,
    ProficiencyLevel ProficiencyLevel, LearningGoal PrimaryGoal, string? GoalNote);
