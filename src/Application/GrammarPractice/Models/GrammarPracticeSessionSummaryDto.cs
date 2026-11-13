using Domain.GrammarPractice;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeSessionSummaryDto(string Id, string LearningProfileId, string LanguageId,
    string? GrammarPointId, string ExplanationLanguage, int QuestionCount, GrammarPracticeSessionStatus Status,
    int AnsweredCount, int CorrectCount, long Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt,
    DateTimeOffset? EndedAt)
{
    public int IncorrectCount => AnsweredCount - CorrectCount;
    public int UnansweredCount => QuestionCount - AnsweredCount;
    public decimal? AccuracyPercentage => AnsweredCount == 0 ? null : Math.Round(CorrectCount * 100m / AnsweredCount, 2);

    public static GrammarPracticeSessionSummaryDto From(GrammarPracticeSession session) => new(
        session.Id.ToString(), session.LearningProfileId.ToString(), session.LanguageId.ToString(),
        session.GrammarPointId?.ToString(), session.ExplanationLanguage, session.QuestionCount, session.Status,
        session.Items.Count(item => item.AnsweredAt.HasValue), session.Items.Count(item => item.IsCorrect == true),
        session.Version, session.CreatedAt, session.UpdatedAt, session.EndedAt);
}
