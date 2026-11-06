using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Validation;
using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Commands.AddVocabularyLearning;

public sealed class AddVocabularyLearningCommandHandler(ICurrentUser current, IVocabularyLearningRepository repository,
    IIdGenerator ids, TimeProvider clock)
{
    public async Task<VocabularyProgressCreationResult> Handle(AddVocabularyLearningCommand command, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(command.ProfileId);
        VocabularyLearningValidation.Id(command.SenseId);
        var context = await repository.GetAddContextAsync(current.UserId, command.ProfileId, command.SenseId, ct)
            ?? throw new NotFoundException("Learning profile or published vocabulary sense was not found.");
        if (context.Progress is not null) return new(VocabularyProgressDto.From(context.Progress), false);
        var progress = VocabularyLearningProgress.Create(ids.NewId(), context.Profile, context.Sense, context.Entry,
            context.Language, VocabularyLearningValidation.Timestamp(clock.GetUtcNow()));
        return await repository.CreateAsync(current.UserId, progress, ct);
    }
}
