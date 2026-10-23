using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Application.LearningProfiles.Validation;

namespace Application.LearningProfiles.Commands.SetCurrentLearningProfile;

public sealed class SetCurrentLearningProfileCommandHandler(ICurrentUser current,
    ILearningProfileRepository profiles, TimeProvider clock)
{
    public Task<CurrentLearningProfileDto> Handle(SetCurrentLearningProfileCommand command, CancellationToken ct) =>
        profiles.SetCurrentAsync(current.UserId, LearningProfileValidation.ParseId(command.ProfileId), clock.GetUtcNow(), ct);
}
