using Domain.Common;

namespace Domain.Vocabulary;

public sealed class VocabularyEntry : Entity
{
    public const int CodeMaxLength = 64;
    public const int TermMaxLength = 200;
    public const int ReadingMaxLength = 200;
    public const int AudioUrlMaxLength = 1024;

    private VocabularyEntry()
    {
    }

    public long LanguageId { get; private init; }
    public string Code { get; private init; } = null!;
    public string Term { get; private set; } = null!;
    public string? Reading { get; private set; }
    public string? AudioUrl { get; private set; }
    public bool IsPublished { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // The caller supplies a Snowflake ID. Language and stable code cannot change after creation.
    public static VocabularyEntry Create(long id, long languageId, string code, string term, string? reading,
        string? audioUrl, bool isPublished, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(languageId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var text = FoundationContentValidation.RequiredText(term, TermMaxLength, nameof(term));
        var pronunciation = FoundationContentValidation.OptionalText(reading, ReadingMaxLength, nameof(reading));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        var timestamp = now.ToUniversalTime();

        return new VocabularyEntry
        {
            Id = id,
            LanguageId = languageId,
            Code = stableCode,
            Term = text,
            Reading = pronunciation,
            AudioUrl = audio,
            IsPublished = isPublished,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string term, string? reading, string? audioUrl, bool isPublished, int sortOrder, DateTimeOffset now)
    {
        var text = FoundationContentValidation.RequiredText(term, TermMaxLength, nameof(term));
        var pronunciation = FoundationContentValidation.OptionalText(reading, ReadingMaxLength, nameof(reading));
        var audio = FoundationContentValidation.AudioUrl(audioUrl);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Term = text;
        Reading = pronunciation;
        AudioUrl = audio;
        IsPublished = isPublished;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
