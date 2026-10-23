using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Contracts.LearningProfiles;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record SetCurrentLearningProfileRequest
{
    [Required, StringLength(19, MinimumLength = 1)] public string ProfileId { get; init; } = "";
}
