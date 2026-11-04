using Domain.Handwriting;

namespace Application.Handwriting.Scoring;

internal static class StrokeGeometry
{
    public static double Distance(InkPoint a, InkPoint b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

    public static double Length(IReadOnlyList<InkPoint> points)
    {
        var result = 0d;
        for (var i = 1; i < points.Count; i++) result += Distance(points[i - 1], points[i]);
        return result;
    }

    public static double Extent(IReadOnlyList<IReadOnlyList<InkPoint>> strokes)
    {
        var points = strokes.SelectMany(x => x).ToArray();
        return Math.Max(points.Max(x => x.X) - points.Min(x => x.X), points.Max(x => x.Y) - points.Min(x => x.Y));
    }

    // Normalize the whole character uniformly; never align/rotate/resize individual strokes independently.
    // This preserves aspect ratio, relative placement and distinctions such as loops on nu versus me.
    public static InkPoint[][] Normalize(IReadOnlyList<IReadOnlyList<InkPoint>> strokes)
    {
        var points = strokes.SelectMany(x => x).ToArray();
        var minX = points.Min(x => x.X); var maxX = points.Max(x => x.X);
        var minY = points.Min(x => x.Y); var maxY = points.Max(x => x.Y);
        var center = new InkPoint((minX + maxX) / 2, (minY + maxY) / 2);
        var scale = 0.8 / Math.Max(Math.Max(maxX - minX, maxY - minY), 1e-9);
        return strokes.Select(stroke => stroke.Select(point =>
            new InkPoint((point.X - center.X) * scale + 0.5, (point.Y - center.Y) * scale + 0.5)).ToArray()).ToArray();
    }

    public static InkPoint[] Resample(IReadOnlyList<InkPoint> points, int count)
    {
        var distances = new double[points.Count];
        for (var i = 1; i < points.Count; i++) distances[i] = distances[i - 1] + Distance(points[i - 1], points[i]);
        if (distances[^1] < 1e-9) return Enumerable.Repeat(points[0], count).ToArray();
        var result = new InkPoint[count];
        var segment = 1;
        for (var i = 0; i < count; i++)
        {
            var target = distances[^1] * i / (count - 1);
            while (segment < points.Count - 1 && distances[segment] < target) segment++;
            var span = distances[segment] - distances[segment - 1];
            var t = span < 1e-9 ? 0 : (target - distances[segment - 1]) / span;
            result[i] = new InkPoint(points[segment - 1].X + (points[segment].X - points[segment - 1].X) * t,
                points[segment - 1].Y + (points[segment].Y - points[segment - 1].Y) * t);
        }
        return result;
    }

    // Ramer-Douglas-Peucker removes dense capture jitter before measuring path length.
    // Distance is to the finite segment, so loops/backtracking do not collapse into a line.
    public static InkPoint[] Simplify(IReadOnlyList<InkPoint> points, double tolerance)
    {
        if (points.Count <= 2) return points.ToArray();
        var keep = new bool[points.Count]; keep[0] = true; keep[^1] = true;
        var segments = new Stack<(int Start, int End)>(); segments.Push((0, points.Count - 1));
        while (segments.TryPop(out var segment))
        {
            var start = points[segment.Start]; var end = points[segment.End];
            var dx = end.X - start.X; var dy = end.Y - start.Y; var span = dx * dx + dy * dy;
            var furthest = -1; var maximum = tolerance;
            for (var i = segment.Start + 1; i < segment.End; i++)
            {
                var t = span < 1e-12 ? 0 : Math.Clamp(((points[i].X - start.X) * dx + (points[i].Y - start.Y) * dy) / span, 0, 1);
                var distance = Distance(points[i], new InkPoint(start.X + t * dx, start.Y + t * dy));
                if (distance > maximum) { maximum = distance; furthest = i; }
            }
            if (furthest < 0) continue;
            keep[furthest] = true;
            segments.Push((segment.Start, furthest)); segments.Push((furthest, segment.End));
        }
        return points.Where((_, index) => keep[index]).ToArray();
    }

    // Discrete Fréchet distance keeps traversal order, while arc-length resampling removes speed differences.
    public static double Frechet(IReadOnlyList<InkPoint> a, IReadOnlyList<InkPoint> b)
    {
        var previous = new double[b.Count];
        var current = new double[b.Count];
        Array.Fill(previous, double.PositiveInfinity);
        for (var i = 0; i < a.Count; i++)
        {
            for (var j = 0; j < b.Count; j++)
            {
                var distance = Distance(a[i], b[j]);
                if (i == 0 && j == 0) current[j] = distance;
                else
                {
                    var best = Math.Min(i > 0 ? previous[j] : double.PositiveInfinity,
                        Math.Min(j > 0 ? current[j - 1] : double.PositiveInfinity, i > 0 && j > 0 ? previous[j - 1] : double.PositiveInfinity));
                    current[j] = Math.Max(distance, best);
                }
            }
            (previous, current) = (current, previous);
        }
        return previous[^1];
    }
}
