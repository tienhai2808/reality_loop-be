using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyLearningPage(IReadOnlyList<VocabularyFlashcardDto> Items, string? NextCursor, DateTimeOffset AsOf);
