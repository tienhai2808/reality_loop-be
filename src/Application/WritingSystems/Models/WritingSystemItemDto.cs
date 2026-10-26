namespace Application.WritingSystems.Models;

public sealed record WritingSystemItemDto(string Id, string Code, string Symbol, int SortOrder, int? RowIndex,
    int? ColumnIndex, long Version, DateTimeOffset UpdatedAt, WritingSystemItemReadingDto? PrimaryReading);
