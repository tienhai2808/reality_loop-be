namespace Application.Languages.Models;

public sealed record LanguagePage(IReadOnlyList<LanguageDto> Items, string? NextCursor);
