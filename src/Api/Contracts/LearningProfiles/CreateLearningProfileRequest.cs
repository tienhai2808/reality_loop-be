using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.LearningProfiles;

namespace Api.Contracts.LearningProfiles;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CreateLearningProfileRequest
{
    [Required, StringLength(LearningProfile.LanguageMaxLength)] public string TargetLanguage { get; init; } = "";
    [Required, StringLength(LearningProfile.LanguageMaxLength)] public string NativeLanguage { get; init; } = "";
    [Required, StringLength(LearningProfile.LanguageMaxLength)] public string ExplanationLanguage { get; init; } = "";
    [EnumDataType(typeof(ProficiencyLevel))] public ProficiencyLevel ProficiencyLevel { get; init; }
    [EnumDataType(typeof(LearningGoal))] public LearningGoal PrimaryGoal { get; init; }
    [StringLength(LearningProfile.GoalNoteMaxLength)] public string? GoalNote { get; init; }
}
