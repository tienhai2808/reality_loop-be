using Domain.Common;

namespace Domain.WritingSystems;

public sealed class WritingSystemItem : Entity
{
    private WritingSystemItem()
    {
    }

    public long WritingSystemId { get; private init; }
    public string Code { get; private init; } = null!;
    public string Symbol { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public int? RowIndex { get; private set; }
    public int? ColumnIndex { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static WritingSystemItem Create(long id, long writingSystemId, string code, string symbol,
        int sortOrder, int? rowIndex, int? columnIndex, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(writingSystemId);
        var stableCode = FoundationContentValidation.Code(code, nameof(code));
        var text = FoundationContentValidation.RequiredText(symbol, 64, nameof(symbol));
        ValidateLayout(sortOrder, rowIndex, columnIndex);
        var timestamp = now.ToUniversalTime();

        return new WritingSystemItem
        {
            Id = id,
            WritingSystemId = writingSystemId,
            Code = stableCode,
            Symbol = text,
            SortOrder = sortOrder,
            RowIndex = rowIndex,
            ColumnIndex = columnIndex,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string symbol, int sortOrder, int? rowIndex, int? columnIndex, DateTimeOffset now)
    {
        var text = FoundationContentValidation.RequiredText(symbol, 64, nameof(symbol));
        ValidateLayout(sortOrder, rowIndex, columnIndex);
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);

        Symbol = text;
        SortOrder = sortOrder;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    private static void ValidateLayout(int sortOrder, int? rowIndex, int? columnIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        if (rowIndex.HasValue != columnIndex.HasValue || rowIndex < 0 || columnIndex < 0)
            throw new ArgumentException("Grid row and column must either both be absent or both be non-negative.");
    }
}
