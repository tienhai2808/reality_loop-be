using Domain.Common;

namespace Domain.Languages;

public sealed class LanguageTranslation : Entity
{
    private LanguageTranslation()
    {
    }

    public long LanguageId { get; private init; }
    public string Locale { get; private init; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static LanguageTranslation Create(long id, long languageId, string locale, string name,
        string? description, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(languageId);
        var normalizedLocale = LanguageTag.Normalize(locale, nameof(locale));
        var title = FoundationContentValidation.RequiredText(name, 150, nameof(name));
        var detail = FoundationContentValidation.OptionalText(description, 2000, nameof(description));
        var timestamp = now.ToUniversalTime();

        return new LanguageTranslation
        {
            Id = id,
            LanguageId = languageId,
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
        var detail = FoundationContentValidation.OptionalText(description, 2000, nameof(description));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Name = title;
        Description = detail;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
