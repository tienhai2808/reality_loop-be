using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Handwriting.Models;
using Application.Handwriting.Validation;

namespace Application.Handwriting.Queries.GetHandwritingAttempt;

public sealed class GetHandwritingAttemptQueryHandler(ICurrentUser current, IHandwritingRepository repository)
{
    public async Task<HandwritingAttemptDto> Handle(GetHandwritingAttemptQuery query, CancellationToken ct)
    {
        HandwritingValidation.Id(query.ProfileId);
        HandwritingValidation.Id(query.AttemptId);
        return await repository.GetAttemptAsync(current.UserId, query.ProfileId, query.AttemptId, ct)
            ?? throw new NotFoundException("Handwriting attempt was not found.");
    }
}
