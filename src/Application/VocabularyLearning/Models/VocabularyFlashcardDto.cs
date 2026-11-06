using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyFlashcardDto(VocabularyProgressDto Progress, string VocabularyEntryId,
    string EntryCode, string Term, string? Reading, string? AudioUrl, string SenseCode, Domain.Vocabulary.PartOfSpeech PartOfSpeech,
    string? Meaning, string? UsageNote, string RequestedLocale, string? TranslationLocale, bool IsTranslationFallback);
