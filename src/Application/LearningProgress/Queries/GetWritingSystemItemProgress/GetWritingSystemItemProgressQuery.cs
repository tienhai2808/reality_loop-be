namespace Application.LearningProgress.Queries.GetWritingSystemItemProgress;

public sealed record GetWritingSystemItemProgressQuery(long ProfileId, string SystemCode, long ItemId);
