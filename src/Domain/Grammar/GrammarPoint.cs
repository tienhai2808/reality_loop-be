using Domain.Common;

namespace Domain.Grammar;

public sealed class GrammarPoint : Entity
{
    public const int CodeMaxLength = 64;
    public const int NativeNameMaxLength = 150;
    public const int PatternMaxLength = 500;

    private GrammarPoint()
    {
    }

    public long LanguageId { get; private init; }
    public string Code { get; private init; } = null!;
    public string NativeName { get; private set; } = null!;
    public string Pattern { get; private set; } = null!;
    public bool IsPublished { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarPoint Create(long id, long languageId, string code, string nativeName, string pattern,
        bool isPublished, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(languageId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var name = FoundationContentValidation.RequiredText(nativeName, NativeNameMaxLength, nameof(nativeName));
        var structure = FoundationContentValidation.RequiredText(pattern, PatternMaxLength, nameof(pattern));
        var timestamp = now.ToUniversalTime();

        return new GrammarPoint
        {
            Id = id,
            LanguageId = languageId,
            Code = stableCode,
            NativeName = name,
            Pattern = structure,
            IsPublished = isPublished,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string nativeName, string pattern, bool isPublished, int sortOrder, DateTimeOffset now)
    {
        var name = FoundationContentValidation.RequiredText(nativeName, NativeNameMaxLength, nameof(nativeName));
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var structure = FoundationContentValidation.RequiredText(pattern, PatternMaxLength, nameof(pattern));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        NativeName = name;
        Pattern = structure;
        IsPublished = isPublished;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
