using System.ComponentModel.DataAnnotations;
using Domain.AI;

namespace Api.Contracts.AI;

public sealed record GetAiConnectionsRequest
{
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    [StringLength(64)] public string? After { get; init; }
}
