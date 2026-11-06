using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyReviewPage(IReadOnlyList<VocabularyReviewDto> Items, string? NextCursor);
