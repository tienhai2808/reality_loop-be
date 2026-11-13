using Domain.GrammarPractice;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeSessionDto(GrammarPracticeSessionSummaryDto Session,
    IReadOnlyList<GrammarPracticeQuestionDto> Items)
{
    public static GrammarPracticeSessionDto From(GrammarPracticeSession session) => new(
        GrammarPracticeSessionSummaryDto.From(session), session.Items.OrderBy(item => item.Position)
            .Select(item => GrammarPracticeQuestionDto.From(item, session.ExplanationLanguage)).ToArray());
}
