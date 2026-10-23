using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Application.LearningProfiles.Validation;

namespace Application.LearningProfiles.Queries.GetLearningProfile;

public sealed class GetLearningProfileQueryHandler(ICurrentUser current, ILearningProfileRepository profiles)
{
    public async Task<LearningProfileDto> Handle(GetLearningProfileQuery query, CancellationToken ct)
    {
        LearningProfileValidation.Id(query.Id);
        return await profiles.GetOwnedAsync(current.UserId, query.Id, ct)
            ?? throw new NotFoundException("Learning profile was not found.");
    }
}
