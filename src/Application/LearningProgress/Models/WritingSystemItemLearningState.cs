namespace Application.LearningProgress.Models;

// NotStarted is a read-model state: no progress row exists for this profile and item.
public enum WritingSystemItemLearningState
{
    NotStarted,
    Learning,
    Completed
}
