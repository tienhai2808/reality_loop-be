using Application.Common.Models;
using Application.Vocabulary.Models;
using Domain.Vocabulary;

namespace Application.Common.Interfaces;

public interface IVocabularyReadRepository
{
    Task<VocabularyEntryPage> GetPageAsync(string languageCode, string locale, PartOfSpeech? partOfSpeech,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<VocabularyEntryDto?> GetByCodeAsync(string languageCode, string entryCode, CancellationToken ct);
    Task<VocabularySensePage> GetSensesPageAsync(string languageCode, string entryCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
}
