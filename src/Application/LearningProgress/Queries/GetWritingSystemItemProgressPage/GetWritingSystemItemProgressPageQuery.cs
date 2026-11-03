using Application.LearningProgress.Models;

namespace Application.LearningProgress.Queries.GetWritingSystemItemProgressPage;

public sealed record GetWritingSystemItemProgressPageQuery(long ProfileId, string SystemCode,
    WritingSystemItemLearningState? Status = null, int PageSize = 100, string? After = null);
