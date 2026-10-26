using Application.Common.Models;
using Application.Pronunciation.Models;

namespace Application.Common.Interfaces;

public interface IPronunciationReadRepository
{
    Task<PronunciationRulePage> GetPageAsync(string languageCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<PronunciationRuleDto?> GetByCodeAsync(string languageCode, string ruleCode, string locale, CancellationToken ct);
    Task<PronunciationExamplePage> GetExamplesPageAsync(string languageCode, string ruleCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
}
