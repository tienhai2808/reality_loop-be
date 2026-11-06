using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Queries.GetVocabularyReviewHistory;

public sealed record GetVocabularyReviewHistoryQuery(long ProfileId, long ProgressId, int PageSize = 20, string? Before = null);
