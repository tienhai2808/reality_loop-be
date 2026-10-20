using Application.AI.Models;
using Domain.AI;

namespace Application.Common.Interfaces;

public interface IAiConnectionRepository
{
    Task<AiConnection?> FindOwnedAsync(long userId, long id, CancellationToken ct);
    Task<AiConnectionPage> GetPageAsync(long userId, int pageSize, AiConnectionCursor? cursor, CancellationToken ct);
    Task<AiCreationReservation> ReserveCreationAsync(long userId, Guid requestId, string fingerprint,
        Guid leaseToken, DateTimeOffset now, CancellationToken ct);
    Task CompleteCreationAsync(AiConnection connection, Guid requestId, Guid leaseToken, DateTimeOffset now, CancellationToken ct);
    Task ReleaseCreationAsync(long userId, Guid requestId, Guid leaseToken, CancellationToken ct);
    Task SaveAsync(AiConnection connection, CancellationToken ct);
    Task DeleteAsync(long userId, long id, long version, CancellationToken ct);
}
