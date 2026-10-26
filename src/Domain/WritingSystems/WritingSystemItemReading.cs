using Domain.Common;

namespace Domain.WritingSystems;

public sealed class WritingSystemItemReading : Entity
{
    private WritingSystemItemReading()
    {
    }

    public long WritingSystemItemId { get; private init; }
    public string Code { get; private init; } = null!;
    public string Transcription { get; private set; } = null!;
    public string TranscriptionSystem { get; private set; } = null!;
    public string? AudioUrl { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static WritingSystemItemReading Create(long id, long writingSystemItemId, string code,
        string transcription, string transcriptionSystem, string? audioUrl, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(writingSystemItemId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var text = FoundationContentValidation.RequiredText(transcription, 200, nameof(transcription));
        var system = FoundationContentValidation.Code(transcriptionSystem, nameof(transcriptionSystem));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        var timestamp = now.ToUniversalTime();

        return new WritingSystemItemReading
        {
            Id = id,
            WritingSystemItemId = writingSystemItemId,
            Code = stableCode,
            Transcription = text,
            TranscriptionSystem = system,
            AudioUrl = audio,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string transcription, string transcriptionSystem, string? audioUrl, int sortOrder,
        DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var text = FoundationContentValidation.RequiredText(transcription, 200, nameof(transcription));
        var system = FoundationContentValidation.Code(transcriptionSystem, nameof(transcriptionSystem));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Transcription = text;
        TranscriptionSystem = system;
        AudioUrl = audio;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
