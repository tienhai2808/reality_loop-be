using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystems.Models;

namespace Application.WritingSystems.Queries.GetWritingSystems;

public sealed class GetWritingSystemsQueryHandler(IWritingSystemReadRepository repository)
{
    public async Task<WritingSystemPage> Handle(GetWritingSystemsQuery query, CancellationToken ct)
    {
        var code = CatalogQueryValidation.Language(query.LanguageCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return await repository.GetPageAsync(code, locale, query.PageSize, cursor, ct);
    }
}
