using Domain.Vocabulary;

namespace Application.Vocabulary.Queries.GetVocabularyEntries;

public sealed record GetVocabularyEntriesQuery(string LanguageCode, string? Locale = null,
    PartOfSpeech? PartOfSpeech = null, int PageSize = 20, string? After = null);
