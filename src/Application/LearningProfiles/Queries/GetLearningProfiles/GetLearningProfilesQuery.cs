namespace Application.LearningProfiles.Queries.GetLearningProfiles;

public sealed record GetLearningProfilesQuery(int PageSize = 20, string? After = null);
