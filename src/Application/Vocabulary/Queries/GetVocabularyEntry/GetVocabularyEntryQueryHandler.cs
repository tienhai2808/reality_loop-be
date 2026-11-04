using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Vocabulary.Models;

namespace Application.Vocabulary.Queries.GetVocabularyEntry;

public sealed class GetVocabularyEntryQueryHandler(IVocabularyReadRepository repository)
{
    public async Task<VocabularyEntryDto> Handle(GetVocabularyEntryQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.EntryCode);
        return await repository.GetByCodeAsync(language, code, ct)
            ?? throw new NotFoundException("Vocabulary entry was not found.");
    }
}
