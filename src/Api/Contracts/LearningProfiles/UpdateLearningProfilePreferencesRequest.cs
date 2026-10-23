using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.LearningProfiles;

namespace Api.Contracts.LearningProfiles;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record UpdateLearningProfilePreferencesRequest
{
    [Range(1, long.MaxValue)] public long Version { get; init; }
    [Required, StringLength(LearningProfile.LanguageMaxLength)] public string NativeLanguage { get; init; } = "";
    [Required, StringLength(LearningProfile.LanguageMaxLength)] public string ExplanationLanguage { get; init; } = "";
    [EnumDataType(typeof(ProficiencyLevel))] public ProficiencyLevel ProficiencyLevel { get; init; }
    [EnumDataType(typeof(LearningGoal))] public LearningGoal PrimaryGoal { get; init; }
    [StringLength(LearningProfile.GoalNoteMaxLength)] public string? GoalNote { get; init; }
}
