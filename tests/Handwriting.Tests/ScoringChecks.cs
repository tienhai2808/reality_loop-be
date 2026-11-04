using System.Diagnostics;
using Application.Handwriting.Scoring;
using Domain.Handwriting;

namespace Handwriting.Tests;

internal static class ScoringChecks
{
    public static void Run()
    {
        var grader = new HandwritingGrader(new HandwritingScoringPolicy());
        var line = DomainChecks.Line;
        Check.That(grader.Grade(line, line).Passed, "Application grader accepts exact geometry.");
        Check.That(grader.Grade(line, new InkDrawing([])).Issues.Single() == HandwritingIssue.MissingDrawing, "Missing input is feedback.");
        Check.That(grader.Grade(line, new InkDrawing(new[] { new[] { new InkPoint(0.5, 0.5) } })).Issues.Single() == HandwritingIssue.DrawingTooSmall, "A tap is too small.");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Check.Throws<OperationCanceledException>(() => grader.Grade(line, line, cancellation.Token), "Canceled scoring stops before work.");
        var reference = new InkDrawing(Enumerable.Repeat(line.Strokes[0], 12));
        var drawing = new InkDrawing(Enumerable.Repeat(line.Strokes[0], 11)
            .Append(new[] { new InkPoint(0.1, 0.5), new InkPoint(0.7, 0.5) }));
        var watch = Stopwatch.StartNew();
        Check.That(!grader.Grade(reference, drawing).Passed && watch.Elapsed < TimeSpan.FromSeconds(5),
            "Ambiguous 12-stroke input uses bounded subset matching rather than factorial backtracking.");
    }
}
