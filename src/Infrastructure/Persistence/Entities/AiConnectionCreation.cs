namespace Infrastructure.Persistence.Entities;

// Durable request receipt. A completed response survives deletion of its connection until expiry.
public sealed class AiConnectionCreation
{
    private AiConnectionCreation() { }
    public long UserId { get; private set; }
    public Guid RequestId { get; private set; }
    public string Fingerprint { get; private set; } = null!;
    public Guid? LeaseToken { get; private set; }
    public DateTimeOffset? LeaseExpiresAt { get; private set; }
    public string? ResponseJson { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
}
