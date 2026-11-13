using Domain.GrammarPractice;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeOptionsDto(string LearningProfileId, string LanguageId, string LanguageCode,
    string ExplanationLanguage, IReadOnlyList<GrammarPracticePointOptionDto> GrammarPoints)
{
    public int MinQuestionCount => GrammarPracticeSession.MinQuestions;
    public int MaxQuestionCount => GrammarPracticeSession.MaxQuestions;
    public int DefaultQuestionCount => GrammarPracticeSession.MaxQuestions;
    public int AvailableQuestionCount => GrammarPoints.Sum(point => point.AvailableQuestionCount);
}
