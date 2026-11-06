using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Queries.GetVocabularyLearningPage;

public sealed record GetVocabularyLearningPageQuery(long ProfileId, bool DueOnly = true, VocabularyLearningStatus? Status = null, int PageSize = 20, string? After = null, string? Locale = null);
