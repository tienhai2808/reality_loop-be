using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Services;

public sealed class OAuthLoginService(IUserRepository users, ISessionRepository sessions,
    IUnitOfWork unitOfWork, IIdGenerator ids, ITokenService tokens, IAuthenticationPolicy policy, TimeProvider clock)
{
    // Call only after the provider has verified the identity. External HTTP runs before database locks.
    public async Task<AuthenticationResult> LoginAsync(OAuthProvider provider, OAuthIdentity identity,
        LoginDevice device, CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        var user = await users.FindByOAuthAccountForShareAsync(provider, identity.AccountId, ct);
        var isNewUser = user is null;
        var now = clock.GetUtcNow();
        if (user is null)
        {
            // Unique constraints arbitrate concurrent signups. Email never authorizes account linking.
            user = User.Create(ids.NewId(), identity.Email, identity.FirstName, identity.LastName, identity.AvatarUrl, now);
            user.AddOAuthAccount(ids.NewId(), provider, identity.AccountId, now);
        }
        if (user.Status != UserStatus.Active) throw new ForbiddenException("This account is not active.");

        var session = user.AddSession(ids.NewId(), device.DeviceType, device.DeviceName, device.Os,
            device.Browser, device.IpAddress, null, null, null, now, now.Add(policy.SessionLifetime));
        var refresh = tokens.CreateRefreshToken();
        var expiresAt = now.Add(policy.RefreshTokenLifetime);
        if (expiresAt > session.ExpiresAt) expiresAt = session.ExpiresAt;
        session.AddInitialRefreshToken(ids.NewId(), refresh.Hash, now, expiresAt);
        var access = tokens.CreateAccessToken(user, session, now);
        if (isNewUser) users.Add(user);
        else sessions.Add(session);
        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return AuthenticationResult.Create(access, refresh, expiresAt, session, user);
    }
}
