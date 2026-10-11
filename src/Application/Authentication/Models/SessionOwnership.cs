namespace Application.Authentication.Models;

public sealed record SessionOwnership(long Id, DateTimeOffset ExpiresAt, DateTimeOffset? RevokedAt);
