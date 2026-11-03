using Domain.Common;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Domain.LearningProgress;

public sealed class WritingSystemItemProgress : Entity
{
    private WritingSystemItemProgress()
    {
    }

    public long LearningProfileId { get; private init; }
    public long WritingSystemItemId { get; private init; }
    public WritingSystemItemProgressStatus Status { get; private set; }
    public DateTimeOffset LastStudiedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // The caller supplies a Snowflake ID and the item's catalog ancestry.
    // Absence of a progress row represents an item the learner has not started.
    public static WritingSystemItemProgress Create(long id, LearningProfile learningProfile,
        WritingSystemItem writingSystemItem, WritingSystem writingSystem, Language language, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(learningProfile);
        ArgumentNullException.ThrowIfNull(writingSystemItem);
        ArgumentNullException.ThrowIfNull(writingSystem);
        ArgumentNullException.ThrowIfNull(language);
        if (writingSystemItem.WritingSystemId != writingSystem.Id)
            throw new ArgumentException("The item must belong to the supplied writing system.", nameof(writingSystemItem));
        if (writingSystem.LanguageId != language.Id)
            throw new ArgumentException("The writing system must belong to the supplied language.", nameof(writingSystem));
        if (learningProfile.TargetLanguage != language.Code)
            throw new ArgumentException("The item must belong to the learning profile's target language.", nameof(writingSystemItem));
        var timestamp = now.ToUniversalTime();

        return new WritingSystemItemProgress
        {
            Id = id,
            LearningProfileId = learningProfile.Id,
            WritingSystemItemId = writingSystemItem.Id,
            Status = WritingSystemItemProgressStatus.Learning,
            LastStudiedAt = timestamp,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void RecordStudy(DateTimeOffset now)
    {
        var timestamp = ValidateTimestamp(now);
        if (LastStudiedAt == timestamp) return;
        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);

        LastStudiedAt = change.UpdatedAt;
        UpdatedAt = change.UpdatedAt;
        Version = change.Version;
    }

    public void MarkCompleted(DateTimeOffset now)
    {
        var timestamp = ValidateTimestamp(now);
        if (Status == WritingSystemItemProgressStatus.Completed) return;
        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);

        Status = WritingSystemItemProgressStatus.Completed;
        CompletedAt = change.UpdatedAt;
        LastStudiedAt = change.UpdatedAt;
        UpdatedAt = change.UpdatedAt;
        Version = change.Version;
    }

    public void RestartLearning(DateTimeOffset now)
    {
        var timestamp = ValidateTimestamp(now);
        if (Status == WritingSystemItemProgressStatus.Learning) return;
        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);

        Status = WritingSystemItemProgressStatus.Learning;
        CompletedAt = null;
        LastStudiedAt = change.UpdatedAt;
        UpdatedAt = change.UpdatedAt;
        Version = change.Version;
    }

    private DateTimeOffset ValidateTimestamp(DateTimeOffset now)
    {
        if (now < UpdatedAt)
            throw new ArgumentException("The study time cannot precede the previous update.", nameof(now));
        return now.ToUniversalTime();
    }
}
