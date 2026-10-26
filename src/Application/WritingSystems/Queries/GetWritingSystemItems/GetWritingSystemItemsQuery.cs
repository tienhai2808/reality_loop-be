namespace Application.WritingSystems.Queries.GetWritingSystemItems;

public sealed record GetWritingSystemItemsQuery(string LanguageCode, string SystemCode, int PageSize = 100, string? After = null);
