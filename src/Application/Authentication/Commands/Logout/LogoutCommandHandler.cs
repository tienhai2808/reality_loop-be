using Application.Authentication.Models;
using Application.Authentication.Services;

namespace Application.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(SessionLogoutService logout)
{
    public Task Handle(LogoutCommand command, CancellationToken ct) => logout.LogoutAsync(SessionRevocationScope.Current, [], ct);
}
