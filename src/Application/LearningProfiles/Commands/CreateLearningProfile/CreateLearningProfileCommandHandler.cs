using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Domain.LearningProfiles;

namespace Application.LearningProfiles.Commands.CreateLearningProfile;

public sealed class CreateLearningProfileCommandHandler(ICurrentUser current, ILearningProfileRepository profiles,
    IIdGenerator ids, TimeProvider clock)
{
    public async Task<LearningProfileDto> Handle(CreateLearningProfileCommand command, CancellationToken ct)
    {
        LearningProfile profile;
        try
        {
            profile = LearningProfile.Create(ids.NewId(), current.UserId, command.TargetLanguage,
                command.NativeLanguage, command.ExplanationLanguage, command.ProficiencyLevel,
                command.PrimaryGoal, command.GoalNote, clock.GetUtcNow());
        }
        catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }

        await profiles.CreateAsync(profile, ct);
        return LearningProfileDto.From(profile);
    }
}
