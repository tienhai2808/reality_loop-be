using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystems.Models;

namespace Application.WritingSystems.Queries.GetWritingSystemItems;

public sealed class GetWritingSystemItemsQueryHandler(IWritingSystemReadRepository repository)
{
    public async Task<WritingSystemItemPage> Handle(GetWritingSystemItemsQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return await repository.GetItemsPageAsync(language, code, query.PageSize, cursor, ct);
    }
}
