using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Languages.Models;

namespace Application.Languages.Queries.GetLanguages;

public sealed class GetLanguagesQueryHandler(ILanguageReadRepository repository)
{
    public async Task<LanguagePage> Handle(GetLanguagesQuery query, CancellationToken ct)
    {
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return await repository.GetPageAsync(locale, query.PageSize, cursor, ct);
    }
}
