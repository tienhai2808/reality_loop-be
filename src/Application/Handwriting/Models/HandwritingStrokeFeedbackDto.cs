using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingStrokeFeedbackDto(int Index, bool Passed, int MatchScore, double Distance,
    double LengthRatio, IReadOnlyList<HandwritingIssue> Issues);
