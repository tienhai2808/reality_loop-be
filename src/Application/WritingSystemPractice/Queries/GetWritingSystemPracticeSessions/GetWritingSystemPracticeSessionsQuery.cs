using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Queries.GetWritingSystemPracticeSessions;

public sealed record GetWritingSystemPracticeSessionsQuery(long ProfileId, string? SystemCode = null,
    WritingSystemPracticeSessionStatus? Status = null, int PageSize = 20, string? Before = null);
