namespace Application.WritingSystems.Queries.GetWritingSystem;

public sealed record GetWritingSystemQuery(string LanguageCode, string SystemCode, string? Locale = null);
