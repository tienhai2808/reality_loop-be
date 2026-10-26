namespace Application.Pronunciation.Queries.GetPronunciationRule;

public sealed record GetPronunciationRuleQuery(string LanguageCode, string RuleCode, string? Locale = null);
