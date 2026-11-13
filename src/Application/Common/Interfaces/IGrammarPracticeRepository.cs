using Application.GrammarPractice.Models;
using Domain.GrammarPractice;

namespace Application.Common.Interfaces;

public interface IGrammarPracticeRepository
{
    Task<GrammarPracticeOptionsDto?> GetOptionsAsync(long userId, long profileId, CancellationToken ct);
    Task<GrammarPracticeContext?> GetContextAsync(long userId, long profileId, string? pointCode, CancellationToken ct);
    Task<IReadOnlyList<GrammarPracticeCandidate>> SampleAsync(GrammarPracticeContext context, int count, CancellationToken ct);
    Task CreateAsync(long userId, GrammarPracticeSession session, CancellationToken ct);
    Task<GrammarPracticeSession?> FindOwnedAsync(long userId, long profileId, long sessionId, CancellationToken ct);
    Task<GrammarPracticeSessionPage> GetPageAsync(long userId, long profileId, string? pointCode,
        GrammarPracticeSessionStatus? status, int pageSize, long? before, CancellationToken ct);
    Task SaveAnswerAsync(long userId, GrammarPracticeSession session, GrammarPracticeSessionItem item,
        long expectedVersion, CancellationToken ct);
    Task SaveAbandonAsync(long userId, GrammarPracticeSession session, long expectedVersion, CancellationToken ct);
}
