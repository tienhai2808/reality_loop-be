using Domain.Common;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Domain.Handwriting;

/// <summary>An immutable, already-graded submission. A retry is another attempt, never an overwrite.</summary>
public sealed class HandwritingPracticeAttempt : Entity
{
    public const int MaxCharacters = 4;

    private readonly List<HandwritingPracticeCharacter> _characters = [];

    private HandwritingPracticeAttempt()
    {
    }

    public long LearningProfileId { get; private init; }
    public long WritingSystemId { get; private init; }
    public string TargetText { get; private init; } = null!;
    public string AlgorithmVersion { get; private init; } = null!;
    public int CharacterCount { get; private init; }
    public bool Passed { get; private init; }
    public int MatchScore { get; private init; }
    public long Version { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private init; }
    public IReadOnlyCollection<HandwritingPracticeCharacter> Characters => _characters.AsReadOnly();

    public static HandwritingPracticeAttempt Create(long id, LearningProfile profile, WritingSystem system,
        Language language, string algorithmVersion, IReadOnlyList<HandwritingPracticeAnswer> answers, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(system);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(answers);
        var algorithm = FoundationContentValidation.Code(algorithmVersion, nameof(algorithmVersion));
        if (system.LanguageId != language.Id || profile.TargetLanguage != language.Code ||
            !language.IsEnabled || !system.IsPublished)
            throw new ArgumentException("An enabled language and published writing system must match the learning profile.");
        if (answers.Count is < 1 or > MaxCharacters)
            throw new ArgumentException($"An attempt requires 1 to {MaxCharacters} characters.", nameof(answers));
        foreach (var answer in answers)
        {
            ArgumentNullException.ThrowIfNull(answer);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(answer.Id);
            ArgumentNullException.ThrowIfNull(answer.Template);
            ArgumentNullException.ThrowIfNull(answer.Item);
            ArgumentNullException.ThrowIfNull(answer.Drawing);
            ArgumentNullException.ThrowIfNull(answer.Feedback);
            if (!answer.Template.IsPublished || answer.Template.WritingSystemItemId != answer.Item.Id ||
                answer.Item.WritingSystemId != system.Id || answer.Template.Symbol != answer.Item.Symbol)
                throw new ArgumentException("Each answer needs a published template for an unchanged item in this writing system.", nameof(answers));
            if (answer.Feedback.ExpectedStrokeCount != answer.Template.Drawing.Strokes.Count ||
                answer.Feedback.ActualStrokeCount != answer.Drawing.Strokes.Count)
                throw new ArgumentException("Feedback stroke counts must match the reference and submitted drawing.", nameof(answers));
            if (now < answer.Template.CreatedAt)
                throw new ArgumentException("An attempt cannot precede its reference template.", nameof(now));
        }
        if (answers.Select(x => x.Id).Append(id).Distinct().Count() != answers.Count + 1)
            throw new ArgumentException("Attempt and character IDs must be distinct.", nameof(answers));
        if (answers.Sum(x => x.Drawing.PointCount) > InkDrawing.MaxTotalPoints)
            throw new ArgumentException("Too many points in the complete attempt.", nameof(answers));

        var timestamp = now.ToUniversalTime();
        var attempt = new HandwritingPracticeAttempt
        {
            Id = id,
            LearningProfileId = profile.Id,
            WritingSystemId = system.Id,
            TargetText = string.Concat(answers.Select(x => x.Template.Symbol)),
            AlgorithmVersion = algorithm,
            CharacterCount = answers.Count,
            Passed = answers.All(x => x.Feedback.Passed),
            MatchScore = (int)Math.Round(answers.Average(x => x.Feedback.MatchScore)),
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
        for (var position = 0; position < answers.Count; position++)
            attempt._characters.Add(HandwritingPracticeCharacter.Create(id, position, answers[position], timestamp));
        return attempt;
    }
}
