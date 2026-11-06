using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Services;

// Deterministic initial policy, not SM-2/FSRS and not a claim of optimized recall probability.
public sealed class VocabularyReviewScheduler
{
    public const string Version = "simple-v1";
    public const int MaxIntervalMinutes = 525600;

    public VocabularyReviewSchedule Schedule(VocabularyLearningProgress progress, VocabularyReviewRating rating)
    {
        ArgumentNullException.ThrowIfNull(progress);
        if (!Enum.IsDefined(rating)) throw new ArgumentOutOfRangeException(nameof(rating));
        long interval = rating switch
        {
            VocabularyReviewRating.Again => 1,
            VocabularyReviewRating.Hard => progress.ReviewCount == 0 ? 10 : Math.Max(10, (progress.IntervalMinutes * 6L + 4) / 5),
            VocabularyReviewRating.Good => Math.Max(1440, progress.IntervalMinutes * 2L),
            VocabularyReviewRating.Easy => Math.Max(4320, progress.IntervalMinutes * 3L),
            _ => throw new ArgumentOutOfRangeException(nameof(rating))
        };
        var minutes = (int)Math.Min(interval, MaxIntervalMinutes);
        return new(minutes < 1440 ? VocabularyLearningStatus.Learning : VocabularyLearningStatus.Reviewing, minutes, Version);
    }
}
