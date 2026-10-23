using Domain.LearningProfiles;

namespace Application.LearningProfiles.Models;

public sealed record LearningProfileOptionsDto(IReadOnlyList<ProficiencyLevel> ProficiencyLevels,
    IReadOnlyList<LearningGoal> LearningGoals);
