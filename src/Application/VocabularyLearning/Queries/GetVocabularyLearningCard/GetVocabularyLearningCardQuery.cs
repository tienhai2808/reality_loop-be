using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Queries.GetVocabularyLearningCard;

public sealed record GetVocabularyLearningCardQuery(long ProfileId, long ProgressId, string? Locale = null);
