using Domain.Common;

namespace Domain.WritingSystemPractice;

public sealed class WritingSystemPracticeSessionItem : Entity
{
    private string[] _options = [];

    private WritingSystemPracticeSessionItem()
    {
    }

    public long SessionId { get; private init; }
    public long WritingSystemItemId { get; private init; }
    public int Position { get; private init; }
    public string Symbol { get; private init; } = null!;
    public string TranscriptionSystem { get; private init; } = null!;
    // A defensive copy prevents callers from changing the persisted question through an array reference.
    public string[] Options => (string[])_options.Clone();
    public int CorrectOptionIndex { get; private init; }
    public int? SelectedOptionIndex { get; private set; }
    public DateTimeOffset? AnsweredAt { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public bool? IsCorrect => SelectedOptionIndex is { } selected ? selected == CorrectOptionIndex : null;

    internal static WritingSystemPracticeSessionItem Create(long sessionId, int position,
        WritingSystemPracticeQuestion question, DateTimeOffset now) => new()
    {
        Id = question.Id,
        SessionId = sessionId,
        WritingSystemItemId = question.WritingSystemItemId,
        Position = position,
        Symbol = question.Symbol,
        TranscriptionSystem = question.TranscriptionSystem,
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
