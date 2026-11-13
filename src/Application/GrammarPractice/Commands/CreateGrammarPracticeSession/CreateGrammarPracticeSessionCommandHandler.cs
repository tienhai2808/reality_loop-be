using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Services;
using Application.GrammarPractice.Validation;
using Domain.GrammarPractice;

namespace Application.GrammarPractice.Commands.CreateGrammarPracticeSession;

public sealed class CreateGrammarPracticeSessionCommandHandler(ICurrentUser current,
    IGrammarPracticeRepository repository, IIdGenerator ids, TimeProvider clock)
{
    public async Task<GrammarPracticeSessionDto> Handle(CreateGrammarPracticeSessionCommand command, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(command.ProfileId);
        GrammarPracticeValidation.QuestionCount(command.QuestionCount);
        var code = command.GrammarPointCode is null ? null : CatalogQueryValidation.Code(command.GrammarPointCode);
        var context = await repository.GetContextAsync(current.UserId, command.ProfileId, code, ct)
            ?? throw new NotFoundException("Learning profile, enabled language or published grammar point was not found.");
        var candidates = await repository.SampleAsync(context, command.QuestionCount, ct);
        var questions = GrammarPracticeQuestionFactory.Create(context, candidates, command.QuestionCount, ids);
        var now = clock.GetUtcNow();
        var sourceUpdatedAt = questions.Max(question => question.SourceUpdatedAt);
        if (now < sourceUpdatedAt) now = sourceUpdatedAt;
        if (now < context.Profile.UpdatedAt) now = context.Profile.UpdatedAt;
        var session = GrammarPracticeSession.Create(ids.NewId(), context.Profile, context.Language, questions, now,
            context.SelectedPoint);
        await repository.CreateAsync(current.UserId, session, ct);
        return GrammarPracticeSessionDto.From(session);
    }
}
