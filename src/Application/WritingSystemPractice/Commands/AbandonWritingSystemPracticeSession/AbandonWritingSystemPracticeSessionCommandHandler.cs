using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WritingSystemPractice.Models;
using Application.WritingSystemPractice.Validation;

namespace Application.WritingSystemPractice.Commands.AbandonWritingSystemPracticeSession;

public sealed class AbandonWritingSystemPracticeSessionCommandHandler(ICurrentUser current,
    IWritingSystemPracticeRepository repository, TimeProvider clock)
{
    public async Task<WritingSystemPracticeSessionDto> Handle(AbandonWritingSystemPracticeSessionCommand command, CancellationToken ct)
    {
        WritingSystemPracticeValidation.Id(command.ProfileId);
        WritingSystemPracticeValidation.Id(command.SessionId);
        WritingSystemPracticeValidation.Version(command.Version);
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
        return WritingSystemPracticeSessionDto.From(session);
    }
}
