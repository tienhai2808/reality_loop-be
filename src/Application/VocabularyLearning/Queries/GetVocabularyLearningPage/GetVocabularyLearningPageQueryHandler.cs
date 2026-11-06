using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Validation;

namespace Application.VocabularyLearning.Queries.GetVocabularyLearningPage;

public sealed class GetVocabularyLearningPageQueryHandler(ICurrentUser current, IVocabularyLearningRepository repository, TimeProvider clock)
{
    public async Task<VocabularyLearningPage> Handle(GetVocabularyLearningPageQuery query, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(query.ProfileId);
        VocabularyLearningValidation.PageSize(query.PageSize);
        if (query.Status is { } status && !Enum.IsDefined(status)) throw new RequestValidationException("Unknown learning status.");
        var now = VocabularyLearningValidation.Timestamp(clock.GetUtcNow());
        var cursor = VocabularyLearningValidation.Cursor(query.After, now);
        var locale = query.Locale is null ? null : CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetPageAsync(current.UserId, query.ProfileId, query.DueOnly, query.Status, query.PageSize,
            cursor, cursor?.AsOf ?? now, locale, ct);
    }
}
