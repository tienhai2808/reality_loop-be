using System.Net;
using Domain.Common;

namespace Domain.Users;

public sealed class User : Entity
{
    private readonly List<OAuthAccount> _oauthAccounts = [];
    private readonly List<Session> _sessions = [];

    private User()
    {
    }

    private User(
        long id,
        string email,
        string firstName,
        string lastName,
        string? avatarUrl,
        DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
        Status = UserStatus.Active;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public string Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string? AvatarUrl { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<OAuthAccount> OAuthAccounts =>
        _oauthAccounts.AsReadOnly();

    public IReadOnlyCollection<Session> Sessions =>
        _sessions.AsReadOnly();

    public static User Create(
        long id,
        string email,
        string firstName,
        string lastName,
        string? avatarUrl,
        DateTimeOffset createdAt)
    {
        return new User(
            id,
            email,
            firstName,
            lastName,
            avatarUrl,
            createdAt);
    }

    public void AddOAuthAccount(
        long id,
        OAuthProvider provider,
        string providerAccountId,
        DateTimeOffset createdAt)
    {
        var account = new OAuthAccount(
            id,
            Id,
            provider,
            providerAccountId,
            createdAt);

        _oauthAccounts.Add(account);

        UpdatedAt = createdAt;
    }

    public Session AddSession(
        long id,
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
        var session = new Session(
            id,
            Id,
            deviceType,
            deviceName,
            os,
            browser,
            ipAddress,
            countryCode,
            region,
            city,
            createdAt,
            expiresAt);

        _sessions.Add(session);

        return session;
    }
}
