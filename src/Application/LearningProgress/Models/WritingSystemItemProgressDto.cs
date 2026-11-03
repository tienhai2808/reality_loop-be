using Domain.LearningProgress;

namespace Application.LearningProgress.Models;

public sealed record WritingSystemItemProgressDto(string LearningProfileId, string WritingSystemItemId,
    string ItemCode, int SortOrder, WritingSystemItemLearningState Status, long Version,
    DateTimeOffset? LastStudiedAt, DateTimeOffset? CompletedAt, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)
{
    public static WritingSystemItemProgressDto From(WritingSystemItemProgress progress, string itemCode, int sortOrder) =>
        new(progress.LearningProfileId.ToString(), progress.WritingSystemItemId.ToString(), itemCode, sortOrder,
            progress.Status == WritingSystemItemProgressStatus.Completed
                ? WritingSystemItemLearningState.Completed : WritingSystemItemLearningState.Learning,
            progress.Version, progress.LastStudiedAt, progress.CompletedAt, progress.CreatedAt, progress.UpdatedAt);
}
