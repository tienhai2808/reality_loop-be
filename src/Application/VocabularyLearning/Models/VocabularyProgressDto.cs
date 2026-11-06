using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyProgressDto(string Id, string LearningProfileId, string VocabularySenseId,
    VocabularyLearningStatus Status, DateTimeOffset NextReviewAt, DateTimeOffset? LastReviewedAt, int IntervalMinutes,
    long ReviewCount, long AgainCount, long ConsecutiveRecallCount, string? SchedulerVersion, long Version,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static VocabularyProgressDto From(VocabularyLearningProgress p) => new(p.Id.ToString(), p.LearningProfileId.ToString(),
        p.VocabularySenseId.ToString(), p.Status, p.NextReviewAt, p.LastReviewedAt, p.IntervalMinutes, p.ReviewCount,
        p.AgainCount, p.ConsecutiveRecallCount, p.SchedulerVersion, p.Version, p.CreatedAt, p.UpdatedAt);
}
