using Application.Authentication.Models;
using Domain.Users;

namespace Application.Common.Interfaces;

public interface ISessionRepository
{
    Task<Session?> FindByRefreshTokenForUpdateAsync(string hash, long userId, CancellationToken ct);
    Task<Session?> FindByIdForUpdateAsync(long sessionId, long userId, CancellationToken ct);
    Task<RefreshToken?> LoadRefreshTokenAsync(Session session, string hash, CancellationToken ct);
    Task PersistRotationAsync(Session session, RefreshToken current, RefreshToken next, DateTimeOffset now, CancellationToken ct);
    Task<UserDto?> GetAuthenticatedUserAsync(long sessionId, long userId, DateTimeOffset now, CancellationToken ct);
    Task<IReadOnlyList<SessionOwnership>> GetOwnershipAsync(long userId, long callerId, long[] selectedIds, CancellationToken ct);
    Task<SessionPage> GetPageAsync(long userId, long callerId, long? beforeId, int pageSize, DateTimeOffset now, CancellationToken ct);
    Task RevokeAsync(long userId, long callerId, SessionRevocationScope scope, long[] selectedIds, DateTimeOffset now, CancellationToken ct);
    void Add(Session session);
}
