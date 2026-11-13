namespace Infrastructure.Persistence.Seeding.GrammarPractice;

internal sealed record GrammarExerciseSeedDefinition(int Key, string GrammarPointCode, string Prompt,
    string[] Options, int CorrectOptionIndex, string VietnameseInstruction, string EnglishInstruction,
    string VietnameseExplanation, string EnglishExplanation);
