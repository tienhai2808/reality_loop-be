using Application.Common.Models;
using Application.Languages.Models;

namespace Application.Common.Interfaces;

public interface ILanguageReadRepository
{
    Task<LanguagePage> GetPageAsync(string locale, int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<LanguageDto?> GetByCodeAsync(string languageCode, string locale, CancellationToken ct);
}
