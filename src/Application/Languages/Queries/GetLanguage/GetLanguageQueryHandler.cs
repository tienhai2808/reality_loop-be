using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Languages.Models;

namespace Application.Languages.Queries.GetLanguage;

public sealed class GetLanguageQueryHandler(ILanguageReadRepository repository)
{
    public async Task<LanguageDto> Handle(GetLanguageQuery query, CancellationToken ct)
    {
        var code = CatalogQueryValidation.Language(query.LanguageCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetByCodeAsync(code, locale, ct)
            ?? throw new NotFoundException("Language was not found.");
    }
}
