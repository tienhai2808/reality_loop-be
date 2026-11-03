using Domain.Common;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Domain.WritingSystemPractice;

public sealed class WritingSystemPracticeSession : Entity
{
    public const int MaxQuestions = 20;

    private readonly List<WritingSystemPracticeSessionItem> _items = [];

    private WritingSystemPracticeSession()
    {
    }

    public long LearningProfileId { get; private init; }
    public long WritingSystemId { get; private init; }
    public int QuestionCount { get; private init; }
    public WritingSystemPracticeSessionStatus Status { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<WritingSystemPracticeSessionItem> Items => _items.AsReadOnly();

    // IDs for the session and its questions come from the existing Snowflake generator.
    public static WritingSystemPracticeSession Create(long id, LearningProfile profile, WritingSystem writingSystem,
        Language language, IReadOnlyList<WritingSystemPracticeQuestion> questions, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(writingSystem);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(questions);
        if (writingSystem.LanguageId != language.Id || profile.TargetLanguage != language.Code)
            throw new ArgumentException("The writing system must belong to the profile's target language.", nameof(writingSystem));
        if (!language.IsEnabled || !writingSystem.IsPublished)
            throw new ArgumentException("Practice requires an enabled language and a published writing system.", nameof(writingSystem));
        if (questions.Count is < 1 or > MaxQuestions)
            throw new ArgumentException($"A session must have 1 to {MaxQuestions} questions.", nameof(questions));
        if (questions.Any(x => x is null || x.WritingSystemId != writingSystem.Id) ||
            questions.Select(x => x.Id).Distinct().Count() != questions.Count ||
            questions.Select(x => x.WritingSystemItemId).Distinct().Count() != questions.Count)
            throw new ArgumentException("Questions must have distinct IDs and target distinct items in this writing system.", nameof(questions));

        var timestamp = now.ToUniversalTime();
        var session = new WritingSystemPracticeSession
        {
            Id = id,
            LearningProfileId = profile.Id,
            WritingSystemId = writingSystem.Id,
            QuestionCount = questions.Count,
            Status = WritingSystemPracticeSessionStatus.InProgress,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
        for (var position = 0; position < questions.Count; position++)
            session._items.Add(WritingSystemPracticeSessionItem.Create(id, position, questions[position], timestamp));
        return session;
    }

    public void Answer(long sessionItemId, int selectedOptionIndex, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sessionItemId);
        var timestamp = ValidateTimestamp(now);
        // A partially loaded EF navigation must never make a session finish prematurely.
        if (_items.Count != QuestionCount)
            throw new InvalidOperationException("All session questions must be loaded before answering.");
        var item = _items.SingleOrDefault(x => x.Id == sessionItemId)
            ?? throw new ArgumentException("The question does not belong to this session.", nameof(sessionItemId));
        if (!item.ValidateAnswer(selectedOptionIndex)) return;
        if (Status != WritingSystemPracticeSessionStatus.InProgress)
            throw new InvalidOperationException("Only an in-progress session accepts new answers.");

        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);
        item.RecordAnswer(selectedOptionIndex, timestamp);
        if (_items.All(x => x.AnsweredAt is not null))
        {
            Status = WritingSystemPracticeSessionStatus.Completed;
            EndedAt = timestamp;
        }
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    public void Abandon(DateTimeOffset now)
    {
        var timestamp = ValidateTimestamp(now);
        if (Status == WritingSystemPracticeSessionStatus.Abandoned) return;
        if (Status == WritingSystemPracticeSessionStatus.Completed)
            throw new InvalidOperationException("A completed session cannot be abandoned.");
        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);
        Status = WritingSystemPracticeSessionStatus.Abandoned;
        EndedAt = change.UpdatedAt;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    private DateTimeOffset ValidateTimestamp(DateTimeOffset now)
    {
        if (now < UpdatedAt)
            throw new ArgumentException("The event time cannot precede the previous update.", nameof(now));
        return now.ToUniversalTime();
    }
}
