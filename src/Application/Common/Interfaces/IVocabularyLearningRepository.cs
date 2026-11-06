using Application.VocabularyLearning.Models;
using Domain.VocabularyLearning;

namespace Application.Common.Interfaces;

public interface IVocabularyLearningRepository
{
    Task<VocabularyLearningContext?> GetAddContextAsync(long userId, long profileId, long senseId, CancellationToken ct);
    Task<VocabularyProgressCreationResult> CreateAsync(long userId, VocabularyLearningProgress progress, CancellationToken ct);
    Task<VocabularyReviewContext?> GetReviewContextAsync(long userId, long profileId, long progressId, Guid requestId, CancellationToken ct);
    Task<VocabularyReviewDto> SaveReviewAsync(long userId, VocabularyLearningProgress progress, VocabularyReview review, CancellationToken ct);
    Task<VocabularyFlashcardDto?> GetCardAsync(long userId, long profileId, long progressId, string? locale, CancellationToken ct);
    Task<VocabularyLearningPage> GetPageAsync(long userId, long profileId, bool dueOnly, VocabularyLearningStatus? status,
        int pageSize, VocabularyDueCursor? cursor, DateTimeOffset asOf, string? locale, CancellationToken ct);
    Task<VocabularyLearningSummaryDto?> GetSummaryAsync(long userId, long profileId, DateTimeOffset asOf, CancellationToken ct);
    Task<VocabularyReviewPage> GetHistoryAsync(long userId, long profileId, long progressId, int pageSize, long? before, CancellationToken ct);
}
