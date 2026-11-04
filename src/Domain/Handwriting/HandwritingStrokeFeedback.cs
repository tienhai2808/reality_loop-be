namespace Domain.Handwriting;

public sealed class HandwritingStrokeFeedback
{
    public HandwritingStrokeFeedback(int index, int matchScore, double distance, double lengthRatio,
        IEnumerable<HandwritingIssue> issues)
    {
        if (index is < 0 or >= InkDrawing.MaxStrokes) throw new ArgumentOutOfRangeException(nameof(index));
        if (matchScore is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(matchScore));
        if (!double.IsFinite(distance) || distance < 0) throw new ArgumentOutOfRangeException(nameof(distance));
        if (!double.IsFinite(lengthRatio) || lengthRatio < 0) throw new ArgumentOutOfRangeException(nameof(lengthRatio));
        ArgumentNullException.ThrowIfNull(issues);
        var values = issues.Take(Enum.GetValues<HandwritingIssue>().Length + 1).ToArray();
        if (values.Any(x => !Enum.IsDefined(x) || x is HandwritingIssue.MissingDrawing or
                HandwritingIssue.WrongStrokeCount or HandwritingIssue.DrawingTooSmall) ||
            values.Distinct().Count() != values.Length)
            throw new ArgumentException("Stroke issues must be distinct, defined stroke-level issues.", nameof(issues));
        Index = index;
        MatchScore = matchScore;
        Distance = distance;
        LengthRatio = lengthRatio;
        Issues = Array.AsReadOnly(values);
    }

    public int Index { get; }
    public int MatchScore { get; }
    public double Distance { get; }
    public double LengthRatio { get; }
    public IReadOnlyList<HandwritingIssue> Issues { get; }
    public bool Passed => Issues.Count == 0;
}
