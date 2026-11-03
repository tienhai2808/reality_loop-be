using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeSessionSummaryDto(string Id, string LearningProfileId, string WritingSystemId,
    int QuestionCount, WritingSystemPracticeSessionStatus Status, int AnsweredCount, int CorrectCount,
    long Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? EndedAt)
{
    public int IncorrectCount => AnsweredCount - CorrectCount;
    public int UnansweredCount => QuestionCount - AnsweredCount;
    public decimal? AccuracyPercentage => AnsweredCount == 0 ? null : Math.Round(CorrectCount * 100m / AnsweredCount, 2);

    public static WritingSystemPracticeSessionSummaryDto From(WritingSystemPracticeSession session) => new(
        session.Id.ToString(), session.LearningProfileId.ToString(), session.WritingSystemId.ToString(),
        session.QuestionCount, session.Status, session.Items.Count(x => x.AnsweredAt.HasValue),
        session.Items.Count(x => x.IsCorrect == true), session.Version, session.CreatedAt, session.UpdatedAt, session.EndedAt);
}
