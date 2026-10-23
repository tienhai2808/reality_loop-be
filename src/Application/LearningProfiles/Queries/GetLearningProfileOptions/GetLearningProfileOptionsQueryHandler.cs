using Application.LearningProfiles.Models;
using Domain.LearningProfiles;

namespace Application.LearningProfiles.Queries.GetLearningProfileOptions;

public sealed class GetLearningProfileOptionsQueryHandler
{
    public LearningProfileOptionsDto Handle(GetLearningProfileOptionsQuery query) =>
        new(Enum.GetValues<ProficiencyLevel>(), Enum.GetValues<LearningGoal>());
}
