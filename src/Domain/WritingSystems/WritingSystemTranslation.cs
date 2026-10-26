using Domain.Common;

namespace Domain.WritingSystems;

public sealed class WritingSystemTranslation : Entity
{
    private WritingSystemTranslation()
    {
    }

    public long WritingSystemId { get; private init; }
    public string Locale { get; private init; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static WritingSystemTranslation Create(long id, long writingSystemId, string locale, string name,
        string? description, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(writingSystemId);
        var normalizedLocale = LanguageTag.Normalize(locale, nameof(locale));
        var title = FoundationContentValidation.RequiredText(name, 150, nameof(name));
        var detail = FoundationContentValidation.OptionalText(description, 4000, nameof(description));
        var timestamp = now.ToUniversalTime();

        return new WritingSystemTranslation
        {
            Id = id,
            WritingSystemId = writingSystemId,
            Locale = normalizedLocale,
            Name = title,
            Description = detail,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string name, string? description, DateTimeOffset now)
    {
        var title = FoundationContentValidation.RequiredText(name, 150, nameof(name));
        var detail = FoundationContentValidation.OptionalText(description, 4000, nameof(description));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Name = title;
        Description = detail;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
