namespace Application.WritingSystems.Queries.GetWritingSystems;

public sealed record GetWritingSystemsQuery(string LanguageCode, string? Locale = null, int PageSize = 20, string? After = null);
