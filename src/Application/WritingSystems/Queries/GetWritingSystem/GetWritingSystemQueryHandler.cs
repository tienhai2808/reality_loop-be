using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystems.Models;

namespace Application.WritingSystems.Queries.GetWritingSystem;

public sealed class GetWritingSystemQueryHandler(IWritingSystemReadRepository repository)
{
    public async Task<WritingSystemDto> Handle(GetWritingSystemQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetByCodeAsync(language, code, locale, ct)
            ?? throw new NotFoundException("Writing system was not found.");
    }
}
