using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingAttemptSummaryDto(string Id, string LearningProfileId, string WritingSystemId,
    string TargetText, string AlgorithmVersion, int CharacterCount, bool Passed, int MatchScore, long Version,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static HandwritingAttemptSummaryDto From(HandwritingPracticeAttempt attempt) => new(attempt.Id.ToString(),
        attempt.LearningProfileId.ToString(), attempt.WritingSystemId.ToString(), attempt.TargetText, attempt.AlgorithmVersion,
        attempt.CharacterCount, attempt.Passed, attempt.MatchScore, attempt.Version, attempt.CreatedAt, attempt.UpdatedAt);
}
