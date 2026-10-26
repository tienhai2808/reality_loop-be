namespace Application.Languages.Queries.GetLanguages;

public sealed record GetLanguagesQuery(string? Locale = null, int PageSize = 20, string? After = null);
