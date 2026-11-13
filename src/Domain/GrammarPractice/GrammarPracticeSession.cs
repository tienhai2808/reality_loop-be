using Domain.Common;
using Domain.Grammar;
using Domain.Languages;
using Domain.LearningProfiles;

namespace Domain.GrammarPractice;

public sealed class GrammarPracticeSession : Entity
{
    public const int MinQuestions = 5;
    public const int MaxQuestions = 10;

    private readonly List<GrammarPracticeSessionItem> _items = [];

    private GrammarPracticeSession()
    {
    }

    public long LearningProfileId { get; private init; }
    public long LanguageId { get; private init; }
    public long? GrammarPointId { get; private init; }
    public string ExplanationLanguage { get; private init; } = null!;
    public int QuestionCount { get; private init; }
    public GrammarPracticeSessionStatus Status { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<GrammarPracticeSessionItem> Items => _items.AsReadOnly();

    // IDs for the session and its questions come from the existing Snowflake generator.
    public static GrammarPracticeSession Create(long id, LearningProfile profile, Language language,
        IReadOnlyList<GrammarPracticeQuestion> questions, DateTimeOffset now, GrammarPoint? grammarPoint = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(questions);
        if (profile.TargetLanguage != language.Code || !language.IsEnabled)
            throw new ArgumentException("The enabled language must match the learning profile.");
        if (grammarPoint is not null && (grammarPoint.LanguageId != language.Id || !grammarPoint.IsPublished))
            throw new ArgumentException("The selected grammar point must be published in this language.");
        if (questions.Count is < MinQuestions or > MaxQuestions)
            throw new ArgumentException($"A session must have {MinQuestions} to {MaxQuestions} questions.", nameof(questions));
        if (questions.Any(question => question is null || question.LanguageId != language.Id ||
                (grammarPoint is not null && question.GrammarPointId != grammarPoint.Id)) ||
            questions.Select(question => question.Id).Append(id).Distinct().Count() != questions.Count + 1 ||
            questions.Select(question => question.GrammarExerciseId).Distinct().Count() != questions.Count)
            throw new ArgumentException("Questions must have distinct IDs and target distinct exercises in the selected scope.");
        if (questions.Any(question => now < question.SourceUpdatedAt))
            throw new ArgumentException("A session cannot precede its source content.", nameof(now));
        var timestamp = now.ToUniversalTime();
        var session = new GrammarPracticeSession
        {
            Id = id,
            LearningProfileId = profile.Id,
            LanguageId = language.Id,
            GrammarPointId = grammarPoint?.Id,
            ExplanationLanguage = profile.ExplanationLanguage,
            QuestionCount = questions.Count,
            Status = GrammarPracticeSessionStatus.InProgress,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
        for (var position = 0; position < questions.Count; position++)
            session._items.Add(GrammarPracticeSessionItem.Create(id, position, questions[position], timestamp));
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
        if (Status != GrammarPracticeSessionStatus.InProgress)
            throw new InvalidOperationException("Only an in-progress session accepts new answers.");

        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);
        item.RecordAnswer(selectedOptionIndex, timestamp);
        if (_items.All(x => x.AnsweredAt is not null))
        {
            Status = GrammarPracticeSessionStatus.Completed;
            EndedAt = timestamp;
        }
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    public void Abandon(DateTimeOffset now)
    {
        var timestamp = ValidateTimestamp(now);
        if (Status == GrammarPracticeSessionStatus.Abandoned) return;
        if (Status == GrammarPracticeSessionStatus.Completed)
            throw new InvalidOperationException("A completed session cannot be abandoned.");
        var change = FoundationContentValidation.Next(Version, UpdatedAt, timestamp);
        Status = GrammarPracticeSessionStatus.Abandoned;
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
