using System.Globalization;
using Domain.LearningProfiles;

namespace Application.LearningProfiles.Models;

public sealed record LearningProfileDto(string Id, string TargetLanguage, string NativeLanguage,
    string ExplanationLanguage, ProficiencyLevel ProficiencyLevel, LearningGoal PrimaryGoal,
    string? GoalNote, long Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static LearningProfileDto From(LearningProfile profile) => new(
        profile.Id.ToString(CultureInfo.InvariantCulture), profile.TargetLanguage, profile.NativeLanguage,
        profile.ExplanationLanguage, profile.ProficiencyLevel, profile.PrimaryGoal, profile.GoalNote,
        profile.Version, profile.CreatedAt, profile.UpdatedAt);
}
