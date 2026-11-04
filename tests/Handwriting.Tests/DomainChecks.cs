using System.Reflection;
using Domain.Handwriting;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Handwriting.Tests;

internal static class DomainChecks
{
    internal static readonly DateTimeOffset Now = new(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);
    internal static readonly InkDrawing Line = new(new[] { new[] { new InkPoint(0.1, 0.2), new InkPoint(0.7, 0.8) } });
    internal static readonly Language Language = Language.Create(10, "ja", "日本語", true, 0, Now);
    internal static readonly WritingSystem System = WritingSystem.Create(11, 10, "hiragana", "ひらがな", true, 0, Now);
    internal static readonly WritingSystemItem Item = WritingSystemItem.Create(12, 11, "a", "あ", 0, null, null, Now);
    internal static readonly LearningProfile Profile = LearningProfile.Create(13, 14, "ja", "vi", "vi",
        ProficiencyLevel.Beginner, LearningGoal.DailyLife, null, Now);

    internal static HandwritingTemplate Template(long id = 15, bool published = true, InkDrawing? drawing = null) =>
        HandwritingTemplate.Create(id, Item, 1, drawing ?? Line, "https://example.test/templates", "revision-1",
            "Template author", "CC BY-SA 3.0", "https://creativecommons.org/licenses/by-sa/3.0/", published, Now);

    internal static HandwritingFeedback Feedback(int count = 1, params HandwritingIssue[] issues) =>
        HandwritingFeedback.Evaluated(count, Enumerable.Range(0, count).Select(i =>
            new HandwritingStrokeFeedback(i, issues.Length == 0 ? 99 : 20, 0.01, 1, issues)));

    public static void Run()
    {
        var source = new[] { new[] { new InkPoint(0, 0), new InkPoint(1, 1) } };
        var drawing = new InkDrawing(source);
        source[0][0] = new InkPoint(0.5, 0.5);
        drawing.ToArray()[0][0] = new InkPoint(0.5, 0.5);
        Check.That(drawing.Strokes[0][0] == new InkPoint(0, 0), "Drawing owns all nested input arrays and returned copies.");
        Check.Throws<NotSupportedException>(() => ((IList<InkPoint>)drawing.Strokes[0])[0] = new(0.4, 0.4), "Drawing exposes read-only strokes.");
        foreach (var number in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, -0.1, 1.1 })
        {
            Check.Throws<ArgumentException>(() => new InkDrawing(new[] { new[] { new InkPoint(number, 0) } }), "Reject invalid x.");
            Check.Throws<ArgumentException>(() => new InkDrawing(new[] { new[] { new InkPoint(0, number) } }), "Reject invalid y.");
        }
        Check.Throws<ArgumentNullException>(() => new InkDrawing(null!), "Null drawing rejected.");
        Check.Throws<ArgumentNullException>(() => new InkDrawing(new IEnumerable<InkPoint>[] { null! }), "Null stroke rejected.");
        Check.Throws<ArgumentException>(() => new InkDrawing(new[] { Array.Empty<InkPoint>() }), "Empty stroke rejected.");
        Check.Throws<ArgumentException>(() => new InkDrawing(Enumerable.Repeat(Line.Strokes[0], 13)), "Stroke limit enforced.");
        Check.Throws<ArgumentException>(() => new InkDrawing(new[] { Enumerable.Repeat(new InkPoint(0, 0), 513) }), "Point limit enforced.");
        Check.Throws<ArgumentException>(() => new InkDrawing(Enumerable.Repeat(Enumerable.Repeat(new InkPoint(0, 0), 512), 9)), "Total point limit enforced.");
        Check.That(new InkDrawing([]).PointCount == 0, "Missing drawing remains valid input for negative feedback.");
        Check.That(new InkDrawing(new[] { new[] { new InkPoint(0, 0) } }).PointCount == 1, "A tap is representable.");

