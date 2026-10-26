using Domain.Common;

namespace Domain.Languages;

public sealed class Language : Entity
{
    private Language()
    {
    }

    public string Code { get; private init; } = null!;
    public string NativeName { get; private set; } = null!;
    public bool IsEnabled { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Language Create(long id, string code, string nativeName, bool isEnabled, int sortOrder,
        DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var normalizedCode = LanguageTag.Normalize(code, nameof(code));
        var name = FoundationContentValidation.RequiredText(nativeName, 150, nameof(nativeName));
        var timestamp = now.ToUniversalTime();

        return new Language
        {
            Id = id,
            Code = normalizedCode,
            NativeName = name,
            IsEnabled = isEnabled,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string nativeName, bool isEnabled, int sortOrder, DateTimeOffset now)
    {
        var name = FoundationContentValidation.RequiredText(nativeName, 150, nameof(nativeName));
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        NativeName = name;
        IsEnabled = isEnabled;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
