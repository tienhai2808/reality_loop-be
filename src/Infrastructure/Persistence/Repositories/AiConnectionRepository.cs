using System.Globalization;
using System.Text.Json;
using Application.AI.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.AI;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class AiConnectionRepository(ApplicationDbContext db) : IAiConnectionRepository
{
    private static readonly JsonSerializerOptions ReceiptJson = new(JsonSerializerDefaults.Web);

    public Task<AiConnection?> FindOwnedAsync(long userId, long id, CancellationToken ct) =>
        db.AiConnections.SingleOrDefaultAsync(x => x.UserId == userId && x.Id == id, ct);

    public async Task<AiConnectionPage> GetPageAsync(long userId, int pageSize, AiConnectionCursor? cursor, CancellationToken ct)
    {
        var query = db.AiConnections.AsNoTracking().Where(x => x.UserId == userId);
        if (cursor is not null) query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        // Ciphertext is deliberately absent from this SQL projection.
        var items = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new AiConnectionDto(
            x.Id.ToString(), x.Provider, x.DisplayName, x.ApiKeyLast4, x.ModelId, x.SortOrder, x.IsEnabled,
            x.Status, x.RetryAfterAt, x.LastFailureCode, x.Version, x.CreatedAt, x.UpdatedAt)).Take(pageSize + 1).ToListAsync(ct);
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        var next = hasNext ? FormattableString.Invariant($"{items[^1].SortOrder}:{items[^1].Id}") : null;
        return new AiConnectionPage(items, next);
    }

    public async Task<AiCreationReservation> ReserveCreationAsync(long userId, Guid requestId, string fingerprint,
        Guid leaseToken, DateTimeOffset now, CancellationToken ct)
    {
        var leaseExpiry = now.AddMinutes(2);
        var expiry = now.AddHours(24);
        var acquired = await db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO ai_connection_creations (user_id, request_id, fingerprint, lease_token, lease_expires_at, expires_at)
            VALUES ({userId}, {requestId}, {fingerprint}, {leaseToken}, {leaseExpiry}, {expiry})
            ON CONFLICT (user_id, request_id) DO UPDATE SET
                fingerprint = EXCLUDED.fingerprint, lease_token = EXCLUDED.lease_token,
                lease_expires_at = EXCLUDED.lease_expires_at, expires_at = EXCLUDED.expires_at, response_json = NULL
            WHERE ai_connection_creations.expires_at <= {now}
                OR (ai_connection_creations.response_json IS NULL AND ai_connection_creations.fingerprint = EXCLUDED.fingerprint
                    AND ai_connection_creations.lease_expires_at <= {now})
            """, ct);
        if (acquired == 1) return new AiCreationReservation(true, null);
        var existing = await db.AiConnectionCreations.AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.RequestId == requestId, ct);
        if (existing is null) throw new ConflictException("The creation reservation changed. Retry with the same Idempotency-Key.");
        if (existing.Fingerprint != fingerprint) throw new ConflictException("This Idempotency-Key was used with different connection settings.");
        AiConnectionDto? response = null;
        if (existing.ResponseJson is { } json)
            response = JsonSerializer.Deserialize<AiConnectionDto>(json, ReceiptJson)
                ?? throw new InvalidOperationException("The AI creation receipt is invalid.");
        return new AiCreationReservation(false, response);
    }

    public async Task CompleteCreationAsync(AiConnection connection, Guid requestId, Guid leaseToken, DateTimeOffset now, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        db.AiConnections.Add(connection);
        await db.SaveChangesAsync(ct);
        var json = JsonSerializer.Serialize(AiConnectionDto.From(connection), ReceiptJson);
        var updated = await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE ai_connection_creations SET response_json = {json}::jsonb, lease_token = NULL, lease_expires_at = NULL
            WHERE user_id = {connection.UserId} AND request_id = {requestId} AND lease_token = {leaseToken}
                AND response_json IS NULL AND lease_expires_at > {now}
            """, ct);
        if (updated != 1) throw new ConflictException("The connection creation lease expired. Retry the same request.");
        await transaction.CommitAsync(ct);
    }

    public Task ReleaseCreationAsync(long userId, Guid requestId, Guid leaseToken, CancellationToken ct) =>
        db.AiConnectionCreations.Where(x => x.UserId == userId && x.RequestId == requestId &&
            x.LeaseToken == leaseToken && x.ResponseJson == null).ExecuteDeleteAsync(ct);

    public async Task SaveAsync(AiConnection connection, CancellationToken ct)
    {
        // Even a no-op PATCH checks the original version against concurrent changes.
        if (db.Entry(connection).State == EntityState.Unchanged)
            db.Entry(connection).Property(x => x.Version).IsModified = true;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("The AI connection changed. Reload it before retrying."); }
    }

    public async Task DeleteAsync(long userId, long id, long version, CancellationToken ct)
    {
        var count = await db.AiConnections.Where(x => x.UserId == userId && x.Id == id && x.Version == version).ExecuteDeleteAsync(ct);
        if (count == 1) return;
        if (!await db.AiConnections.AnyAsync(x => x.UserId == userId && x.Id == id, ct))
            throw new NotFoundException("AI connection was not found.");
        throw new ConflictException("The AI connection changed. Reload it before deleting.");
    }
}
