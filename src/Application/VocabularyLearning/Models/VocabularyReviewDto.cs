using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyReviewDto(string Id, string ProgressId, Guid RequestId, long ReviewNumber,
    VocabularyReviewRating Rating, VocabularyLearningStatus PreviousStatus, VocabularyLearningStatus Status,
    DateTimeOffset PreviousDueAt, DateTimeOffset NextReviewAt, int PreviousIntervalMinutes, int IntervalMinutes,
    long AgainCount, long ConsecutiveRecallCount, long ProgressVersionBefore, long ProgressVersionAfter,
    string SchedulerVersion, DateTimeOffset ReviewedAt)
{
    public static VocabularyReviewDto From(VocabularyReview r) => new(r.Id.ToString(), r.ProgressId.ToString(), r.RequestId,
        r.ReviewNumber, r.Rating, r.PreviousStatus, r.Status, r.PreviousDueAt, r.NextReviewAt, r.PreviousIntervalMinutes,
        r.IntervalMinutes, r.AgainCount, r.ConsecutiveRecallCount, r.ProgressVersionBefore, r.ProgressVersionAfter,
        r.SchedulerVersion, r.ReviewedAt);
}
