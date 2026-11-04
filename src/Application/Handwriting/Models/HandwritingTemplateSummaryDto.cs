namespace Application.Handwriting.Models;

public sealed record HandwritingTemplateSummaryDto(HandwritingReferenceDto Reference, string ItemCode,
    int SortOrder, long Version, DateTimeOffset UpdatedAt);
