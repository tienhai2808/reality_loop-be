using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Api.Contracts.Grammar;

public sealed record GetGrammarPointsRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
