using Domain.Common;

namespace Domain.Pronunciation;

public sealed class PronunciationRuleTranslation : Entity
{
    public const int NameMaxLength = 150;
    public const int ExplanationMaxLength = 8000;
    public const int UsageNoteMaxLength = 2000;

    private PronunciationRuleTranslation()
    {
    }

    public long PronunciationRuleId { get; private init; }
    public string Locale { get; private init; } = null!;
    public string Name { get; private set; } = null!;
    public string Explanation { get; private set; } = null!;
    public string? UsageNote { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static PronunciationRuleTranslation Create(long id, long pronunciationRuleId, string locale,
        string name, string explanation, string? usageNote, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciationRuleId);
        var normalizedLocale = LanguageTag.Normalize(locale, nameof(locale));
        var title = FoundationContentValidation.RequiredText(name, NameMaxLength, nameof(name));
        var content = FoundationContentValidation.RequiredText(explanation, ExplanationMaxLength, nameof(explanation));
        var note = FoundationContentValidation.OptionalText(usageNote, UsageNoteMaxLength, nameof(usageNote));
        var timestamp = now.ToUniversalTime();

        return new PronunciationRuleTranslation
        {
            Id = id,
            PronunciationRuleId = pronunciationRuleId,
            Locale = normalizedLocale,
            Name = title,
            Explanation = content,
            UsageNote = note,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string name, string explanation, string? usageNote, DateTimeOffset now)
    {
        var title = FoundationContentValidation.RequiredText(name, NameMaxLength, nameof(name));
        var content = FoundationContentValidation.RequiredText(explanation, ExplanationMaxLength, nameof(explanation));
        var note = FoundationContentValidation.OptionalText(usageNote, UsageNoteMaxLength, nameof(usageNote));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Name = title;
        Explanation = content;
        UsageNote = note;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
