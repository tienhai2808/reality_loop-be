using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Services;
using Application.VocabularyLearning.Validation;

namespace Application.VocabularyLearning.Commands.RecordVocabularyReview;

public sealed class RecordVocabularyReviewCommandHandler(ICurrentUser current, IVocabularyLearningRepository repository,
    VocabularyReviewScheduler scheduler, IIdGenerator ids, TimeProvider clock)
{
    public async Task<VocabularyReviewDto> Handle(RecordVocabularyReviewCommand command, CancellationToken ct)
    {
        VocabularyLearningValidation.Id(command.ProfileId);
        VocabularyLearningValidation.Id(command.ProgressId);
        if (command.RequestId == Guid.Empty || command.Version <= 0 || !Enum.IsDefined(command.Rating))
            throw new RequestValidationException("A non-empty request ID, positive version and valid rating are required.");
        var context = await repository.GetReviewContextAsync(current.UserId, command.ProfileId, command.ProgressId, command.RequestId, ct)
            ?? throw new NotFoundException("Vocabulary learning progress was not found.");
        if (context.ExistingReview is { } existing)
        {
            if (existing.ProgressVersionBefore != command.Version || existing.Rating != command.Rating)
                throw new ConflictException("The request ID was already used with a different rating or version.");
            return VocabularyReviewDto.From(existing);
        }
        if (!context.Available) throw new NotFoundException("The vocabulary learning card is no longer available.");
        if (context.Progress.Version != command.Version) throw new ConflictException("The learning progress changed. Reload this card.");
        var now = VocabularyLearningValidation.Timestamp(clock.GetUtcNow());
        if (now < context.Progress.UpdatedAt || context.Progress.Version == long.MaxValue)
            throw new ConflictException("The learning progress cannot be updated at this time.");
        var review = context.Progress.RecordReview(ids.NewId(), command.RequestId, command.Rating,
            scheduler.Schedule(context.Progress, command.Rating), now);
        return await repository.SaveReviewAsync(current.UserId, context.Progress, review, ct);
    }
}
