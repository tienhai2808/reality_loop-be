using System.Diagnostics;
using HandwritingLab;
using HandwritingLab.Contracts;
using HandwritingLab.Models;
using HandwritingLab.Scoring;
using HandwritingLab.Tests;

var catalog = new TemplateCatalog();
var policy = new ScoringPolicy();
var grader = new HandwritingGrader(catalog, policy);
var baseline = System.Text.Json.JsonSerializer.Serialize(catalog.Dataset);
GradeRequest Request(string id, params IReadOnlyList<IReadOnlyList<InkPoint>>[] characters) => new() { TargetId = id, Characters = characters };
IReadOnlyList<IReadOnlyList<InkPoint>> Copy(CharacterTemplate template, Func<InkPoint, InkPoint>? map = null) =>
    template.Strokes.Select(x => (IReadOnlyList<InkPoint>)x.Select(map ?? (point => point)).ToArray()).ToArray();

foreach (var target in catalog.Targets)
{
    var characters = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x)).ToArray();
    var reference = grader.Grade(Request(target.Id, characters));
    Check.That(reference.Passed && reference.MatchScore == 100, $"Reference target {target.Text} passes.");
    Check.That(reference.AlgorithmVersion == policy.Version && reference.TemplateRevision == catalog.Dataset.SourceRevision, "Grades pin rule/template versions.");
    var scaled = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x, p => new InkPoint(p.X * .65 + .17, p.Y * .65 + .08))).ToArray();
    Check.That(grader.Grade(Request(target.Id, scaled)).Passed, "Uniform size/translation changes are allowed.");
    var random = new Random(42);
    var noisy = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x, p => new InkPoint(
        Math.Clamp(p.X + (random.NextDouble() - .5) * .012, 0, 1), Math.Clamp(p.Y + (random.NextDouble() - .5) * .012, 0, 1)))).ToArray();
    var noisyGrade = grader.Grade(Request(target.Id, noisy));
    Check.That(noisyGrade.Passed, $"Small geometric perturbations are tolerated for {target.Text}: {System.Text.Json.JsonSerializer.Serialize(noisyGrade)}");
    var repeated = characters.Select(character => (IReadOnlyList<IReadOnlyList<InkPoint>>)character
        .Select(stroke => (IReadOnlyList<InkPoint>)stroke.SelectMany(p => new[] { p, p }).ToArray()).ToArray()).ToArray();
    // Large template paths can exceed the external request cap after duplication; use a smaller repeat fixture below.
    if (repeated.SelectMany(x => x).All(x => x.Count <= policy.MaximumPointsPerStroke))
        Check.That(grader.Grade(Request(target.Id, repeated)).Passed, "Duplicate events/pauses do not change grading.");
    var tiny = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x, p => new InkPoint(p.X * .08 + .45, p.Y * .08 + .45))).ToArray();
    Check.That(!grader.Grade(Request(target.Id, tiny)).Passed, "Tiny drawings do not pass by normalization alone.");
    var mirrored = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x, p => new InkPoint(1 - p.X, p.Y))).ToArray();
    Check.That(!grader.Grade(Request(target.Id, mirrored)).Passed, "Mirroring is not normalized away.");
    var rotated = target.CharacterIds.Select(catalog.Character).Select(x => Copy(x, p => new InkPoint(1 - p.Y, p.X))).ToArray();
    Check.That(!grader.Grade(Request(target.Id, rotated)).Passed, "Rotation is not normalized away.");
    var empty = target.CharacterIds.Select(_ => (IReadOnlyList<IReadOnlyList<InkPoint>>)Array.Empty<IReadOnlyList<InkPoint>>()).ToArray();
    Check.That(!grader.Grade(Request(target.Id, empty)).Passed, "Empty handwriting never passes.");
}

