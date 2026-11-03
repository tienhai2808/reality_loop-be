using System.ComponentModel.DataAnnotations;
using Domain.WritingSystemPractice;

namespace Api.Contracts.WritingSystemPractice;

public sealed record GetWritingSystemPracticeSessionsRequest
{
    [StringLength(64, MinimumLength = 1)] public string? SystemCode { get; init; }
    [EnumDataType(typeof(WritingSystemPracticeSessionStatus))] public WritingSystemPracticeSessionStatus? Status { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(19, MinimumLength = 1)] public string? Before { get; init; }
}
