using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyReviewContext(VocabularyLearningProgress Progress, bool Available, VocabularyReview? ExistingReview);
