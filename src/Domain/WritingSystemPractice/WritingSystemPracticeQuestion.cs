using Domain.Common;
using Domain.WritingSystems;

namespace Domain.WritingSystemPractice;

// Immutable input for constructing a session; this is not a separate persisted entity.
public sealed class WritingSystemPracticeQuestion
{
    public const int MinOptions = 2;
    public const int MaxOptions = 4;

    private readonly string[] _options;

    private WritingSystemPracticeQuestion(long id, WritingSystemItem item, string transcriptionSystem,
        string[] options, int correctOptionIndex)
    {
        Id = id;
        WritingSystemItemId = item.Id;
        WritingSystemId = item.WritingSystemId;
        Symbol = item.Symbol;
        TranscriptionSystem = transcriptionSystem;
        _options = options;
        CorrectOptionIndex = correctOptionIndex;
    }

    public long Id { get; }
    public long WritingSystemItemId { get; }
    public long WritingSystemId { get; }
    public string Symbol { get; }
    public string TranscriptionSystem { get; }
    public IReadOnlyList<string> Options => Array.AsReadOnly(_options);
    public int CorrectOptionIndex { get; }

    // Supply all catalog readings for the target item so another valid reading cannot be a distractor.
    // Selection/shuffling belongs to the future application use case, not to Domain.
    public static WritingSystemPracticeQuestion Create(long id, WritingSystemItem item,
        WritingSystemItemReading correctReading, IReadOnlyCollection<WritingSystemItemReading> itemReadings,
        IReadOnlyList<string> options)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(correctReading);
        ArgumentNullException.ThrowIfNull(itemReadings);
        ArgumentNullException.ThrowIfNull(options);
        if (correctReading.WritingSystemItemId != item.Id ||
            itemReadings.Any(x => x is null || x.WritingSystemItemId != item.Id) ||
            !itemReadings.Any(x => x.Id == correctReading.Id && x.Transcription == correctReading.Transcription &&
                                   x.TranscriptionSystem == correctReading.TranscriptionSystem))
            throw new ArgumentException("Readings must belong to the target item and include the correct reading.", nameof(itemReadings));
        if (options.Count is < MinOptions or > MaxOptions)
            throw new ArgumentException($"A question must have {MinOptions} to {MaxOptions} options.", nameof(options));

        var snapshot = options.Select(x => FoundationContentValidation.RequiredText(x, 200, nameof(options))).ToArray();
        if (snapshot.Distinct(StringComparer.OrdinalIgnoreCase).Count() != snapshot.Length)
            throw new ArgumentException("Options must be distinct after text normalization, ignoring case.", nameof(options));
        var correctIndex = Array.FindIndex(snapshot,
            x => string.Equals(x, correctReading.Transcription, StringComparison.OrdinalIgnoreCase));
        var acceptedReadings = itemReadings.Where(x => x.TranscriptionSystem == correctReading.TranscriptionSystem)
            .Select(x => x.Transcription).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (correctIndex < 0 || snapshot.Count(acceptedReadings.Contains) != 1)
            throw new ArgumentException("Exactly one option must be a valid reading of the target item.", nameof(options));

        return new WritingSystemPracticeQuestion(id, item, correctReading.TranscriptionSystem, snapshot, correctIndex);
    }
}
