using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Validation;

namespace Application.GrammarPractice.Queries.GetGrammarPracticeSessions;

public sealed class GetGrammarPracticeSessionsQueryHandler(ICurrentUser current, IGrammarPracticeRepository repository)
{
    public Task<GrammarPracticeSessionPage> Handle(GetGrammarPracticeSessionsQuery query, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(query.ProfileId);
        if (query.Status is { } status && !Enum.IsDefined(status))
            throw new RequestValidationException("Unknown grammar practice session status.");
        var before = GrammarPracticeValidation.Page(query.PageSize, query.Before);
        var code = query.GrammarPointCode is null ? null : CatalogQueryValidation.Code(query.GrammarPointCode);
        return repository.GetPageAsync(current.UserId, query.ProfileId, code, query.Status, query.PageSize, before, ct);
    }
}
