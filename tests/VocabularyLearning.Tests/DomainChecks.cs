using System.Reflection;
using System.Text.Json;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.Vocabulary;
using Domain.VocabularyLearning;

namespace VocabularyLearning.Tests;

internal static class DomainChecks
{
    internal static readonly DateTimeOffset Now = new(2026, 11, 5, 12, 0, 0, TimeSpan.Zero);
    internal static readonly LearningProfile Profile = LearningProfile.Create(2, 1, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, Now);
    internal static readonly Language Language = Language.Create(3, "ja", "日本語", true, 0, Now);
    internal static readonly VocabularyEntry Entry = VocabularyEntry.Create(4, 3, "test", "語", null, null, true, 0, Now);
    internal static readonly VocabularySense Sense = VocabularySense.Create(5, 4, "main", PartOfSpeech.Noun, 0, Now);
    internal static VocabularyLearningProgress New() => VocabularyLearningProgress.Create(6, Profile, Sense, Entry, Language, Now);

    public static void Run()
    {
        var root = New();
        Check.That(root.Status == VocabularyLearningStatus.New && root.NextReviewAt == Now && root.LastReviewedAt is null, "New card is immediately due and not falsely reviewed.");
        Check.That(root.ReviewCount == 0 && root.AgainCount == 0 && root.ConsecutiveRecallCount == 0 && root.IntervalMinutes == 0 && root.SchedulerVersion is null && root.Version == 1, "Initial scheduler/counters/version.");
        var initial = JsonSerializer.Serialize(root);
        foreach (var rating in Enum.GetValues<VocabularyReviewRating>())
        {
            var progress = New();
            var request = Guid.NewGuid();
            var review = progress.RecordReview(7, request, rating, new(VocabularyLearningStatus.Learning, 10, "  SIMPLE-V1  "), Now.AddMinutes(1));
            Check.That(progress.ReviewCount == 1 && progress.Version == 2 && progress.NextReviewAt == Now.AddMinutes(11), "Every rating records one review and schedules from review time.");
            Check.That(review.ProgressId == progress.Id && review.RequestId == request && review.ProgressVersionBefore == 1 && review.ProgressVersionAfter == 2 && review.ReviewNumber == 1, "History ties request/sequence/version to the correct root.");
            Check.That(review.PreviousStatus == VocabularyLearningStatus.New && review.PreviousDueAt == Now && review.PreviousIntervalMinutes == 0 && review.NextReviewAt == progress.NextReviewAt && review.SchedulerVersion == "simple-v1", "Before/after schedule snapshot.");
            Check.That(progress.AgainCount == (rating == VocabularyReviewRating.Again ? 1 : 0) && progress.ConsecutiveRecallCount == (rating == VocabularyReviewRating.Again ? 0 : 1), "Self-assessment counters match rating.");
        }
        var first = root.RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good, new(VocabularyLearningStatus.Reviewing, 1440, "simple-v1"), Now);
        var second = root.RecordReview(8, Guid.NewGuid(), VocabularyReviewRating.Again, new(VocabularyLearningStatus.Learning, 1, "simple-v2"), Now.AddMinutes(2));
        Check.That(root.ReviewCount == 2 && root.AgainCount == 1 && root.ConsecutiveRecallCount == 0 && root.Version == 3, "Forgetting resets only the recall streak and returns to learning.");
        Check.That(first.NextReviewAt == Now.AddDays(1) && second.PreviousDueAt == first.NextReviewAt && second.ProgressVersionBefore == 2, "Early reviews are allowed; immutable previous history stays unchanged.");
        Check.That(root.NextReviewAt == Now.AddMinutes(3) && root.Status == VocabularyLearningStatus.Learning, "New schedule is based on actual review time, not the old due date.");
        Check.That(Profile.Version == 1, "Learning state does not mutate profile preferences.");
        var utc = VocabularyLearningProgress.Create(9, Profile, Sense, Entry, Language, Now.ToOffset(TimeSpan.FromHours(7)));
        Check.That(utc.CreatedAt.Offset == TimeSpan.Zero && utc.NextReviewAt == Now, "Normalize new timestamps to UTC.");
        utc.RecordReview(10, Guid.NewGuid(), VocabularyReviewRating.Easy, new(VocabularyLearningStatus.Reviewing, 60, "simple-v1"), Now.ToOffset(TimeSpan.FromHours(7)));
        Check.That(utc.UpdatedAt.Offset == TimeSpan.Zero && utc.NextReviewAt == Now.AddHours(1), "Normalize review timestamps to UTC.");
        foreach (var action in new Action[]
        {
            () => New().RecordReview(0, Guid.NewGuid(), VocabularyReviewRating.Good, new(VocabularyLearningStatus.Learning, 1, "v1"), Now),
            () => New().RecordReview(6, Guid.NewGuid(), VocabularyReviewRating.Good, new(VocabularyLearningStatus.Learning, 1, "v1"), Now),
            () => New().RecordReview(7, Guid.Empty, VocabularyReviewRating.Good, new(VocabularyLearningStatus.Learning, 1, "v1"), Now),
            () => New().RecordReview(7, Guid.NewGuid(), (VocabularyReviewRating)99, new(VocabularyLearningStatus.Learning, 1, "v1"), Now),
            () => New().RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Again, new(VocabularyLearningStatus.Reviewing, 1, "v1"), Now),
            () => New().RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good, null!, Now),
            () => New().RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good, new(VocabularyLearningStatus.Learning, 1, "v1"), Now.AddSeconds(-1)),
            () => new VocabularyReviewSchedule(VocabularyLearningStatus.New, 1, "v1"),
            () => new VocabularyReviewSchedule((VocabularyLearningStatus)99, 1, "v1"),
            () => new VocabularyReviewSchedule(VocabularyLearningStatus.Learning, 0, "v1"),
            () => new VocabularyReviewSchedule(VocabularyLearningStatus.Learning, -1, "v1"),
            () => new VocabularyReviewSchedule(VocabularyLearningStatus.Learning, 1, "bad_code"),
            () => new VocabularyReviewSchedule(VocabularyLearningStatus.Learning, 1, ""),
            () => VocabularyLearningProgress.Create(0, Profile, Sense, Entry, Language, Now),
            () => VocabularyLearningProgress.Create(6, Profile, VocabularySense.Create(20, 21, "wrong", PartOfSpeech.Noun, 0, Now), Entry, Language, Now),
            () => VocabularyLearningProgress.Create(6, Profile, Sense, Entry, Language.Create(22, "en", "English", true, 0, Now), Now),
            () => VocabularyLearningProgress.Create(6, LearningProfile.Create(23, 1, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Work, null, Now), Sense, Entry, Language, Now)
        }) Check.Throws<ArgumentException>(action, "Reject invalid model input.");
        foreach (var field in new[] { "Version", "ReviewCount", "ConsecutiveRecallCount" })
        {
            var overflow = New();
            typeof(VocabularyLearningProgress).GetProperty(field)!.SetValue(overflow, long.MaxValue);
            var before = JsonSerializer.Serialize(overflow);
            Check.Throws<OverflowException>(() => overflow.RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good,
                new(VocabularyLearningStatus.Learning, 1, "v1"), Now), "Counter/version overflow rejected.");
            Check.That(before == JsonSerializer.Serialize(overflow), "Overflow leaves the whole root unchanged.");
        }
        var date = New();
        Check.Throws<ArgumentOutOfRangeException>(() => date.RecordReview(7, Guid.NewGuid(), VocabularyReviewRating.Good,
            new(VocabularyLearningStatus.Reviewing, 60, "v1"), DateTimeOffset.MaxValue), "Date overflow rejected before mutation.");
        Check.That(initial == JsonSerializer.Serialize(date), "Date overflow is atomic.");
        foreach (var type in new[] { typeof(VocabularyLearningProgress), typeof(VocabularyReview) })
        {
            Check.That(!type.GetProperties().Any(x => x.SetMethod?.IsPublic == true), "No public entity setters.");
            Check.That(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0, "Factory-controlled entities.");
        }
    }
}
