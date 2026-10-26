using Domain.Common;

namespace Domain.Pronunciation;

public sealed class PronunciationRule : Entity
{
    public const int CodeMaxLength = 64;
    public const int NativeNameMaxLength = 150;

    private PronunciationRule()
    {
    }

    public long LanguageId { get; private init; }
    public string Code { get; private init; } = null!;
    public string NativeName { get; private set; } = null!;
    public bool IsPublished { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static PronunciationRule Create(long id, long languageId, string code, string nativeName,
        bool isPublished, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(languageId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var name = FoundationContentValidation.RequiredText(nativeName, NativeNameMaxLength, nameof(nativeName));
        var timestamp = now.ToUniversalTime();

        return new PronunciationRule
        {
            Id = id,
            LanguageId = languageId,
            Code = stableCode,
            NativeName = name,
            IsPublished = isPublished,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string nativeName, bool isPublished, int sortOrder, DateTimeOffset now)
    {
        var name = FoundationContentValidation.RequiredText(nativeName, NativeNameMaxLength, nameof(nativeName));
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        NativeName = name;
        IsPublished = isPublished;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
