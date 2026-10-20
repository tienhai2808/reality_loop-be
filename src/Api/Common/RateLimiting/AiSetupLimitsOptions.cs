using System.ComponentModel.DataAnnotations;

namespace Api.Common.RateLimiting;

public sealed class AiSetupLimitsOptions
{
    public const string SectionName = "RateLimiting:AiSetup";
    [Range(1, 10000)] public int IpPermitLimit { get; init; } = 120;
    [Range(1, 10000)] public int UserPermitLimit { get; init; } = 60;
    [Range(1, 3600)] public int WindowSeconds { get; init; } = 60;
    [Range(1, 20)] public int ConcurrentRequestsPerUser { get; init; } = 2;
}
