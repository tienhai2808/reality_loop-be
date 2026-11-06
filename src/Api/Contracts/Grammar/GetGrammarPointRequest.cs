using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Api.Contracts.Grammar;

public sealed record GetGrammarPointRequest
{
    [StringLength(LanguageTag.MaxLength, MinimumLength = 1)] public string? Locale { get; init; }
}
