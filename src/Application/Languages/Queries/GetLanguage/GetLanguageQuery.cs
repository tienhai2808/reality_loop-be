namespace Application.Languages.Queries.GetLanguage;

public sealed record GetLanguageQuery(string LanguageCode, string? Locale = null);
