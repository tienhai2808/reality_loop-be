using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyLearningSummaryDto(string LearningProfileId, int Total, int New, int Learning,
    int Reviewing, int Due, DateTimeOffset? NextFutureReviewAt, DateTimeOffset AsOf);
