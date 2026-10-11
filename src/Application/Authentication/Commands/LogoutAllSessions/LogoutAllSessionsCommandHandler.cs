using Application.Authentication.Models;
using Application.Authentication.Services;

namespace Application.Authentication.Commands.LogoutAllSessions;

public sealed class LogoutAllSessionsCommandHandler(SessionLogoutService logout)
{
    public Task Handle(LogoutAllSessionsCommand command, CancellationToken ct) => logout.LogoutAsync(SessionRevocationScope.All, [], ct);
}