        var template = Template();
        Check.That(template.Version == 1 && template.Revision == 1 && template.DrawingFormatVersion == 1, "Template starts at initial versions.");
        template.SetPublished(false, Now.AddSeconds(1));
        Check.That(!template.IsPublished && template.Version == 2 && template.UpdatedAt == Now.AddSeconds(1), "Publication increments optimistic version only.");
        template.SetPublished(false, Now.AddSeconds(2));
        Check.That(template.Version == 2 && template.UpdatedAt == Now.AddSeconds(1), "No-op publication preserves version and time.");
        Check.Throws<ArgumentException>(() => template.SetPublished(true, Now), "Cannot backdate publication.");
        typeof(HandwritingTemplate).GetProperty("Version")!.SetValue(template, long.MaxValue);
        Check.Throws<OverflowException>(() => template.SetPublished(true, Now.AddSeconds(2)), "Overflow does not partially publish.");
        Check.That(!template.IsPublished && template.Version == long.MaxValue && template.UpdatedAt == Now.AddSeconds(1), "Overflow mutation atomic.");
        Check.Throws<ArgumentOutOfRangeException>(() => Template(0), "Template requires Snowflake ID.");
        Check.Throws<ArgumentException>(() => Template(drawing: new InkDrawing([])), "Reference cannot be empty.");
        Check.Throws<ArgumentException>(() => Template(drawing: new InkDrawing(new[] { new[] { new InkPoint(0, 0), new InkPoint(0, 0) } })), "Degenerate reference rejected.");
        foreach (var url in new[] { "http://example.test/a", "https://user:pass@example.test/a", "https://example.test/#fragment", "https://example.test/a b", "" })
            Check.Throws<ArgumentException>(() => HandwritingTemplate.Create(16, Item, 1, Line, url, "rev", "Author", "License", "https://example.test/license", true, Now), "Invalid provenance URL rejected.");

        foreach (var issue in Enum.GetValues<HandwritingIssue>())
        {
            if (issue is HandwritingIssue.MissingDrawing or HandwritingIssue.DrawingTooSmall or HandwritingIssue.WrongStrokeCount) continue;
            var grade = Feedback(1, issue);
            Check.That(!grade.Passed && grade.Issues.SequenceEqual(new[] { issue }) && grade.MatchScore == 20, "Stroke issues propagate to character.");
        }
        Check.That(Feedback().Passed && Feedback().MatchScore == 99, "No issues means passed.");
        Check.Throws<ArgumentException>(() => Feedback(1, (HandwritingIssue)999), "Unknown enum rejected.");
        Check.Throws<ArgumentException>(() => Feedback(1, HandwritingIssue.WrongShape, HandwritingIssue.WrongShape), "Duplicate issue rejected.");
        Check.Throws<ArgumentException>(() => Feedback(1, HandwritingIssue.MissingDrawing), "Precheck issue cannot be attached to a stroke.");
        Check.Throws<ArgumentOutOfRangeException>(() => new HandwritingStrokeFeedback(0, 101, 0, 1, []), "Score bounded.");
        Check.Throws<ArgumentOutOfRangeException>(() => new HandwritingStrokeFeedback(0, 100, double.NaN, 1, []), "Distance finite.");
        Check.Throws<ArgumentOutOfRangeException>(() => new HandwritingStrokeFeedback(0, 100, 0, -1, []), "Ratio nonnegative.");
        Check.Throws<ArgumentException>(() => HandwritingFeedback.Evaluated(2, [new(0, 99, 0, 1, [])]), "Incomplete stroke feedback rejected.");
        Check.Throws<ArgumentException>(() => HandwritingFeedback.Evaluated(2, [new(0, 99, 0, 1, []), new(0, 99, 0, 1, [])]), "Repeated feedback index rejected.");
        Check.Throws<ArgumentException>(() => HandwritingFeedback.Rejected(1, 1, HandwritingIssue.MissingDrawing), "Rejection must match count.");
        Check.That(!HandwritingFeedback.Rejected(1, 0, HandwritingIssue.MissingDrawing).Passed, "Missing drawing rejected correctly.");
        Check.That(HandwritingFeedback.Rejected(2, 1, HandwritingIssue.WrongStrokeCount).MatchScore == 0, "Wrong count yields no score.");
        Check.That(!HandwritingFeedback.Rejected(1, 1, HandwritingIssue.DrawingTooSmall).Passed, "Too small supported.");

