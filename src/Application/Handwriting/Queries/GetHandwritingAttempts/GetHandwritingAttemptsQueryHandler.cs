using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Handwriting.Models;
using Application.Handwriting.Validation;

namespace Application.Handwriting.Queries.GetHandwritingAttempts;

public sealed class GetHandwritingAttemptsQueryHandler(ICurrentUser current, IHandwritingRepository repository)
{
    public Task<HandwritingAttemptPage> Handle(GetHandwritingAttemptsQuery query, CancellationToken ct)
    {
        HandwritingValidation.Id(query.ProfileId);
        var code = query.SystemCode is null ? null : CatalogQueryValidation.Code(query.SystemCode);
        var before = HandwritingValidation.Page(query.PageSize, query.Before);
        return repository.GetAttemptsAsync(current.UserId, query.ProfileId, code, query.Passed, query.PageSize, before, ct);
    }
}
