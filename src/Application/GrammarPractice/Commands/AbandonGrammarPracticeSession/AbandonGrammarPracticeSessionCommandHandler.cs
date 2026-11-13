using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Validation;

namespace Application.GrammarPractice.Commands.AbandonGrammarPracticeSession;

public sealed class AbandonGrammarPracticeSessionCommandHandler(ICurrentUser current,
    IGrammarPracticeRepository repository, TimeProvider clock)
{
    public async Task<GrammarPracticeSessionDto> Handle(AbandonGrammarPracticeSessionCommand command, CancellationToken ct)
    {
        GrammarPracticeValidation.Id(command.ProfileId);
        GrammarPracticeValidation.Id(command.SessionId);
        GrammarPracticeValidation.Version(command.Version);
        var session = await repository.FindOwnedAsync(current.UserId, command.ProfileId, command.SessionId, ct)
            ?? throw new NotFoundException("Practice session was not found.");
        if (session.Version != command.Version)
            throw new ConflictException("The practice session changed. Reload it before retrying.");
        var now = clock.GetUtcNow();
        if (now < session.UpdatedAt) now = session.UpdatedAt;
        try { session.Abandon(now); }
        catch (InvalidOperationException ex) { throw new ConflictException(ex.Message); }
        catch (OverflowException) { throw new ConflictException("The practice session version cannot be incremented."); }
        await repository.SaveAbandonAsync(current.UserId, session, command.Version, ct);
        return GrammarPracticeSessionDto.From(session);
    }
}
