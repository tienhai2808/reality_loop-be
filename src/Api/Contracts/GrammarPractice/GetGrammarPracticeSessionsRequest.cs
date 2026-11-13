using System.ComponentModel.DataAnnotations;
using Domain.GrammarPractice;

namespace Api.Contracts.GrammarPractice;

public sealed record GetGrammarPracticeSessionsRequest
{
    [StringLength(64, MinimumLength = 1)] public string? GrammarPointCode { get; init; }
    [EnumDataType(typeof(GrammarPracticeSessionStatus))] public GrammarPracticeSessionStatus? Status { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19, MinimumLength = 1)] public string? Before { get; init; }
}
