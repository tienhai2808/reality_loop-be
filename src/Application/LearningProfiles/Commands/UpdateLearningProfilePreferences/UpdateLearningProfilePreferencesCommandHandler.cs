using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Application.LearningProfiles.Validation;

namespace Application.LearningProfiles.Commands.UpdateLearningProfilePreferences;

public sealed class UpdateLearningProfilePreferencesCommandHandler(ICurrentUser current,
    ILearningProfileRepository profiles, TimeProvider clock)
{
    public async Task<LearningProfileDto> Handle(UpdateLearningProfilePreferencesCommand command, CancellationToken ct)
    {
        LearningProfileValidation.Id(command.Id);
        LearningProfileValidation.Version(command.Version);
        var profile = await profiles.FindOwnedAsync(current.UserId, command.Id, ct)
            ?? throw new NotFoundException("Learning profile was not found.");
        if (profile.Version != command.Version)
            throw new ConflictException("The learning profile changed. Reload it before updating.");

        var now = clock.GetUtcNow();
        if (now < profile.UpdatedAt) now = profile.UpdatedAt;
        try
        {
            profile.UpdatePreferences(command.NativeLanguage, command.ExplanationLanguage,
                command.ProficiencyLevel, command.PrimaryGoal, command.GoalNote, now);
        }
        catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }

        await profiles.UpdatePreferencesAsync(profile, command.Version, ct);
        return LearningProfileDto.From(profile);
    }
}
