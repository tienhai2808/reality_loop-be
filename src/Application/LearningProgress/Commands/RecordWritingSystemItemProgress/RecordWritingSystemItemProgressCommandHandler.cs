using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.LearningProgress.Models;
using Application.LearningProgress.Validation;
using Domain.LearningProgress;

namespace Application.LearningProgress.Commands.RecordWritingSystemItemProgress;

public sealed class RecordWritingSystemItemProgressCommandHandler(ICurrentUser current,
    IWritingSystemItemProgressRepository repository, IIdGenerator ids, TimeProvider clock)
{
    public async Task<WritingSystemItemProgressDto> Handle(RecordWritingSystemItemProgressCommand command, CancellationToken ct)
    {
        LearningProgressValidation.Id(command.ProfileId);
        LearningProgressValidation.Id(command.ItemId);
        LearningProgressValidation.Version(command.Version);
        if (!Enum.IsDefined(command.Action)) throw new RequestValidationException("Unknown learning progress action.");
        var code = CatalogQueryValidation.Code(command.SystemCode);
        var context = await repository.FindContextAsync(current.UserId, command.ProfileId, code, command.ItemId, ct)
            ?? throw new NotFoundException("Learning profile or published writing system item was not found.");
        if ((context.Progress?.Version ?? 0) != command.Version)
            throw new ConflictException("The learning progress changed. Reload this item before retrying.");
        if (context.Progress is null && command.Action == WritingSystemItemProgressAction.Restart)
            throw new RequestValidationException("An item that has not been started cannot be restarted.");

        var now = clock.GetUtcNow();
        if (context.Progress is { } existing && now < existing.UpdatedAt) now = existing.UpdatedAt;
        WritingSystemItemProgress progress;
        try
        {
            progress = context.Progress ?? WritingSystemItemProgress.Create(ids.NewId(), context.Profile,
                context.Item, context.WritingSystem, context.Language, now);
            switch (command.Action)
            {
                case WritingSystemItemProgressAction.Study: progress.RecordStudy(now); break;
                case WritingSystemItemProgressAction.Complete: progress.MarkCompleted(now); break;
                case WritingSystemItemProgressAction.Restart: progress.RestartLearning(now); break;
            }
        }
        catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }
        catch (OverflowException) { throw new ConflictException("The learning progress version cannot be advanced."); }

        await repository.SaveAsync(current.UserId, code, progress, command.Version, ct);
        return WritingSystemItemProgressDto.From(progress, context.Item.Code, context.Item.SortOrder);
    }
}
