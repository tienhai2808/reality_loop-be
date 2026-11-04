namespace HandwritingLab.Models;

public sealed record CharacterGrade(string Symbol, bool Passed, int MatchScore, int ExpectedStrokeCount,
    int ActualStrokeCount, IReadOnlyList<HandwritingIssue> Issues, IReadOnlyList<StrokeGrade> Strokes);
