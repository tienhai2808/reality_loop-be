using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(ISessionRepository sessions, IUserRepository users,
    IUnitOfWork unitOfWork, ITokenService tokens, IIdGenerator ids, IAuthenticationPolicy policy, TimeProvider clock)
{
    public async Task<AuthenticationResult> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken) || command.RefreshToken.Length > 512)
            throw new RequestValidationException("A refresh token is required.");
        var hash = tokens.HashRefreshToken(command.RefreshToken);
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        var user = await users.FindByRefreshTokenForShareAsync(hash, ct)
            ?? throw new AuthenticationException("Invalid refresh token.");
        var session = await sessions.FindByRefreshTokenForUpdateAsync(hash, user.Id, ct)
            ?? throw new AuthenticationException("Invalid session.");
        var token = await sessions.LoadRefreshTokenAsync(session, hash, ct)
            ?? throw new AuthenticationException("Invalid refresh token.");
        var now = clock.GetUtcNow();
        if (token.ConsumedAt is not null)
        {
            await sessions.RevokeAsync(user.Id, session.Id, SessionRevocationScope.Current, [], now, ct);
            await transaction.CommitAsync(ct);
            throw new AuthenticationException("Refresh token reuse detected. Sign in again.");
        }
        if (!session.IsActive(now) || !token.IsActive(now))
            throw new AuthenticationException("The refresh token has expired or been revoked.");
        if (user.Status != UserStatus.Active)
        {
            await sessions.RevokeAsync(user.Id, session.Id, SessionRevocationScope.Current, [], now, ct);
            await transaction.CommitAsync(ct);
            throw new ForbiddenException("This account is not active.");
        }

        var refresh = tokens.CreateRefreshToken();
        var expiresAt = now.Add(policy.RefreshTokenLifetime);
        if (expiresAt > session.ExpiresAt) expiresAt = session.ExpiresAt;
        var next = session.RotateRefreshToken(token.Id, ids.NewId(), refresh.Hash, now, expiresAt);
        var access = tokens.CreateAccessToken(user, session, now);
        await sessions.PersistRotationAsync(session, token, next, now, ct);
        await transaction.CommitAsync(ct);
        return AuthenticationResult.Create(access, refresh, expiresAt, session, user);
    }
}
