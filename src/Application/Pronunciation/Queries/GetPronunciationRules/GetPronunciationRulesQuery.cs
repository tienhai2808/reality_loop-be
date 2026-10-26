namespace Application.Pronunciation.Queries.GetPronunciationRules;

public sealed record GetPronunciationRulesQuery(string LanguageCode, string? Locale = null,
    int PageSize = 20, string? After = null);
