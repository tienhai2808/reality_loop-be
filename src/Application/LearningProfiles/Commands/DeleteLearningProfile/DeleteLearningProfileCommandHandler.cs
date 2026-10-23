using Application.Common.Interfaces;
using Application.LearningProfiles.Validation;

namespace Application.LearningProfiles.Commands.DeleteLearningProfile;

public sealed class DeleteLearningProfileCommandHandler(ICurrentUser current, ILearningProfileRepository profiles)
{
    public Task Handle(DeleteLearningProfileCommand command, CancellationToken ct)
    {
        LearningProfileValidation.Id(command.Id);
        LearningProfileValidation.Version(command.Version);
        return profiles.DeleteAsync(current.UserId, command.Id, command.Version, ct);
    }
}
