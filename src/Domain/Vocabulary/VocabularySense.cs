using Domain.Common;

namespace Domain.Vocabulary;

public sealed class VocabularySense : Entity
{
    public const int CodeMaxLength = 64;

    private VocabularySense()
    {
    }

    public long VocabularyEntryId { get; private init; }
    public string Code { get; private init; } = null!;
    public PartOfSpeech PartOfSpeech { get; private set; }
    public int SortOrder { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static VocabularySense Create(long id, long vocabularyEntryId, string code, PartOfSpeech partOfSpeech,
        int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vocabularyEntryId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        if (!Enum.IsDefined(partOfSpeech)) throw new ArgumentOutOfRangeException(nameof(partOfSpeech));
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var timestamp = now.ToUniversalTime();

        return new VocabularySense
        {
            Id = id,
            VocabularyEntryId = vocabularyEntryId,
            Code = stableCode,
            PartOfSpeech = partOfSpeech,
            SortOrder = sortOrder,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(PartOfSpeech partOfSpeech, int sortOrder, DateTimeOffset now)
    {
        if (!Enum.IsDefined(partOfSpeech)) throw new ArgumentOutOfRangeException(nameof(partOfSpeech));
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        PartOfSpeech = partOfSpeech;
        SortOrder = sortOrder;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
