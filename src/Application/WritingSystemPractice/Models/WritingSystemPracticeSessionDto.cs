using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeSessionDto(WritingSystemPracticeSessionSummaryDto Session,
    IReadOnlyList<WritingSystemPracticeQuestionDto> Items)
{
    public static WritingSystemPracticeSessionDto From(WritingSystemPracticeSession session) => new(
        WritingSystemPracticeSessionSummaryDto.From(session),
        session.Items.OrderBy(x => x.Position).Select(WritingSystemPracticeQuestionDto.From).ToArray());
}
