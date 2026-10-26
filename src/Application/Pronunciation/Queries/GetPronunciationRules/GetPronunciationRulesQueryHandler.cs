using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Pronunciation.Models;

namespace Application.Pronunciation.Queries.GetPronunciationRules;

public sealed class GetPronunciationRulesQueryHandler(IPronunciationReadRepository repository)
{
    public Task<PronunciationRulePage> Handle(GetPronunciationRulesQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetPageAsync(language, locale, query.PageSize, cursor, ct);
    }
}
