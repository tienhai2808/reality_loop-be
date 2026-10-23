using Application.Common.Interfaces;
using Application.LearningProfiles.Models;

namespace Application.LearningProfiles.Queries.GetCurrentLearningProfile;

public sealed class GetCurrentLearningProfileQueryHandler(ICurrentUser current, ILearningProfileRepository profiles)
{
    public Task<CurrentLearningProfileDto?> Handle(GetCurrentLearningProfileQuery query, CancellationToken ct) =>
        profiles.GetCurrentAsync(current.UserId, ct);
}
