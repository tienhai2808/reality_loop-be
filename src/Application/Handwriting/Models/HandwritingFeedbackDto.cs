using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingFeedbackDto(int ExpectedStrokeCount, int ActualStrokeCount,
    IReadOnlyList<HandwritingIssue> Issues, IReadOnlyList<HandwritingStrokeFeedbackDto> Strokes, bool Passed, int MatchScore)
{
    public static HandwritingFeedbackDto From(HandwritingFeedback feedback) => new(feedback.ExpectedStrokeCount,
        feedback.ActualStrokeCount, feedback.Issues, feedback.Strokes.Select(x => new HandwritingStrokeFeedbackDto(
            x.Index, x.Passed, x.MatchScore, x.Distance, x.LengthRatio, x.Issues)).ToArray(), feedback.Passed, feedback.MatchScore);
}
