using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Validation;

namespace Application.VocabularyLearning.Queries.GetVocabularyLearningSummary;

public sealed class GetVocabularyLearningSummaryQueryHandler(ICurrentUser current, IVocabularyLearningRepository repository, TimeProvider clock)
{
    public async Task<VocabularyLearningSummaryDto> Handle(GetVocabularyLearningSummaryQuery query, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(query.ProfileId);
        return await repository.GetSummaryAsync(current.UserId, query.ProfileId,
            VocabularyLearningValidation.Timestamp(clock.GetUtcNow()), ct) ?? throw new NotFoundException("Learning profile was not found.");
    }
}
