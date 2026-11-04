namespace Domain.Handwriting;

/// <summary>A server-produced evaluation, not a client assertion or a recognition probability.</summary>
public sealed class HandwritingFeedback
{
    public const int FormatVersion = 1;

    private HandwritingFeedback(int expectedStrokeCount, int actualStrokeCount,
        IReadOnlyList<HandwritingIssue> issues, IReadOnlyList<HandwritingStrokeFeedback> strokes)
    {
        ExpectedStrokeCount = expectedStrokeCount;
        ActualStrokeCount = actualStrokeCount;
        Issues = issues;
        Strokes = strokes;
    }

    public int ExpectedStrokeCount { get; }
    public int ActualStrokeCount { get; }
    public IReadOnlyList<HandwritingIssue> Issues { get; }
    public IReadOnlyList<HandwritingStrokeFeedback> Strokes { get; }
    public bool Passed => Issues.Count == 0;
    public int MatchScore => Strokes.Count == 0 ? 0 : (int)Math.Round(Strokes.Average(x => x.MatchScore));

    public static HandwritingFeedback Rejected(int expectedStrokeCount, int actualStrokeCount, HandwritingIssue issue)
    {
        ValidateCounts(expectedStrokeCount, actualStrokeCount);
        var valid = issue switch
        {
            HandwritingIssue.MissingDrawing => actualStrokeCount == 0,
            HandwritingIssue.WrongStrokeCount => actualStrokeCount > 0 && actualStrokeCount != expectedStrokeCount,
            HandwritingIssue.DrawingTooSmall => actualStrokeCount == expectedStrokeCount,
            _ => false
        };
        if (!valid) throw new ArgumentException("The rejection must match the drawing's stroke counts.", nameof(issue));
        return new(expectedStrokeCount, actualStrokeCount, Array.AsReadOnly(new[] { issue }),
            Array.AsReadOnly(Array.Empty<HandwritingStrokeFeedback>()));
    }

    public static HandwritingFeedback Evaluated(int expectedStrokeCount, IEnumerable<HandwritingStrokeFeedback> strokes)
    {
        ValidateCounts(expectedStrokeCount, expectedStrokeCount);
        ArgumentNullException.ThrowIfNull(strokes);
        var copy = strokes.Take(InkDrawing.MaxStrokes + 1).ToArray();
        if (copy.Length != expectedStrokeCount || copy.Where((x, i) => x is null || x.Index != i).Any())
            throw new ArgumentException("Feedback must contain every stroke, in order, exactly once.", nameof(strokes));
        return new(expectedStrokeCount, expectedStrokeCount,
            Array.AsReadOnly(copy.SelectMany(x => x.Issues).Distinct().ToArray()), Array.AsReadOnly(copy));
    }

    private static void ValidateCounts(int expected, int actual)
    {
        if (expected is < 1 or > InkDrawing.MaxStrokes) throw new ArgumentOutOfRangeException(nameof(expected));
        if (actual is < 0 or > InkDrawing.MaxStrokes) throw new ArgumentOutOfRangeException(nameof(actual));
    }
}
