namespace Application.Handwriting.Queries.GetHandwritingAttempts;

public sealed record GetHandwritingAttemptsQuery(long ProfileId, string? SystemCode = null, bool? Passed = null,
    int PageSize = 20, string? Before = null);
