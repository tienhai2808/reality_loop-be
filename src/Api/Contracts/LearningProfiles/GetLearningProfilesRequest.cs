using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.LearningProfiles;

public sealed record GetLearningProfilesRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19, MinimumLength = 1)] public string? After { get; init; }
}
