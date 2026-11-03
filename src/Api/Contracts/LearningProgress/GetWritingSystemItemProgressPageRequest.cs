using System.ComponentModel.DataAnnotations;
using Application.LearningProgress.Models;

namespace Api.Contracts.LearningProgress;

public sealed record GetWritingSystemItemProgressPageRequest
{
    [EnumDataType(typeof(WritingSystemItemLearningState))] public WritingSystemItemLearningState? Status { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 100;
    [StringLength(30, MinimumLength = 1)] public string? After { get; init; }
}
