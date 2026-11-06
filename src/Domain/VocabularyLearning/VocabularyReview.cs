using Domain.Common;

namespace Domain.VocabularyLearning;

public sealed class VocabularyReview : Entity
{
    private VocabularyReview()
    {
    }

    public long ProgressId { get; private init; }
    public Guid RequestId { get; private init; }
    public long ReviewNumber { get; private init; }
    public VocabularyReviewRating Rating { get; private init; }
    public VocabularyLearningStatus PreviousStatus { get; private init; }
    public VocabularyLearningStatus Status { get; private init; }
    public DateTimeOffset PreviousDueAt { get; private init; }
    public DateTimeOffset NextReviewAt { get; private init; }
    public int PreviousIntervalMinutes { get; private init; }
    public int IntervalMinutes { get; private init; }
    public long AgainCount { get; private init; }
    public long ConsecutiveRecallCount { get; private init; }
    public long ProgressVersionBefore { get; private init; }
    public long ProgressVersionAfter { get; private init; }
    public string SchedulerVersion { get; private init; } = null!;
    public DateTimeOffset ReviewedAt { get; private init; }
    public long Version { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private init; }

    internal static VocabularyReview Create(long id, VocabularyLearningProgress progress, Guid requestId,
        VocabularyReviewRating rating, VocabularyReviewSchedule schedule, DateTimeOffset nextDue,
        long reviewNumber, long againCount, long recallCount, long nextVersion, DateTimeOffset timestamp) => new()
    {
        Id = id,
        ProgressId = progress.Id,
        RequestId = requestId,
        ReviewNumber = reviewNumber,
        Rating = rating,
        PreviousStatus = progress.Status,
        Status = schedule.Status,
        PreviousDueAt = progress.NextReviewAt,
        NextReviewAt = nextDue,
        PreviousIntervalMinutes = progress.IntervalMinutes,
        IntervalMinutes = schedule.IntervalMinutes,
        AgainCount = againCount,
        ConsecutiveRecallCount = recallCount,
        ProgressVersionBefore = progress.Version,
        ProgressVersionAfter = nextVersion,
        SchedulerVersion = schedule.SchedulerVersion,
        ReviewedAt = timestamp,
        Version = 1,
        CreatedAt = timestamp,
        UpdatedAt = timestamp
    };
}
