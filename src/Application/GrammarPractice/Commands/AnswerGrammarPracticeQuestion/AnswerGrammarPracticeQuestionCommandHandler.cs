using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Validation;
using Domain.GrammarPractice;

namespace Application.GrammarPractice.Commands.AnswerGrammarPracticeQuestion;

public sealed class AnswerGrammarPracticeQuestionCommandHandler(ICurrentUser current,
    IGrammarPracticeRepository repository, TimeProvider clock)
{
    public async Task<GrammarPracticeSessionDto> Handle(AnswerGrammarPracticeQuestionCommand command, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(command.ProfileId);
        GrammarPracticeValidation.Id(command.SessionId);
        GrammarPracticeValidation.Id(command.SessionItemId);
        GrammarPracticeValidation.Version(command.Version);
        if (command.SelectedOptionIndex is < 0 or >= GrammarExercise.MaxOptions)
            throw new RequestValidationException("Selected option index must be between 0 and 3.");
        var session = await repository.FindOwnedAsync(current.UserId, command.ProfileId, command.SessionId, ct)
            ?? throw new NotFoundException("Practice session was not found.");
        if (session.Version != command.Version)
            throw new ConflictException("The practice session changed. Reload it before retrying.");
        var item = session.Items.SingleOrDefault(x => x.Id == command.SessionItemId)
            ?? throw new NotFoundException("Practice question was not found in this session.");
        var now = clock.GetUtcNow();
        if (now < session.UpdatedAt) now = session.UpdatedAt;
        try { session.Answer(item.Id, command.SelectedOptionIndex, now); }
        catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }
        catch (InvalidOperationException ex) { throw new ConflictException(ex.Message); }
        catch (OverflowException) { throw new ConflictException("The practice session version cannot be incremented."); }
        await repository.SaveAnswerAsync(current.UserId, session, item, command.Version, ct);
        return GrammarPracticeSessionDto.From(session);
    }
}
