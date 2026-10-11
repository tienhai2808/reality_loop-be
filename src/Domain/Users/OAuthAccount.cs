using Domain.Common;

namespace Domain.Users;

public sealed class OAuthAccount : Entity
{
    private OAuthAccount()
    {
    } 
    
    internal OAuthAccount(long id, long userId, OAuthProvider provider, string providerAccountId,
        DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Provider = provider;
        ProviderAccountId = providerAccountId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public long UserId { get; private set; }

    public OAuthProvider Provider { get; private set; }

    public string ProviderAccountId { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
}