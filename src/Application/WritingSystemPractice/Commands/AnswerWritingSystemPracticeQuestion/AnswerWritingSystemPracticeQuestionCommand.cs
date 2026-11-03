namespace Application.WritingSystemPractice.Commands.AnswerWritingSystemPracticeQuestion;

public sealed record AnswerWritingSystemPracticeQuestionCommand(long ProfileId, long SessionId, long SessionItemId,
    int SelectedOptionIndex, long Version);
