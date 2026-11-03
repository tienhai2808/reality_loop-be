namespace Application.LearningProgress.Models;

public sealed record LearningProgressSummaryDto(string LearningProfileId, string TargetLanguage,
    IReadOnlyList<WritingSystemProgressSummaryDto> WritingSystems);
