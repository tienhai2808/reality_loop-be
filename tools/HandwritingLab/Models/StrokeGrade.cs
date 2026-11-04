namespace HandwritingLab.Models;

public sealed record StrokeGrade(int Index, bool Passed, int MatchScore, double Distance, double LengthRatio,
    IReadOnlyList<HandwritingIssue> Issues);
