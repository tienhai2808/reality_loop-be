using System.ComponentModel.DataAnnotations;
using Domain.AI;

namespace Api.Contracts.AI;

public sealed record UpdateAiConnectionRequest
{
    [Range(1, long.MaxValue)] public long Version { get; init; }
    [StringLength(150)] public string? DisplayName { get; init; }
    [StringLength(200, MinimumLength = 1)] public string? ModelId { get; init; }
    [StringLength(8192, MinimumLength = 8)] public string? ApiKey { get; init; }
    public bool? IsEnabled { get; init; }
    public override string ToString() => "UpdateAiConnectionRequest (API key redacted)";
}
