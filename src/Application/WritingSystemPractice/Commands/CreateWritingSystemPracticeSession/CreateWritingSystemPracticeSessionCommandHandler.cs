using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.WritingSystemPractice.Models;
using Application.WritingSystemPractice.Services;
using Application.WritingSystemPractice.Validation;
using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Commands.CreateWritingSystemPracticeSession;

public sealed class CreateWritingSystemPracticeSessionCommandHandler(ICurrentUser current,
    IWritingSystemPracticeRepository repository, IIdGenerator ids, TimeProvider clock)
{
    public async Task<WritingSystemPracticeSessionDto> Handle(CreateWritingSystemPracticeSessionCommand command, CancellationToken ct)
    {
        WritingSystemPracticeValidation.Id(command.ProfileId);
        WritingSystemPracticeValidation.QuestionCount(command.QuestionCount);
        var code = CatalogQueryValidation.Code(command.SystemCode);
        var catalog = await repository.GetCatalogAsync(current.UserId, command.ProfileId, code, ct)
            ?? throw new NotFoundException("Learning profile or published writing system was not found.");
        var questions = WritingSystemPracticeQuestionFactory.Create(catalog, command.QuestionCount, ids);
        var session = WritingSystemPracticeSession.Create(ids.NewId(), catalog.Profile, catalog.WritingSystem,
            catalog.Language, questions, clock.GetUtcNow());
        await repository.CreateAsync(current.UserId, session, ct);
        return WritingSystemPracticeSessionDto.From(session);
    }
}
