using System.Net;
using Domain.Common;

namespace Domain.Users;

public sealed class Session : Entity
{
    public long UserId { get; private set; }

    public SessionDeviceType DeviceType { get; private set; }

    public string? DeviceName { get; private set; }

    public string? Os { get; private set; }

    public string? Browser { get; private set; }

    public IPAddress? IpAddress { get; private set; }

    public string? CountryCode { get; private set; }

    public string? Region { get; private set; }

    public string? City { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset LastSeenAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens.AsReadOnly();

    private Session()
    {
    }

    internal Session(
        long id,
        long userId,
        SessionDeviceType deviceType,
        string? deviceName,
        string? os,
        string? browser,
        IPAddress? ipAddress,
        string? countryCode,
        string? region,
        string? city,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        if (expiresAt <= createdAt)
            throw new ArgumentException(
                "Expiration must be after creation.");

        Id = id;
        UserId = userId;
        DeviceType = deviceType;
        DeviceName = deviceName;
        Os = os;
        Browser = browser;
        IpAddress = ipAddress;
        CountryCode = countryCode;
        Region = region;
        City = city;
        CreatedAt = createdAt;
        LastSeenAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return RevokedAt is null && ExpiresAt > now;
    }

    public void Touch(DateTimeOffset now)
    {
        if (!IsActive(now))
            throw new InvalidOperationException(
                "Session is not active.");

        LastSeenAt = now;
    }

    public RefreshToken AddInitialRefreshToken(
        long tokenId,
        string tokenHash,
        DateTimeOffset now,
        DateTimeOffset expiresAt)
    {
        if (!IsActive(now))
            throw new InvalidOperationException(
                "Session is not active.");

        if (_refreshTokens.Count != 0)
            throw new InvalidOperationException(
                "Initial refresh token already exists.");

        if (expiresAt > ExpiresAt)
            throw new InvalidOperationException(
                "Refresh token cannot outlive session.");

        var token = new RefreshToken(
            tokenId,
            Id,
            tokenHash,
            null,
            now,
            expiresAt);

        _refreshTokens.Add(token);

        return token;
    }

    public RefreshToken RotateRefreshToken(
        long currentTokenId,
        long newTokenId,
        string newTokenHash,
        DateTimeOffset now,
        DateTimeOffset expiresAt)
    {
        if (!IsActive(now))
            throw new InvalidOperationException(
                "Session is not active.");

        var current = _refreshTokens.SingleOrDefault(x => x.Id == currentTokenId);

        if (current is null)
            throw new InvalidOperationException(
                "Refresh token does not belong to session.");

        if (!current.IsActive(now))
            throw new InvalidOperationException(
                "Refresh token is not active.");

        if (_refreshTokens.Any(x => x.IsActive(now) &&
                                    x.Id != currentTokenId))
            throw new InvalidOperationException(
                "Another active refresh token exists.");

        if (expiresAt > ExpiresAt)
            throw new InvalidOperationException(
                "Refresh token cannot outlive session.");

        var next = new RefreshToken(
            newTokenId,
            Id,
            newTokenHash,
            current.Id,
            now,
            expiresAt);

        current.Consume(now);
        _refreshTokens.Add(next);

        LastSeenAt = now;

        return next;
    }

    public void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null)
            return;

        RevokedAt = now;

        foreach (var token in _refreshTokens)
            token.Revoke(now);
    }
}