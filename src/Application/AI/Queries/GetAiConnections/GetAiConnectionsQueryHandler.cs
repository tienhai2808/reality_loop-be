using System.Globalization;
using Application.AI.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.AI.Queries.GetAiConnections;

public sealed class GetAiConnectionsQueryHandler(ICurrentUser current, IAiConnectionRepository connections)
{
    public Task<AiConnectionPage> Handle(GetAiConnectionsQuery query, CancellationToken ct)
    {
        if (query.PageSize is < 1 or > 100) throw new RequestValidationException("Page size must be between 1 and 100.");
        AiConnectionCursor? cursor = null;
        if (query.After is not null)
        {
            var parts = query.After.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var order) ||
                !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
                throw new RequestValidationException("Invalid AI connection cursor.");
            cursor = new AiConnectionCursor(order, id);
        }
        return connections.GetPageAsync(current.UserId, query.PageSize, cursor, ct);
    }
}
