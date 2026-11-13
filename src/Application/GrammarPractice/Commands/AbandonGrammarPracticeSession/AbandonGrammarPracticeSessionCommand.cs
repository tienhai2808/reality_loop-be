namespace Application.GrammarPractice.Commands.AbandonGrammarPracticeSession;

public sealed record AbandonGrammarPracticeSessionCommand(long ProfileId, long SessionId, long Version);
