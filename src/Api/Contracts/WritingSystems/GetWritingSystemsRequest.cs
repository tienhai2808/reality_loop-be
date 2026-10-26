using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Api.Contracts.WritingSystems;

public sealed record GetWritingSystemsRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
