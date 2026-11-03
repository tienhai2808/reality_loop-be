namespace Application.WritingSystemPractice.Commands.AbandonWritingSystemPracticeSession;

public sealed record AbandonWritingSystemPracticeSessionCommand(long ProfileId, long SessionId, long Version);
