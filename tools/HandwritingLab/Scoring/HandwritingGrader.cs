using HandwritingLab.Contracts;
using HandwritingLab.Models;

namespace HandwritingLab.Scoring;

public sealed class HandwritingGrader(TemplateCatalog catalog, ScoringPolicy policy)
{
    public GradeResult Grade(GradeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.TargetId) || request.TargetId.Length > 16)
            throw new ArgumentException("A supported target ID is required.");
        var target = catalog.Find(request.TargetId) ?? throw new ArgumentException("This target is not supported by the prototype.");
        Validate(request, target);
        var characters = target.CharacterIds.Select((id, index) => GradeCharacter(catalog.Character(id), request.Characters[index])).ToArray();
        return new GradeResult(target.Id, policy.Version, catalog.Dataset.SourceRevision,
            characters.All(x => x.Passed), (int)Math.Round(characters.Average(x => x.MatchScore)), characters);
    }

    private void Validate(GradeRequest request, PracticeTarget target)
    {
        if (request.Characters is null || request.Characters.Count != target.CharacterIds.Count || request.Characters.Count > policy.MaximumCharacters)
            throw new ArgumentException("Supply exactly one stroke array per target character.");
        var total = 0;
        foreach (var strokes in request.Characters)
        {
            if (strokes is null || strokes.Count > policy.MaximumStrokesPerCharacter)
                throw new ArgumentException("A character has too many strokes or a null stroke array.");
            foreach (var points in strokes)
            {
                if (points is null || points.Count is < 1 || points.Count > policy.MaximumPointsPerStroke)
                    throw new ArgumentException("Each stroke must have 1 to 512 points.");
                total += points.Count;
                if (total > policy.MaximumTotalPoints) throw new ArgumentException("The drawing has too many points.");
                if (points.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y) || p.X is < 0 or > 1 || p.Y is < 0 or > 1))
                    throw new ArgumentException("Point coordinates must be finite numbers in the range 0 to 1.");
            }
        }
    }

    private static CharacterGrade GradeCharacter(CharacterTemplate template, IReadOnlyList<IReadOnlyList<InkPoint>> strokes)
    {
        static Domain.Handwriting.InkDrawing Drawing(IReadOnlyList<IReadOnlyList<InkPoint>> value) =>
            new(value.Select(stroke => stroke.Select(point => new Domain.Handwriting.InkPoint(point.X, point.Y))));
        var feedback = Core.Grade(Drawing(template.Strokes), Drawing(strokes));
        static HandwritingIssue Issue(Domain.Handwriting.HandwritingIssue value) => Enum.Parse<HandwritingIssue>(value.ToString());
        return new CharacterGrade(template.Symbol, feedback.Passed, feedback.MatchScore, feedback.ExpectedStrokeCount,
            feedback.ActualStrokeCount, feedback.Issues.Select(Issue).ToArray(), feedback.Strokes.Select(stroke =>
                new StrokeGrade(stroke.Index, stroke.Passed, stroke.MatchScore, stroke.Distance, stroke.LengthRatio,
                    stroke.Issues.Select(Issue).ToArray())).ToArray());
    }

    private static readonly Application.Handwriting.Scoring.HandwritingGrader Core =
        new(new Application.Handwriting.Scoring.HandwritingScoringPolicy());
}
