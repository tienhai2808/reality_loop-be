using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Api.Contracts.Languages;

public sealed record CatalogLocaleRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
}
