using Application.VocabularyLearning.Services;
using Domain.VocabularyLearning;

namespace VocabularyLearning.Tests;

internal static class SchedulerChecks
{
    public static void Run()
    {
        var scheduler = new VocabularyReviewScheduler();
        var root = DomainChecks.New();
        foreach (var (rating, expected) in new[] { (VocabularyReviewRating.Again, 1), (VocabularyReviewRating.Hard, 10),
                     (VocabularyReviewRating.Good, 1440), (VocabularyReviewRating.Easy, 4320) })
        {
            var schedule = scheduler.Schedule(root, rating);
            Check.That(schedule.IntervalMinutes == expected && schedule.SchedulerVersion == "simple-v1" && root.Version == 1,
                "First-review policy and pure scheduler.");
        }
        root.RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good, scheduler.Schedule(root, VocabularyReviewRating.Good), DomainChecks.Now);
        Check.That(scheduler.Schedule(root, VocabularyReviewRating.Hard).IntervalMinutes == 1728, "Hard uses ceil(1.2 * previous interval).");
        Check.That(scheduler.Schedule(root, VocabularyReviewRating.Good).IntervalMinutes == 2880, "Good doubles interval.");
        Check.That(scheduler.Schedule(root, VocabularyReviewRating.Easy).IntervalMinutes == 4320, "Easy triples interval.");
        for (var i = 0; i < 20; i++)
            root.RecordReview(20 + i, Guid.NewGuid(), VocabularyReviewRating.Easy, scheduler.Schedule(root, VocabularyReviewRating.Easy), DomainChecks.Now);
        Check.That(root.IntervalMinutes == VocabularyReviewScheduler.MaxIntervalMinutes, "Intervals cap at one year without overflow.");
        Check.That(scheduler.Schedule(root, VocabularyReviewRating.Again) is { IntervalMinutes: 1, Status: VocabularyLearningStatus.Learning }, "Again always relearns after one minute.");
        Check.Throws<ArgumentOutOfRangeException>(() => scheduler.Schedule(root, (VocabularyReviewRating)99), "Unknown rating is rejected.");
    }
}
