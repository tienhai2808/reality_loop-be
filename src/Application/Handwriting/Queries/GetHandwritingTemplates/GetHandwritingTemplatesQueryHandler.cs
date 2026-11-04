using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Handwriting.Models;
using Application.Handwriting.Validation;

namespace Application.Handwriting.Queries.GetHandwritingTemplates;

public sealed class GetHandwritingTemplatesQueryHandler(ICurrentUser current, IHandwritingRepository repository)
{
    public Task<HandwritingTemplatePage> Handle(GetHandwritingTemplatesQuery query, CancellationToken ct)
    {
        HandwritingValidation.Id(query.ProfileId);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetTemplatesAsync(current.UserId, query.ProfileId, code, query.PageSize, cursor, ct);
    }
}
