using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace Infrastructure.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public async Task<IApplicationTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        // Every failed operation rolls back its entire transaction; no partial retry needs savepoints.
        db.Database.AutoSavepointsEnabled = false;
        return new ApplicationTransaction(await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct));
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("The account or token already exists. Start a new login; account linking requires authentication with the existing provider.");
        }
    }

    private sealed class ApplicationTransaction(IDbContextTransaction transaction) : IApplicationTransaction
    {
        public Task CommitAsync(CancellationToken ct) => transaction.CommitAsync(ct);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
