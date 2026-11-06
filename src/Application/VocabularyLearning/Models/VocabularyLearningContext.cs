using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyLearningContext(Domain.LearningProfiles.LearningProfile Profile,
    Domain.Languages.Language Language, Domain.Vocabulary.VocabularyEntry Entry, Domain.Vocabulary.VocabularySense Sense,
    VocabularyLearningProgress? Progress);
