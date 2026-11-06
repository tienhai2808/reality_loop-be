using Application.Common.Models;
using Application.Grammar.Models;

namespace Application.Common.Interfaces;

public interface IGrammarReadRepository
{
    Task<GrammarPointPage> GetPageAsync(string languageCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<GrammarPointDto?> GetByCodeAsync(string languageCode, string pointCode, string locale, CancellationToken ct);
}
