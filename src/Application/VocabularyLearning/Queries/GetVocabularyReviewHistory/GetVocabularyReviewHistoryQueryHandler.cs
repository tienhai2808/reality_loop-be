using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Validation;

namespace Application.VocabularyLearning.Queries.GetVocabularyReviewHistory;

public sealed class GetVocabularyReviewHistoryQueryHandler(ICurrentUser current, IVocabularyLearningRepository repository)
{
    public async Task<VocabularyReviewPage> Handle(GetVocabularyReviewHistoryQuery query, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(query.ProfileId);
        VocabularyLearningValidation.Id(query.ProgressId);
        VocabularyLearningValidation.PageSize(query.PageSize);
        var before = VocabularyLearningValidation.Before(query.Before);
        return await repository.GetHistoryAsync(current.UserId, query.ProfileId, query.ProgressId, query.PageSize, before, ct);
    }
}
