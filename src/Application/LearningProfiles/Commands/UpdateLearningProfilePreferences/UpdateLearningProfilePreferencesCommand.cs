using Domain.LearningProfiles;

namespace Application.LearningProfiles.Commands.UpdateLearningProfilePreferences;

public sealed record UpdateLearningProfilePreferencesCommand(long Id, long Version, string NativeLanguage, string ExplanationLanguage,
    ProficiencyLevel ProficiencyLevel, LearningGoal PrimaryGoal, string? GoalNote);
