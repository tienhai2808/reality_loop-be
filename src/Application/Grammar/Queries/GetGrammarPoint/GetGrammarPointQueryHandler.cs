using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Grammar.Models;

namespace Application.Grammar.Queries.GetGrammarPoint;

public sealed class GetGrammarPointQueryHandler(IGrammarReadRepository repository)
{
    public async Task<GrammarPointDto> Handle(GetGrammarPointQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.PointCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetByCodeAsync(language, code, locale, ct)
            ?? throw new NotFoundException("Grammar point was not found.");
    }
}
