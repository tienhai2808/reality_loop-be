using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Services;

public sealed class SessionLogoutService(ICurrentUser currentUser, IUserRepository users,
    ISessionRepository sessions, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task LogoutAsync(SessionRevocationScope scope, long[] selectedIds, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var callerId = currentUser.SessionId;
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        // Independent devices may refresh/logout concurrently. Multi-session operations take the exclusive lock.
        var user = scope == SessionRevocationScope.Current
            ? await users.FindForShareAsync(userId, ct)
            : await users.FindForUpdateAsync(userId, ct);
        if (user is null || user.Status != UserStatus.Active)
            throw new AuthenticationException("The authenticated account is no longer active.");

        var now = clock.GetUtcNow();
        if (scope == SessionRevocationScope.Current)
        {
            var caller = await sessions.FindByIdForUpdateAsync(callerId, userId, ct);
            if (caller is null || !caller.IsActive(clock.GetUtcNow()))
                throw new AuthenticationException("The authenticated session is no longer active.");
            now = clock.GetUtcNow();
        }
        else
        {
            // One projected read validates the caller and every selected target AFTER acquiring the user lock.
            var owned = await sessions.GetOwnershipAsync(userId, callerId, selectedIds, ct);
            var caller = owned.SingleOrDefault(x => x.Id == callerId);
            now = clock.GetUtcNow();
            if (caller is null || caller.RevokedAt is not null || caller.ExpiresAt <= now)
                throw new AuthenticationException("The authenticated session is no longer active.");
            if (scope == SessionRevocationScope.Selected)
            {
                var ownedIds = owned.Select(x => x.Id).ToHashSet();
                if (selectedIds.Any(x => !ownedIds.Contains(x)))
                    throw new NotFoundException("One or more sessions were not found.");
            }
        }
        await sessions.RevokeAsync(userId, callerId, scope, selectedIds, now, ct);
        await transaction.CommitAsync(ct);
    }
}
