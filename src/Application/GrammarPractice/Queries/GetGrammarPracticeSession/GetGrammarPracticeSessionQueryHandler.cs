using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Validation;

namespace Application.GrammarPractice.Queries.GetGrammarPracticeSession;

public sealed class GetGrammarPracticeSessionQueryHandler(ICurrentUser current, IGrammarPracticeRepository repository)
{
    public async Task<GrammarPracticeSessionDto> Handle(GetGrammarPracticeSessionQuery query, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(query.ProfileId);
        GrammarPracticeValidation.Id(query.SessionId);
        var session = await repository.FindOwnedAsync(current.UserId, query.ProfileId, query.SessionId, ct)
            ?? throw new NotFoundException("Practice session was not found.");
        return GrammarPracticeSessionDto.From(session);
    }
}