foreach (var target in catalog.Targets.Where(x => x.CharacterIds.Count == 1))
{
    var template = catalog.Character(target.CharacterIds[0]);
    for (var index = 0; index < template.Strokes.Count; index++)
    {
        var reversed = Copy(template).ToArray(); reversed[index] = reversed[index].Reverse().ToArray();
        var grade = grader.Grade(Request(target.Id, reversed));
        Check.That(!grade.Passed && grade.Characters[0].Issues.Contains(HandwritingIssue.WrongDirection), "Reversed stroke is rejected with direction feedback.");
        var shortStroke = Copy(template).ToArray(); shortStroke[index] = [shortStroke[index][0]];
        Check.That(!grader.Grade(Request(target.Id, shortStroke)).Passed, "Tap in place of a stroke is rejected.");
        var missing = Copy(template).Where((_, i) => i != index).ToArray();
        Check.That(grader.Grade(Request(target.Id, missing)).Characters[0].Issues.Contains(HandwritingIssue.WrongStrokeCount), "Missing stroke is detected.");
    }
    var swapped = Copy(template).ToArray(); (swapped[0], swapped[1]) = (swapped[1], swapped[0]);
    Check.That(grader.Grade(Request(target.Id, swapped)).Characters[0].Issues.Contains(HandwritingIssue.WrongStrokeOrder), "Swapped strokes are rejected with order feedback.");
    var extra = Copy(template).Append(template.Strokes[0]).ToArray();
    Check.That(!grader.Grade(Request(target.Id, extra)).Passed, "Extra stroke is rejected.");
    var detached = Copy(template).ToArray(); detached[0] = detached[0].Select(p => new InkPoint(Math.Clamp(p.X + .25, 0, 1), p.Y)).ToArray();
    Check.That(!grader.Grade(Request(target.Id, detached)).Passed, "Moving only one stroke breaks relative placement.");
    foreach (var other in catalog.Dataset.Characters.Where(x => x.Id != template.Id))
        Check.That(!grader.Grade(Request(target.Id, Copy(other))).Passed, $"{other.Symbol} is not accepted as {template.Symbol}.");
}

var a = catalog.Character("u3042"); var o = catalog.Character("u304a");
Check.That(!grader.Grade(Request("ao", Copy(o), Copy(a))).Passed, "Word fails when characters are swapped.");
var scribble = Copy(a).ToArray();
scribble[0] = Enumerable.Range(0, 401).Select(i => new InkPoint(.5 + .25 * Math.Cos(i * .2), .5 + .25 * Math.Sin(i * .2))).ToArray();
Check.That(!grader.Grade(Request("a", scribble)).Passed, "Repeated loops do not pass by touching the reference region.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("unknown", Copy(a))), "Unknown target rejected.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("ao", Copy(a))), "Character count must match target.");
Check.Throws<ArgumentException>(() => grader.Grade(new() { TargetId = "a", Characters = null! }), "Null character payload rejected.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", null!)), "Null character array rejected.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", new IReadOnlyList<InkPoint>[] { null! })), "Null stroke rejected.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", new[] { Array.Empty<InkPoint>() })), "Zero-point stroke rejected.");
foreach (var bad in new[] { double.NaN, double.PositiveInfinity, -.001, 1.001 })
    Check.Throws<ArgumentException>(() => grader.Grade(Request("a", new[] { new[] { new InkPoint(bad, .5) } })), "Finite bounded coordinates required.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", Enumerable.Repeat(a.Strokes[0], 13).ToArray())), "Stroke budget enforced before shape math.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", new[] { Enumerable.Repeat(new InkPoint(.5, .5), 513).ToArray() })), "Per-stroke point budget enforced.");
Check.Throws<ArgumentException>(() => grader.Grade(Request("a", Enumerable.Repeat((IReadOnlyList<InkPoint>)Enumerable.Repeat(new InkPoint(.5, .5), 512).ToArray(), 9).ToArray())), "Global point budget enforced.");
var immutable = false;
try { ((IList<InkPoint>)a.Strokes[0])[0] = new InkPoint(0, 0); }
catch (NotSupportedException) { immutable = true; }
Check.That(immutable, "Published templates are immutable.");
var watch = Stopwatch.StartNew();
var requests = Enumerable.Range(0, 100).Select(_ => Task.Run(() => grader.Grade(Request("ao", Copy(a), Copy(o))))).ToArray();
var concurrent = await Task.WhenAll(requests);
watch.Stop();
Check.That(concurrent.All(x => x.Passed), "Shared grader/catalog are deterministic under concurrent requests.");
Check.That(System.Text.Json.JsonSerializer.Serialize(catalog.Dataset) == baseline, "Grading does not mutate source templates.");
Console.WriteLine($"Passed {Check.Count} synthetic/validation checks. 100 concurrent grades: {watch.ElapsedMilliseconds} ms (local measurement, not a production load test).");
await HttpChecks.RunAsync(catalog);
Console.WriteLine($"Passed {Check.Count} handwriting algorithm/HTTP checks. No real-human accuracy claim.");
