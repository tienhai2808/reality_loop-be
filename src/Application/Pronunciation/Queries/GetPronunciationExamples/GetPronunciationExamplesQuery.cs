namespace Application.Pronunciation.Queries.GetPronunciationExamples;

public sealed record GetPronunciationExamplesQuery(string LanguageCode, string RuleCode,
    int PageSize = 20, string? After = null);
