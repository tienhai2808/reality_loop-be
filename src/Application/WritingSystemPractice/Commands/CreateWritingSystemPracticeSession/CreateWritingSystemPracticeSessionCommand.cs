namespace Application.WritingSystemPractice.Commands.CreateWritingSystemPracticeSession;

public sealed record CreateWritingSystemPracticeSessionCommand(long ProfileId, string SystemCode, int QuestionCount = 10);