        var answer = new HandwritingPracticeAnswer(21, Template(), Item, Line, Feedback());
        HandwritingPracticeAttempt Create(IReadOnlyList<HandwritingPracticeAnswer>? answers = null, LearningProfile? profile = null,
            DateTimeOffset? now = null, string algorithm = "geometry-v1") =>
            HandwritingPracticeAttempt.Create(20, profile ?? Profile, System, Language, algorithm, answers ?? [answer], now ?? Now);
        var attempt = Create([answer, answer with { Id = 22 }]);
        Check.That(attempt.TargetText == "ああ" && attempt.CharacterCount == 2 && attempt.Passed && attempt.MatchScore == 99, "Repeated letters and aggregate grade supported.");
        Check.That(attempt.Characters.Select(x => x.Position).SequenceEqual(new[] { 0, 1 }), "Characters have stable ordered positions.");
        Check.That(attempt.Characters.All(x => x.Version == 1 && x.CreatedAt == Now && x.UpdatedAt == Now), "Immutable child metadata.");
        var failed = Create([answer, answer with { Id = 22, Feedback = Feedback(1, HandwritingIssue.WrongShape) }]);
        Check.That(!failed.Passed && failed.MatchScore == 60, "All characters must pass; score averages geometric feedback with banker's rounding.");
        Check.Throws<ArgumentException>(() => Create([]), "No empty attempts.");
        Check.Throws<ArgumentException>(() => Create(Enumerable.Range(0, 5).Select(i => answer with { Id = 21 + i }).ToArray()), "Attempt size bounded.");
        Check.Throws<ArgumentException>(() => Create([answer, answer]), "Child IDs unique.");
        Check.Throws<ArgumentException>(() => Create([answer with { Id = 20 }]), "Root ID differs from child IDs.");
        Check.Throws<ArgumentException>(() => Create([answer with { Template = Template(16, false) }]), "Unpublished reference rejected.");
        Check.Throws<ArgumentException>(() => Create([answer with { Item = WritingSystemItem.Create(23, 11, "b", "い", 1, null, null, Now) }]), "Wrong source item rejected.");
        Check.Throws<ArgumentException>(() => Create([answer with { Feedback = Feedback(2) }]), "Feedback counts match both drawings.");
        Check.Throws<ArgumentException>(() => Create(profile: LearningProfile.Create(30, 14, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, Now)), "Profile language mismatch rejected.");
        Check.Throws<ArgumentException>(() => Create(now: Now.AddSeconds(-1)), "Attempt cannot predate template.");
        Check.Throws<ArgumentException>(() => Create(algorithm: ""), "Algorithm revision is required.");
        Check.Throws<ArgumentException>(() => Create(algorithm: "geometry V1"), "Algorithm uses stable code convention.");
        var empty = Create([answer with { Drawing = new InkDrawing([]), Feedback = HandwritingFeedback.Rejected(1, 0, HandwritingIssue.MissingDrawing) }]);
        Check.That(!empty.Passed && empty.MatchScore == 0, "Failed/blank submissions are retained.");
        var offset = Create(now: Now.ToOffset(TimeSpan.FromHours(7)));
        Check.That(offset.CreatedAt.Offset == TimeSpan.Zero && offset.UpdatedAt == Now, "Stored timestamps normalize to UTC.");
        Check.That(Profile.Version == 1, "Attempt creation does not modify learning profile/progress.");
        foreach (var type in new[] { typeof(HandwritingPracticeAttempt), typeof(HandwritingPracticeCharacter), typeof(HandwritingTemplate) })
        {
            Check.That(!type.GetProperties().Any(x => x.SetMethod?.IsPublic == true), $"Controlled properties: {type.Name}.");
            Check.That(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0, $"Controlled factory: {type.Name}.");
        }
    }
}
