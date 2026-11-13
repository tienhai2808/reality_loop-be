using Domain.Languages;
using Domain.WritingSystems;
using Domain.Vocabulary;
using Domain.Grammar;
using Domain.Pronunciation;
using Domain.Handwriting;

namespace Infrastructure.Persistence.Seeding;

internal static class CatalogSeedData
{
    public static readonly Language[] LanguageRows = [.. Japanese.FoundationSeedData.LanguageRows, .. German.FoundationSeedData.LanguageRows];
    public static readonly LanguageTranslation[] LanguageTranslationRows = [.. Japanese.FoundationSeedData.LanguageTranslationRows, .. German.FoundationSeedData.LanguageTranslationRows];
    public static readonly WritingSystem[] WritingSystemRows = [.. Japanese.FoundationSeedData.WritingSystemRows, .. German.FoundationSeedData.WritingSystemRows];
    public static readonly WritingSystemTranslation[] WritingSystemTranslationRows = [.. Japanese.FoundationSeedData.WritingSystemTranslationRows, .. German.FoundationSeedData.WritingSystemTranslationRows];
    public static readonly WritingSystemItem[] WritingSystemItemRows = [.. Japanese.FoundationSeedData.WritingSystemItemRows, .. German.FoundationSeedData.WritingSystemItemRows];
    public static readonly WritingSystemItemReading[] WritingSystemItemReadingRows = [.. Japanese.FoundationSeedData.WritingSystemItemReadingRows, .. German.FoundationSeedData.WritingSystemItemReadingRows];
    public static readonly VocabularyEntry[] VocabularyEntryRows = [.. Japanese.VocabularySeedData.EntryRows, .. German.VocabularySeedData.EntryRows];
    public static readonly VocabularySense[] VocabularySenseRows = [.. Japanese.VocabularySeedData.SenseRows, .. German.VocabularySeedData.SenseRows];
    public static readonly VocabularySenseTranslation[] VocabularySenseTranslationRows = [.. Japanese.VocabularySeedData.TranslationRows, .. German.VocabularySeedData.TranslationRows];
    public static readonly GrammarPoint[] GrammarPointRows = [.. Japanese.GrammarSeedData.PointRows, .. German.GrammarSeedData.PointRows];
    public static readonly GrammarPointTranslation[] GrammarPointTranslationRows = [.. Japanese.GrammarSeedData.TranslationRows, .. German.GrammarSeedData.TranslationRows];
    public static readonly PronunciationRule[] PronunciationRuleRows = [.. Japanese.PronunciationSeedData.RuleRows, .. German.PronunciationSeedData.RuleRows];
    public static readonly PronunciationRuleTranslation[] PronunciationRuleTranslationRows = [.. Japanese.PronunciationSeedData.TranslationRows, .. German.PronunciationSeedData.TranslationRows];
    public static readonly PronunciationExample[] PronunciationExampleRows = [.. Japanese.PronunciationSeedData.ExampleRows, .. German.PronunciationSeedData.ExampleRows];
    public static readonly HandwritingTemplate[] HandwritingTemplateRows = [.. Japanese.HandwritingSeedData.Templates, .. German.HandwritingSeedData.Templates];
}
