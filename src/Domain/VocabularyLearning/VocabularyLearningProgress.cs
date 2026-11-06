using Domain.Common;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.Vocabulary;

namespace Domain.VocabularyLearning;

public sealed class VocabularyLearningProgress : Entity
{
    private VocabularyLearningProgress()
    {
    }

    public long LearningProfileId { get; private init; }
    public long VocabularySenseId { get; private init; }
    public VocabularyLearningStatus Status { get; private set; }
    public DateTimeOffset NextReviewAt { get; private set; }
    public DateTimeOffset? LastReviewedAt { get; private set; }
    public int IntervalMinutes { get; private set; }
    public long ReviewCount { get; private set; }
    public long AgainCount { get; private set; }
    public long ConsecutiveRecallCount { get; private set; }
    public string? SchedulerVersion { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static VocabularyLearningProgress Create(long id, LearningProfile profile, VocabularySense sense,
        VocabularyEntry entry, Language language, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(language);
        if (sense.VocabularyEntryId != entry.Id || entry.LanguageId != language.Id || profile.TargetLanguage != language.Code)
            throw new ArgumentException("The vocabulary sense must belong to the profile's target language.", nameof(sense));
        if (!entry.IsPublished || !language.IsEnabled)
            throw new ArgumentException("Learning requires a published entry and an enabled language.", nameof(entry));
        var timestamp = now.ToUniversalTime();
        return new()
        {
            Id = id,
            LearningProfileId = profile.Id,
            VocabularySenseId = sense.Id,
            Status = VocabularyLearningStatus.New,
            NextReviewAt = timestamp,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    // Returns an immutable history row. Persist it together with this root in one transaction.
    // No history navigation is needed: recording one review never requires loading prior reviews.
    public VocabularyReview RecordReview(long reviewId, Guid requestId, VocabularyReviewRating rating,
        VocabularyReviewSchedule schedule, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(reviewId);
        if (reviewId == Id) throw new ArgumentException("Review and progress IDs must differ.", nameof(reviewId));
        if (requestId == Guid.Empty) throw new ArgumentException("A non-empty review request ID is required.", nameof(requestId));
        if (!Enum.IsDefined(rating)) throw new ArgumentOutOfRangeException(nameof(rating));
        ArgumentNullException.ThrowIfNull(schedule);
        if (rating == VocabularyReviewRating.Again && schedule.Status != VocabularyLearningStatus.Learning)
            throw new ArgumentException("A forgotten card must return to learning.", nameof(schedule));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);
        var nextDue = change.UpdatedAt.AddMinutes(schedule.IntervalMinutes);
        var reviewCount = checked(ReviewCount + 1);
        var againCount = rating == VocabularyReviewRating.Again ? checked(AgainCount + 1) : AgainCount;
        var recallCount = rating == VocabularyReviewRating.Again ? 0 : checked(ConsecutiveRecallCount + 1);
        // Calculate and validate every value before mutating the root (including date/count overflows).
        var review = VocabularyReview.Create(reviewId, this, requestId, rating, schedule, nextDue,
            reviewCount, againCount, recallCount, change.Version, change.UpdatedAt);
        Status = schedule.Status;
        NextReviewAt = nextDue;
        LastReviewedAt = change.UpdatedAt;
        IntervalMinutes = schedule.IntervalMinutes;
        ReviewCount = reviewCount;
        AgainCount = againCount;
        ConsecutiveRecallCount = recallCount;
        SchedulerVersion = schedule.SchedulerVersion;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
        return review;
    }
}
