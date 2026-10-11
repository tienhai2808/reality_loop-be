using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Authentication.Commands.RevokeRefreshToken;

public sealed class RevokeRefreshTokenCommandHandler(ISessionRepository sessions, IUserRepository users,
    IUnitOfWork unitOfWork, ITokenService tokens, TimeProvider clock)
{
    public async Task Handle(RevokeRefreshTokenCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken) || command.RefreshToken.Length > 512)
            throw new RequestValidationException("A refresh token is required.");
        var hash = tokens.HashRefreshToken(command.RefreshToken);
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        var user = await users.FindByRefreshTokenForShareAsync(hash, ct);
        if (user is null) return;
        var session = await sessions.FindByRefreshTokenForUpdateAsync(hash, user.Id, ct);
        if (session is null) return;
        await sessions.RevokeAsync(user.Id, session.Id, SessionRevocationScope.Current, [], clock.GetUtcNow(), ct);
        await transaction.CommitAsync(ct);
    }
}
