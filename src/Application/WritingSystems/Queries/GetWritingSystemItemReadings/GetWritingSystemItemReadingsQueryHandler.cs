using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystems.Models;

namespace Application.WritingSystems.Queries.GetWritingSystemItemReadings;

public sealed class GetWritingSystemItemReadingsQueryHandler(IWritingSystemReadRepository repository)
{
    public async Task<WritingSystemItemReadingPage> Handle(GetWritingSystemItemReadingsQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var system = CatalogQueryValidation.Code(query.SystemCode);
        var item = CatalogQueryValidation.Code(query.ItemCode);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return await repository.GetReadingsPageAsync(language, system, item, query.PageSize, cursor, ct);
    }
}
