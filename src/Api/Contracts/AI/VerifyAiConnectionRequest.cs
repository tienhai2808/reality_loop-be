using System.ComponentModel.DataAnnotations;
using Domain.AI;

namespace Api.Contracts.AI;

public sealed record VerifyAiConnectionRequest
{
    [Range(1, long.MaxValue)] public long Version { get; init; }
}
