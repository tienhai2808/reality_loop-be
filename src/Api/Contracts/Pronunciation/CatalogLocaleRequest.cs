using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Api.Contracts.Pronunciation;

public sealed record CatalogLocaleRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
}
