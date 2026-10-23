using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Application.LearningProfiles.Validation;

namespace Application.LearningProfiles.Queries.GetLearningProfiles;

public sealed class GetLearningProfilesQueryHandler(ICurrentUser current, ILearningProfileRepository profiles)
{
    public Task<LearningProfilePage> Handle(GetLearningProfilesQuery query, CancellationToken ct)
    {
        if (query.PageSize is < 1 or > 100)
            throw new RequestValidationException("Page size must be between 1 and 100.");
        long? after = query.After is null ? null : LearningProfileValidation.ParseId(query.After);
        return profiles.GetPageAsync(current.UserId, query.PageSize, after, ct);
    }
}
