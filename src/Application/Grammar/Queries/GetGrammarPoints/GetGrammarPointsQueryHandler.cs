using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Grammar.Models;

namespace Application.Grammar.Queries.GetGrammarPoints;

public sealed class GetGrammarPointsQueryHandler(IGrammarReadRepository repository)
{
    public Task<GrammarPointPage> Handle(GetGrammarPointsQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetPageAsync(language, locale, query.PageSize, cursor, ct);
    }
}
