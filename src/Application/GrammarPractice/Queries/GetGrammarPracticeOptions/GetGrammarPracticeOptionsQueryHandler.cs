using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Validation;

namespace Application.GrammarPractice.Queries.GetGrammarPracticeOptions;

public sealed class GetGrammarPracticeOptionsQueryHandler(ICurrentUser current, IGrammarPracticeRepository repository)
{
    public async Task<GrammarPracticeOptionsDto> Handle(GetGrammarPracticeOptionsQuery query, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(query.ProfileId);
        return await repository.GetOptionsAsync(current.UserId, query.ProfileId, ct)
            ?? throw new NotFoundException("Learning profile or enabled language was not found.");
    }
}
