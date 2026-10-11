using Application.Common.Interfaces;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public Task<User?> FindByOAuthAccountForShareAsync(OAuthProvider provider, string accountId, CancellationToken ct)
    {
        EnsureTransaction();
        var providerName = provider.ToString();
        return db.Users.FromSqlInterpolated($"""
            SELECT u.* FROM users AS u
            WHERE u.id = (SELECT user_id FROM oauth_accounts WHERE provider = {providerName} AND provider_account_id = {accountId})
            FOR SHARE OF u
            """).SingleOrDefaultAsync(ct);
    }

    public Task<User?> FindByRefreshTokenForShareAsync(string hash, CancellationToken ct)
    {
        EnsureTransaction();
        return db.Users.FromSqlInterpolated($"""
            SELECT u.* FROM users AS u
            WHERE u.id = (
                SELECT s.user_id FROM sessions AS s
                JOIN refresh_tokens AS t ON t.session_id = s.id WHERE t.token_hash = {hash})
            FOR SHARE OF u
            """).SingleOrDefaultAsync(ct);
    }

    public Task<User?> FindForShareAsync(long id, CancellationToken ct)
    {
        EnsureTransaction();
        return db.Users.FromSqlInterpolated($"SELECT u.* FROM users AS u WHERE u.id = {id} FOR SHARE OF u")
            .SingleOrDefaultAsync(ct);
    }

    public Task<User?> FindForUpdateAsync(long id, CancellationToken ct)
    {
        EnsureTransaction();
        return db.Users.FromSqlInterpolated($"SELECT u.* FROM users AS u WHERE u.id = {id} FOR UPDATE OF u")
            .SingleOrDefaultAsync(ct);
    }

    public void Add(User user) => db.Users.Add(user);

    private void EnsureTransaction()
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("User locking requires a transaction.");
    }
}
