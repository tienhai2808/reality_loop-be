using Domain.Common;

namespace Domain.VocabularyLearning;

/// <summary>A scheduler decision; the scheduling algorithm belongs to the next application step.</summary>
public sealed class VocabularyReviewSchedule
{
    public VocabularyReviewSchedule(VocabularyLearningStatus status, int intervalMinutes, string schedulerVersion)
    {
        if (!Enum.IsDefined(status) || status == VocabularyLearningStatus.New)
            throw new ArgumentOutOfRangeException(nameof(status));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMinutes);
        Status = status;
        IntervalMinutes = intervalMinutes;
        SchedulerVersion = FoundationContentValidation.Code(schedulerVersion, nameof(schedulerVersion));
    }

    public VocabularyLearningStatus Status { get; }
    public int IntervalMinutes { get; }
    public string SchedulerVersion { get; }
}
