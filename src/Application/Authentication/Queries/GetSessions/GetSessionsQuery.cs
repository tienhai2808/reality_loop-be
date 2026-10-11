namespace Application.Authentication.Queries.GetSessions;

public sealed record GetSessionsQuery(int PageSize = 20, string? Before = null);
