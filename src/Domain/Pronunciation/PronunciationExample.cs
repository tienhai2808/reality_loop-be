using Domain.Common;

namespace Domain.Pronunciation;

public sealed class PronunciationExample : Entity
{
    public const int CodeMaxLength = 64;
    public const int TextMaxLength = 500;
    public const int TranscriptionMaxLength = 500;
    public const int TranscriptionSystemMaxLength = 64;
    public const int AudioUrlMaxLength = 1024;

    private PronunciationExample()
    {
    }

    public long PronunciationRuleId { get; private init; }
    public string Code { get; private init; } = null!;
    public string Text { get; private set; } = null!;
    public string Transcription { get; private set; } = null!;
    public string TranscriptionSystem { get; private set; } = null!;
    public string? AudioUrl { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static PronunciationExample Create(long id, long pronunciationRuleId, string code, string text,
        string transcription, string transcriptionSystem, string? audioUrl, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciationRuleId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var original = FoundationContentValidation.RequiredText(text, TextMaxLength, nameof(text));
        var reading = FoundationContentValidation.RequiredText(transcription, TranscriptionMaxLength, nameof(transcription));
        var system = FoundationContentValidation.Code(transcriptionSystem, nameof(transcriptionSystem));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        var timestamp = now.ToUniversalTime();

        return new PronunciationExample
        {
            Id = id,
            PronunciationRuleId = pronunciationRuleId,
            Code = stableCode,
            Text = original,
            Transcription = reading,
            TranscriptionSystem = system,
            AudioUrl = audio,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string text, string transcription, string transcriptionSystem, string? audioUrl,
        int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var original = FoundationContentValidation.RequiredText(text, TextMaxLength, nameof(text));
        var reading = FoundationContentValidation.RequiredText(transcription, TranscriptionMaxLength, nameof(transcription));
        var system = FoundationContentValidation.Code(transcriptionSystem, nameof(transcriptionSystem));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Text = original;
        Transcription = reading;
        TranscriptionSystem = system;
        AudioUrl = audio;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
