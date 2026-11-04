using Domain.Common;

namespace Domain.Vocabulary;

public sealed class VocabularySenseTranslation : Entity
{
    public const int MeaningMaxLength = 500;
    public const int UsageNoteMaxLength = 2000;

    private VocabularySenseTranslation()
    {
    }

    public long VocabularySenseId { get; private init; }
    public string Locale { get; private init; } = null!;
    public string Meaning { get; private set; } = null!;
    public string? UsageNote { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static VocabularySenseTranslation Create(long id, long vocabularySenseId, string locale,
        string meaning, string? usageNote, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vocabularySenseId);
        var normalizedLocale = LanguageTag.Normalize(locale, nameof(locale));
        var text = FoundationContentValidation.RequiredText(meaning, MeaningMaxLength, nameof(meaning));
        var note = FoundationContentValidation.OptionalText(usageNote, UsageNoteMaxLength, nameof(usageNote));
        var timestamp = now.ToUniversalTime();

        return new VocabularySenseTranslation
        {
            Id = id,
            VocabularySenseId = vocabularySenseId,
            Locale = normalizedLocale,
            Meaning = text,
            UsageNote = note,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string meaning, string? usageNote, DateTimeOffset now)
    {
        var text = FoundationContentValidation.RequiredText(meaning, MeaningMaxLength, nameof(meaning));
        var note = FoundationContentValidation.OptionalText(usageNote, UsageNoteMaxLength, nameof(usageNote));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Meaning = text;
        UsageNote = note;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
