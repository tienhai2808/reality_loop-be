namespace Application.LearningProgress.Models;

public sealed record WritingSystemProgressSummaryDto(string WritingSystemId, string SystemCode, int TotalItems,
    int NotStartedItems, int LearningItems, int CompletedItems, DateTimeOffset? LastStudiedAt)
{
    public decimal CompletionPercentage => TotalItems == 0 ? 0 : Math.Round(100m * CompletedItems / TotalItems, 2);
}
