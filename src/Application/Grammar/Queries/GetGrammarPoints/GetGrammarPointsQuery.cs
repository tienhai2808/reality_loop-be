namespace Application.Grammar.Queries.GetGrammarPoints;

public sealed record GetGrammarPointsQuery(string LanguageCode, string? Locale = null,
    int PageSize = 20, string? After = null);
