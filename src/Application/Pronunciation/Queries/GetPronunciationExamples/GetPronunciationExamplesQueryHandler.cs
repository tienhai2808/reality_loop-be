using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Pronunciation.Models;

namespace Application.Pronunciation.Queries.GetPronunciationExamples;

public sealed class GetPronunciationExamplesQueryHandler(IPronunciationReadRepository repository)
{
    public Task<PronunciationExamplePage> Handle(GetPronunciationExamplesQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.RuleCode);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetExamplesPageAsync(language, code, query.PageSize, cursor, ct);
    }
}
