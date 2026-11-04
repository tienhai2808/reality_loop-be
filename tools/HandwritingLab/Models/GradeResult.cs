namespace HandwritingLab.Models;

public sealed record GradeResult(string TargetId, string AlgorithmVersion, string TemplateRevision,
    bool Passed, int MatchScore, IReadOnlyList<CharacterGrade> Characters);
