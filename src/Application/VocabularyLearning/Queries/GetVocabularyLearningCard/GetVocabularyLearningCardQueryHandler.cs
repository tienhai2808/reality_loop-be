using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Validation;

namespace Application.VocabularyLearning.Queries.GetVocabularyLearningCard;

public sealed class GetVocabularyLearningCardQueryHandler(ICurrentUser current, IVocabularyLearningRepository repository)
{
    public async Task<VocabularyFlashcardDto> Handle(GetVocabularyLearningCardQuery query, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(query.ProfileId);
        VocabularyLearningValidation.Id(query.ProgressId);
        var locale = query.Locale is null ? null : CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetCardAsync(current.UserId, query.ProfileId, query.ProgressId, locale, ct)
            ?? throw new NotFoundException("Available vocabulary learning card was not found.");
    }
}
