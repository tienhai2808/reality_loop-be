namespace Application.GrammarPractice.Commands.CreateGrammarPracticeSession;

public sealed record CreateGrammarPracticeSessionCommand(long ProfileId, string? GrammarPointCode = null,
    int QuestionCount = 10);
