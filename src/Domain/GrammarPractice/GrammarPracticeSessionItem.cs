using Domain.Common;

namespace Domain.GrammarPractice;

public sealed class GrammarPracticeSessionItem : Entity
{
    private string[] _options = [];

    private GrammarPracticeSessionItem()
    {
    }

    public long SessionId { get; private init; }
    public long GrammarExerciseId { get; private init; }
    public long GrammarPointId { get; private init; }
    public long ExerciseVersion { get; private init; }
    public long TranslationVersion { get; private init; }
    public string Instruction { get; private init; } = null!;
    public string Explanation { get; private init; } = null!;
    public int Position { get; private init; }
    public string Prompt { get; private init; } = null!;
    public string TranslationLocale { get; private init; } = null!;
    // A defensive copy prevents callers from changing the persisted question through an array reference.
    public string[] Options => (string[])_options.Clone();
    public int CorrectOptionIndex { get; private init; }
    public int? SelectedOptionIndex { get; private set; }
    public DateTimeOffset? AnsweredAt { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public bool? IsCorrect => SelectedOptionIndex is { } selected ? selected == CorrectOptionIndex : null;

    internal static GrammarPracticeSessionItem Create(long sessionId, int position,
        GrammarPracticeQuestion question, DateTimeOffset now) => new()
    {
        Id = question.Id,
        SessionId = sessionId,
        GrammarExerciseId = question.GrammarExerciseId,
        GrammarPointId = question.GrammarPointId,
        ExerciseVersion = question.ExerciseVersion,
        TranslationVersion = question.TranslationVersion,
        Instruction = question.Instruction,
        Explanation = question.Explanation,
        Position = position,
        Prompt = question.Prompt,
        TranslationLocale = question.Locale,
        _options = question.Options.ToArray(),
        CorrectOptionIndex = question.CorrectOptionIndex,
        Version = 1,
        CreatedAt = now,
        UpdatedAt = now
    };

    internal bool ValidateAnswer(int selectedOptionIndex)
    {
        if (selectedOptionIndex < 0 || selectedOptionIndex >= _options.Length)
            throw new ArgumentOutOfRangeException(nameof(selectedOptionIndex));
        if (SelectedOptionIndex is null) return true;
        if (SelectedOptionIndex == selectedOptionIndex) return false;
        throw new InvalidOperationException("An answered question cannot be answered again with a different option.");
    }

    internal void RecordAnswer(int selectedOptionIndex, DateTimeOffset now)
    {
        if (!ValidateAnswer(selectedOptionIndex)) return;
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);
        SelectedOptionIndex = selectedOptionIndex;
        AnsweredAt = change.UpdatedAt;
        UpdatedAt = change.UpdatedAt;
        Version = change.Version;
    }
}
