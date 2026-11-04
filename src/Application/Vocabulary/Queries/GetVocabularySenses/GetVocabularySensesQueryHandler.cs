using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Vocabulary.Models;

namespace Application.Vocabulary.Queries.GetVocabularySenses;

public sealed class GetVocabularySensesQueryHandler(IVocabularyReadRepository repository)
{
    public Task<VocabularySensePage> Handle(GetVocabularySensesQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.EntryCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetSensesPageAsync(language, code, locale, query.PageSize, cursor, ct);
    }
}
