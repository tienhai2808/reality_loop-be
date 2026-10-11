using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SessionRepository(ApplicationDbContext db) : ISessionRepository
{
    public Task<Session?> FindByRefreshTokenForUpdateAsync(string hash, long userId, CancellationToken ct)
    {
        EnsureTransaction();
        return db.Sessions.FromSqlInterpolated($"""
            SELECT s.* FROM sessions AS s
            WHERE s.user_id = {userId} AND s.id = (SELECT session_id FROM refresh_tokens WHERE token_hash = {hash})
            FOR UPDATE OF s
            """).SingleOrDefaultAsync(ct);
    }

    public Task<Session?> FindByIdForUpdateAsync(long sessionId, long userId, CancellationToken ct)
    {
        EnsureTransaction();
        return db.Sessions.FromSqlInterpolated($"""
            SELECT s.* FROM sessions AS s WHERE s.id = {sessionId} AND s.user_id = {userId} FOR UPDATE OF s
            """).SingleOrDefaultAsync(ct);
    }

    public Task<RefreshToken?> LoadRefreshTokenAsync(Session session, string hash, CancellationToken ct)
    {
        EnsureTransaction();
        // A separate statement AFTER the session lock gets a fresh READ COMMITTED snapshot.
        // EF relationship fixup populates only this token in the tracked session collection.
        // The partial unique index enforces one unconsumed/unrevoked token per session.
        return db.RefreshTokens.SingleOrDefaultAsync(x => x.SessionId == session.Id && x.TokenHash == hash, ct);
    }

    public async Task PersistRotationAsync(Session session, RefreshToken current, RefreshToken next, DateTimeOffset now, CancellationToken ct)
    {
        EnsureTransaction();
        if (current.SessionId != session.Id || next.SessionId != session.Id || next.ParentTokenId != current.Id)
            throw new InvalidOperationException("Rotation tokens must belong to the locked session.");
        // RETURNING dependencies consume the old token BEFORE inserting into the partial unique index.
        // All three mutations share one SQL command and the surrounding transaction.
        var changed = await db.Database.ExecuteSqlInterpolatedAsync($"""
            WITH consumed AS (
                UPDATE refresh_tokens AS t SET consumed_at = {now}
                WHERE t.id = {current.Id} AND t.session_id = {session.Id}
                    AND t.consumed_at IS NULL AND t.revoked_at IS NULL AND t.expires_at > {now}
                RETURNING t.id, t.session_id
            ), inserted AS (
                INSERT INTO refresh_tokens (id, session_id, token_hash, parent_token_id, created_at, expires_at)
                SELECT {next.Id}, c.session_id, {next.TokenHash}, c.id, {next.CreatedAt}, {next.ExpiresAt}
                FROM consumed AS c RETURNING session_id
            )
            UPDATE sessions AS s SET last_seen_at = {now}
            FROM inserted AS i WHERE s.id = i.session_id
            """, ct);
        if (changed != 1) throw new AuthenticationException("Refresh token is no longer active.");
        // Raw SQL bypasses EF tracking. Synchronize exactly the entities this operation persisted.
        db.Entry(current).State = EntityState.Unchanged;
        db.Entry(next).State = EntityState.Unchanged;
        db.Entry(session).State = EntityState.Unchanged;
    }

    public Task<UserDto?> GetAuthenticatedUserAsync(long sessionId, long userId, DateTimeOffset now, CancellationToken ct) =>
        (from session in db.Sessions.AsNoTracking()
         join user in db.Users.AsNoTracking() on session.UserId equals user.Id
         where session.Id == sessionId && session.UserId == userId && session.RevokedAt == null &&
             session.ExpiresAt > now && user.Status == UserStatus.Active
         select new UserDto(user.Id.ToString(), user.Email, user.FirstName, user.LastName,
             user.AvatarUrl, user.Status.ToString(), user.CreatedAt)).SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SessionOwnership>> GetOwnershipAsync(long userId, long callerId, long[] selectedIds, CancellationToken ct) =>
        await db.Sessions.AsNoTracking().Where(x => x.UserId == userId && (x.Id == callerId || selectedIds.Contains(x.Id)))
            .Select(x => new SessionOwnership(x.Id, x.ExpiresAt, x.RevokedAt)).ToListAsync(ct);

    public async Task<SessionPage> GetPageAsync(long userId, long callerId, long? beforeId, int pageSize, DateTimeOffset now, CancellationToken ct)
    {
        var query = db.Sessions.AsNoTracking().Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now);
        if (beforeId is { } cursor) query = query.Where(x => x.Id < cursor);
        var rows = await query.OrderByDescending(x => x.Id).Take(pageSize + 1)
            .Select(x => new SessionDto(x.Id.ToString(), x.DeviceType.ToString(), x.DeviceName,
                x.Os, x.Browser, x.IpAddress == null ? null : x.IpAddress.ToString(), x.CreatedAt,
                x.LastSeenAt, x.ExpiresAt, x.Id == callerId)).ToListAsync(ct);
        var hasMore = rows.Count > pageSize;
        if (hasMore) rows.RemoveAt(pageSize);
        return new SessionPage(rows, hasMore ? rows[^1].Id : null);
    }

    public Task RevokeAsync(long userId, long callerId, SessionRevocationScope scope, long[] selectedIds, DateTimeOffset now, CancellationToken ct)
    {
        EnsureTransaction();
        // Only these fixed predicates are composed into SQL; every request value is a bound parameter.
        var predicate = scope switch
        {
            SessionRevocationScope.Current => "s.id = @caller_id",
            SessionRevocationScope.Selected => "s.id = ANY(@selected_ids)",
            SessionRevocationScope.Others => "s.id <> @caller_id",
            SessionRevocationScope.All => "TRUE",
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };
        var sql = $"""
            WITH revoked AS (
                UPDATE sessions AS s SET revoked_at = @now
                WHERE s.user_id = @user_id AND s.revoked_at IS NULL AND {predicate}
                RETURNING s.id
            )
            UPDATE refresh_tokens AS t SET revoked_at = @now
            FROM revoked AS s WHERE t.session_id = s.id AND t.revoked_at IS NULL
            """;
        object[] parameters =
        [
            new NpgsqlParameter("user_id", NpgsqlDbType.Bigint) { Value = userId },
            new NpgsqlParameter("caller_id", NpgsqlDbType.Bigint) { Value = callerId },
            new NpgsqlParameter("selected_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint) { Value = selectedIds },
            new NpgsqlParameter("now", NpgsqlDbType.TimestampTz) { Value = now }
        ];
        return db.Database.ExecuteSqlRawAsync(sql, parameters, ct);
    }

    public void Add(Session session) => db.Sessions.Add(session);

    private void EnsureTransaction()
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Session mutation requires a transaction.");
    }
}
