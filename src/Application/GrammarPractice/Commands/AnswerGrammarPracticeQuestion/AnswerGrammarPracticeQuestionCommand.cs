namespace Application.GrammarPractice.Commands.AnswerGrammarPracticeQuestion;

public sealed record AnswerGrammarPracticeQuestionCommand(long ProfileId, long SessionId, long SessionItemId,
    int SelectedOptionIndex, long Version);
