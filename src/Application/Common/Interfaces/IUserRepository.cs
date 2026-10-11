using Domain.Users;

namespace Application.Common.Interfaces;

public interface IUserRepository
{
    // Acquire the user lock before any session lock. These require a transaction.
    Task<User?> FindByOAuthAccountForShareAsync(OAuthProvider provider, string accountId, CancellationToken ct);
    Task<User?> FindByRefreshTokenForShareAsync(string hash, CancellationToken ct);
    Task<User?> FindForShareAsync(long id, CancellationToken ct);
    Task<User?> FindForUpdateAsync(long id, CancellationToken ct);
    void Add(User user);
}
