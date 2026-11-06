namespace Application.Grammar.Queries.GetGrammarPoint;

public sealed record GetGrammarPointQuery(string LanguageCode, string PointCode, string? Locale = null);
