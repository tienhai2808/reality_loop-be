using Application.Authentication.Models;
using Application.Authentication.Services;

namespace Application.Authentication.Commands.LogoutOtherSessions;

public sealed class LogoutOtherSessionsCommandHandler(SessionLogoutService logout)
{
    public Task Handle(LogoutOtherSessionsCommand command, CancellationToken ct) => logout.LogoutAsync(SessionRevocationScope.Others, [], ct);
}
