using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Vocabulary.Models;

namespace Application.Vocabulary.Queries.GetVocabularyEntries;

public sealed class GetVocabularyEntriesQueryHandler(IVocabularyReadRepository repository)
{
    public Task<VocabularyEntryPage> Handle(GetVocabularyEntriesQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        if (query.PartOfSpeech is { } partOfSpeech && !Enum.IsDefined(partOfSpeech))
            throw new RequestValidationException("Unknown part of speech.");
        return repository.GetPageAsync(language, locale, query.PartOfSpeech, query.PageSize, cursor, ct);
    }
}
