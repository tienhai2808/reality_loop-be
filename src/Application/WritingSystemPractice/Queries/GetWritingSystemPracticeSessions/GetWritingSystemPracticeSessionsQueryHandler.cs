using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystemPractice.Models;
using Application.WritingSystemPractice.Validation;

namespace Application.WritingSystemPractice.Queries.GetWritingSystemPracticeSessions;

public sealed class GetWritingSystemPracticeSessionsQueryHandler(ICurrentUser current, IWritingSystemPracticeRepository repository)
{
    public Task<WritingSystemPracticeSessionPage> Handle(GetWritingSystemPracticeSessionsQuery query, CancellationToken ct)
    {
        WritingSystemPracticeValidation.Id(query.ProfileId);
        if (query.Status is { } status && !Enum.IsDefined(status))
            throw new RequestValidationException("Unknown practice session status.");
        var before = WritingSystemPracticeValidation.Page(query.PageSize, query.Before);
        var code = query.SystemCode is null ? null : CatalogQueryValidation.Code(query.SystemCode);
        return repository.GetPageAsync(current.UserId, query.ProfileId, code, query.Status, query.PageSize, before, ct);
    }
}
