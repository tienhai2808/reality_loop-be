using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Handwriting.Models;
using Application.Handwriting.Validation;

namespace Application.Handwriting.Queries.GetHandwritingTemplate;

public sealed class GetHandwritingTemplateQueryHandler(ICurrentUser current, IHandwritingRepository repository)
{
    public async Task<HandwritingTemplateDto> Handle(GetHandwritingTemplateQuery query, CancellationToken ct)
    {
        HandwritingValidation.Id(query.ProfileId);
        HandwritingValidation.Id(query.TemplateId);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        return await repository.GetTemplateAsync(current.UserId, query.ProfileId, code, query.TemplateId, ct)
            ?? throw new NotFoundException("Published handwriting template was not found.");
    }
}
