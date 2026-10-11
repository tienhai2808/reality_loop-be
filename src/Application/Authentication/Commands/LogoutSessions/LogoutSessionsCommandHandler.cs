using System.Globalization;
using Application.Authentication.Models;
using Application.Authentication.Services;
using Application.Common.Exceptions;

namespace Application.Authentication.Commands.LogoutSessions;

public sealed class LogoutSessionsCommandHandler(SessionLogoutService logout)
{
    public Task Handle(LogoutSessionsCommand command, CancellationToken ct)
    {
        if (command.SessionIds is null || command.SessionIds.Count is < 1 or > 100)
            throw new RequestValidationException("Select between 1 and 100 sessions.");
        var ids = new HashSet<long>();
        foreach (var value in command.SessionIds)
        {
            if (!long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
                throw new RequestValidationException("Session IDs must be positive integer strings.");
            ids.Add(id);
        }
        return logout.LogoutAsync(SessionRevocationScope.Selected, ids.Order().ToArray(), ct);
    }
}
