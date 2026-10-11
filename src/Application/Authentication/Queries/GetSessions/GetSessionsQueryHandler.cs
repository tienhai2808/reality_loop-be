using System.Globalization;
using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Authentication.Queries.GetSessions;

public sealed class GetSessionsQueryHandler(ICurrentUser currentUser, ISessionRepository sessions, TimeProvider clock)
{
    public Task<SessionPage> Handle(GetSessionsQuery query, CancellationToken ct)
    {
        if (query.PageSize is < 1 or > 100) throw new RequestValidationException("PageSize must be between 1 and 100.");
        long? before = null;
        if (query.Before is not null)
        {
            if (!long.TryParse(query.Before, NumberStyles.None, CultureInfo.InvariantCulture, out var cursor) || cursor <= 0)
                throw new RequestValidationException("Before must be a positive integer session ID.");
            before = cursor;
        }
        return sessions.GetPageAsync(currentUser.UserId, currentUser.SessionId, before, query.PageSize, clock.GetUtcNow(), ct);
    }
}
