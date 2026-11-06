using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyProgressCreationResult(VocabularyProgressDto Progress, bool Created);
