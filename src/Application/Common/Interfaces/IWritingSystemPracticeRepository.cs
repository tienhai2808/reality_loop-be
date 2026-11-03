using Application.WritingSystemPractice.Models;
using Domain.WritingSystemPractice;

namespace Application.Common.Interfaces;

public interface IWritingSystemPracticeRepository
{
    Task<WritingSystemPracticeCatalog?> GetCatalogAsync(long userId, long profileId, string systemCode, CancellationToken ct);
    Task CreateAsync(long userId, WritingSystemPracticeSession session, CancellationToken ct);
    Task<WritingSystemPracticeSession?> FindOwnedAsync(long userId, long profileId, long sessionId, CancellationToken ct);
    Task<WritingSystemPracticeSessionPage> GetPageAsync(long userId, long profileId, string? systemCode,
        WritingSystemPracticeSessionStatus? status, int pageSize, long? before, CancellationToken ct);
    Task SaveAnswerAsync(long userId, WritingSystemPracticeSession session, WritingSystemPracticeSessionItem item,
        long expectedVersion, CancellationToken ct);
    Task SaveAbandonAsync(long userId, WritingSystemPracticeSession session, long expectedVersion, CancellationToken ct);
}
