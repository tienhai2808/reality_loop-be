using Application.Common.Interfaces;
using Application.LearningProgress.Models;
using Application.LearningProgress.Validation;

namespace Application.LearningProgress.Queries.GetWritingSystemProgressSummary;

public sealed class GetWritingSystemProgressSummaryQueryHandler(ICurrentUser current, IWritingSystemItemProgressRepository repository)
{
    public Task<LearningProgressSummaryDto> Handle(GetWritingSystemProgressSummaryQuery query, CancellationToken ct)
    {
        LearningProgressValidation.Id(query.ProfileId);
        return repository.GetSummaryAsync(current.UserId, query.ProfileId, ct);
    }
}
