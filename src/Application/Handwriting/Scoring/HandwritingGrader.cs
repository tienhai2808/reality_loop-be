using Domain.Handwriting;

namespace Application.Handwriting.Scoring;

public sealed class HandwritingGrader(HandwritingScoringPolicy policy)
{
    public HandwritingFeedback Grade(InkDrawing reference, InkDrawing drawing, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(drawing);
        var strokes = drawing.Strokes;
        if (reference.Strokes.Count == 0 || reference.Strokes.Any(x => x.Count < 2 || !x.Any(p => p != x[0])))
            throw new ArgumentException("A non-degenerate reference drawing is required.", nameof(reference));
        ct.ThrowIfCancellationRequested();
        HandwritingFeedback Failed(HandwritingIssue issue) =>
            HandwritingFeedback.Rejected(reference.Strokes.Count, strokes.Count, issue);
        if (strokes.Count == 0) return Failed(HandwritingIssue.MissingDrawing);
        if (strokes.Count != reference.Strokes.Count) return Failed(HandwritingIssue.WrongStrokeCount);
        if (StrokeGeometry.Extent(strokes) < policy.MinimumDrawingExtent) return Failed(HandwritingIssue.DrawingTooSmall);

        var expected = StrokeGeometry.Normalize(reference.Strokes).Select(x => StrokeGeometry.Simplify(x, policy.SimplificationTolerance)).ToArray();
        var drawn = StrokeGeometry.Normalize(strokes).Select(x => StrokeGeometry.Simplify(x, policy.SimplificationTolerance)).ToArray();
        var expectedSamples = expected.Select(x => StrokeGeometry.Resample(x, policy.SamplesPerStroke)).ToArray();
        var drawnSamples = drawn.Select(x => StrokeGeometry.Resample(x, policy.SamplesPerStroke)).ToArray();
        var count = strokes.Count;
        var forward = new double[count, count];
        var backward = new double[count, count];
        for (var i = 0; i < count; i++)
        {
            ct.ThrowIfCancellationRequested();
            var reverse = drawnSamples[i].Reverse().ToArray();
            for (var j = 0; j < count; j++)
            {
                forward[i, j] = StrokeGeometry.Frechet(drawnSamples[i], expectedSamples[j]);
                backward[i, j] = StrokeGeometry.Frechet(reverse, expectedSamples[j]);
            }
        }
        var reordered = Enumerable.Range(0, count).Any(i => Math.Min(forward[i, i], backward[i, i]) > policy.ShapeTolerance)
            && HasMatchingPermutation(forward, backward, count, ct);
        var results = new List<HandwritingStrokeFeedback>();
        for (var i = 0; i < count; i++)
        {
            var issues = new List<HandwritingIssue>();
            var ratio = StrokeGeometry.Length(drawn[i]) / StrokeGeometry.Length(expected[i]);
            if (StrokeGeometry.Length(strokes[i]) < policy.MinimumRawStrokeLength || ratio < policy.MinimumLengthRatio)
                issues.Add(HandwritingIssue.StrokeTooShort);
            if (ratio > policy.MaximumLengthRatio) issues.Add(HandwritingIssue.StrokeTooLong);
            if (reordered) issues.Add(HandwritingIssue.WrongStrokeOrder);
            else if (forward[i, i] > policy.ShapeTolerance)
                issues.Add(backward[i, i] <= policy.ShapeTolerance && backward[i, i] + 0.02 < forward[i, i]
                    ? HandwritingIssue.WrongDirection : HandwritingIssue.WrongShape);
            var score = (int)Math.Round(100 * Math.Clamp(1 - forward[i, i] / (policy.ShapeTolerance * 3), 0, 1));
            results.Add(new HandwritingStrokeFeedback(i, score, Math.Round(forward[i, i], 4), Math.Round(ratio, 3), issues));
        }
        return HandwritingFeedback.Evaluated(count, results);
    }

    // Bounded subset DP avoids factorial search on ambiguous multi-stroke drawings (max 12 strokes).
    private bool HasMatchingPermutation(double[,] forward, double[,] backward, int count, CancellationToken ct)
    {
        var reachable = new bool[1 << count];
        reachable[0] = true;
        for (var mask = 0; mask < reachable.Length; mask++)
        {
            if (!reachable[mask]) continue;
            ct.ThrowIfCancellationRequested();
            var index = System.Numerics.BitOperations.PopCount((uint)mask);
            if (index == count) return true;
            for (var target = 0; target < count; target++)
                if ((mask & (1 << target)) == 0 && Math.Min(forward[index, target], backward[index, target]) <= policy.ShapeTolerance)
                    reachable[mask | (1 << target)] = true;
        }
        return false;
    }
}
