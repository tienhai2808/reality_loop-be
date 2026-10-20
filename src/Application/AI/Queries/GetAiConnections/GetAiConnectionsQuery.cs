namespace Application.AI.Queries.GetAiConnections;

public sealed record GetAiConnectionsQuery(int PageSize = 20, string? After = null);
