using Domain.GrammarPractice;

namespace Application.GrammarPractice.Queries.GetGrammarPracticeSessions;

public sealed record GetGrammarPracticeSessionsQuery(long ProfileId, string? GrammarPointCode = null,
    GrammarPracticeSessionStatus? Status = null, int PageSize = 20, string? Before = null);
