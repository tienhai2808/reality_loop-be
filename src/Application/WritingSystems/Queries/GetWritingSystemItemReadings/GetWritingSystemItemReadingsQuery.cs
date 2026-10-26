namespace Application.WritingSystems.Queries.GetWritingSystemItemReadings;

public sealed record GetWritingSystemItemReadingsQuery(string LanguageCode, string SystemCode, string ItemCode, int PageSize = 20, string? After = null);
