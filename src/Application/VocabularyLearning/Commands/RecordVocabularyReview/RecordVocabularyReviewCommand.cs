using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Commands.RecordVocabularyReview;

public sealed record RecordVocabularyReviewCommand(long ProfileId, long ProgressId, Guid RequestId, long Version, VocabularyReviewRating Rating);
