namespace Application.Vocabulary.Queries.GetVocabularySenses;

public sealed record GetVocabularySensesQuery(string LanguageCode, string EntryCode, string? Locale = null,
    int PageSize = 20, string? After = null);
