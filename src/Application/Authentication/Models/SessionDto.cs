namespace Application.Authentication.Models;

public sealed record SessionDto(string Id, string DeviceType, string? DeviceName, string? Os,
    string? Browser, string? IpAddress, DateTimeOffset CreatedAt, DateTimeOffset LastSeenAt,
    DateTimeOffset ExpiresAt, bool IsCurrent);
