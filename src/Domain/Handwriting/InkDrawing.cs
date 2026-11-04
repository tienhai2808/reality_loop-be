namespace Domain.Handwriting;

/// <summary>Immutable, ordered strokes in a normalized square coordinate space. Empty means no drawing.</summary>
public sealed class InkDrawing
{
    public const int FormatVersion = 1;
    public const int MaxStrokes = 12;
    public const int MaxPointsPerStroke = 512;
    public const int MaxTotalPoints = 4096;

    public InkDrawing(IEnumerable<IEnumerable<InkPoint>> strokes)
    {
        ArgumentNullException.ThrowIfNull(strokes);
        var copy = new List<IReadOnlyList<InkPoint>>();
        var total = 0;
        // Enumerate with bounds so a lazy or unexpectedly large input cannot allocate without limit.
        foreach (var stroke in strokes)
        {
            ArgumentNullException.ThrowIfNull(stroke);
            if (copy.Count == MaxStrokes) throw new ArgumentException("Too many strokes.", nameof(strokes));
            var points = new List<InkPoint>();
            foreach (var point in stroke)
            {
                if (points.Count == MaxPointsPerStroke || ++total > MaxTotalPoints)
                    throw new ArgumentException("Too many drawing points.", nameof(strokes));
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y) ||
                    point.X is < 0 or > 1 || point.Y is < 0 or > 1)
                    throw new ArgumentException("Coordinates must be finite and between zero and one.", nameof(strokes));
                points.Add(point);
            }
            if (points.Count == 0) throw new ArgumentException("A stroke must contain at least one point.", nameof(strokes));
            copy.Add(points.AsReadOnly());
        }
        Strokes = copy.AsReadOnly();
        PointCount = total;
    }

    public IReadOnlyList<IReadOnlyList<InkPoint>> Strokes { get; }
    public int PointCount { get; }

    public InkPoint[][] ToArray() => Strokes.Select(x => x.ToArray()).ToArray();
}
