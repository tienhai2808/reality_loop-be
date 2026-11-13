using Domain.Common;

namespace Domain.GrammarPractice;

public sealed class GrammarExercise : Entity
{
    public const int PromptMaxLength = 1000;
    public const int OptionMaxLength = 250;
    public const int MinOptions = 2;
    public const int MaxOptions = 4;

    private string[] _options = [];

    private GrammarExercise()
    {
    }

    public long GrammarPointId { get; private init; }
    public string Code { get; private init; } = null!;
    public string Prompt { get; private set; } = null!;
    public string[] Options => (string[])_options.Clone();
    public int CorrectOptionIndex { get; private set; }
    public bool IsPublished { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarExercise Create(long id, long grammarPointId, string code, string prompt,
        IReadOnlyList<string> options, int correctOptionIndex, bool isPublished, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(grammarPointId);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var text = ValidatePrompt(prompt);
        var choices = ValidateOptions(options, correctOptionIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var timestamp = now.ToUniversalTime();
        return new GrammarExercise
        {
            Id = id,
            GrammarPointId = grammarPointId,
            Code = stableCode,
            Prompt = text,
            _options = choices,
            CorrectOptionIndex = correctOptionIndex,
            IsPublished = isPublished,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string prompt, IReadOnlyList<string> options, int correctOptionIndex,
        bool isPublished, int sortOrder, DateTimeOffset now)
    {
        var text = ValidatePrompt(prompt);
        var choices = ValidateOptions(options, correctOptionIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);
        Prompt = text;
        _options = choices;
        CorrectOptionIndex = correctOptionIndex;
        IsPublished = isPublished;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    private static string ValidatePrompt(string prompt)
    {
        var text = FoundationContentValidation.RequiredText(prompt, PromptMaxLength, nameof(prompt));
        var blank = text.IndexOf("{blank}", StringComparison.Ordinal);
        if (blank < 0 || blank != text.LastIndexOf("{blank}", StringComparison.Ordinal))
            throw new ArgumentException("A prompt must contain exactly one {blank} placeholder.", nameof(prompt));
        return text;
    }

    private static string[] ValidateOptions(IReadOnlyList<string> options, int correctOptionIndex)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Count is < MinOptions or > MaxOptions)
            throw new ArgumentException($"An exercise requires {MinOptions} to {MaxOptions} options.", nameof(options));
        if (correctOptionIndex < 0 || correctOptionIndex >= options.Count)
            throw new ArgumentOutOfRangeException(nameof(correctOptionIndex));
        var choices = options.Select(value => FoundationContentValidation.RequiredText(value, OptionMaxLength, nameof(options))).ToArray();
        // Case changes can be the subject of a grammar exercise, for example sie/Sie.
        if (choices.Distinct(StringComparer.Ordinal).Count() != choices.Length)
            throw new ArgumentException("Options must be distinct after text normalization.", nameof(options));
        return choices;
    }
}
