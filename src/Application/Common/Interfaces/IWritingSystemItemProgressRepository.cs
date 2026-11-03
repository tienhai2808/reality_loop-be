using Application.Common.Models;
using Application.LearningProgress.Models;
using Domain.LearningProgress;

namespace Application.Common.Interfaces;

public interface IWritingSystemItemProgressRepository
{
    Task<LearningProgressSummaryDto> GetSummaryAsync(long userId, long profileId, CancellationToken ct);
    Task<WritingSystemItemProgressPage> GetPageAsync(long userId, long profileId, string systemCode,
        WritingSystemItemLearningState? status, int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<WritingSystemItemProgressDto?> GetItemAsync(long userId, long profileId, string systemCode,
        long itemId, CancellationToken ct);
    Task<WritingSystemItemProgressContext?> FindContextAsync(long userId, long profileId, string systemCode,
        long itemId, CancellationToken ct);
    Task SaveAsync(long userId, string systemCode, WritingSystemItemProgress progress, long expectedVersion, CancellationToken ct);
}
