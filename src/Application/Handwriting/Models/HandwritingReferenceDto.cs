using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingReferenceDto(string Id, string WritingSystemItemId, string Symbol, int Revision,
    HandwritingSourceDto Source)
{
    public static HandwritingReferenceDto From(HandwritingTemplate template) => new(template.Id.ToString(),
        template.WritingSystemItemId.ToString(), template.Symbol, template.Revision,
        new(template.SourceUrl, template.SourceRevision, template.Attribution, template.License, template.LicenseUrl));
}
