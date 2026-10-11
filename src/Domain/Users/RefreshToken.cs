using Domain.Common;

namespace Domain.Users;

public sealed class RefreshToken : Entity
{
    public long SessionId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public long? ParentTokenId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? ConsumedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    private RefreshToken()
    {
    }

    internal RefreshToken(
        long id,
        long sessionId,
        string tokenHash,
        long? parentTokenId,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException(
                "Token hash is required.",
                nameof(tokenHash));

        if (expiresAt <= createdAt)
            throw new ArgumentException(
                "Expiration must be after creation.");

        Id = id;
        SessionId = sessionId;
        TokenHash = tokenHash;
        ParentTokenId = parentTokenId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return ConsumedAt is null
               && RevokedAt is null
               && ExpiresAt > now;
    }

    internal void Consume(DateTimeOffset now)
    {
        if (!IsActive(now))
            throw new InvalidOperationException(
                "Refresh token is not active.");

        ConsumedAt = now;
    }

    internal void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null)
            return;

        RevokedAt = now;
    }
}