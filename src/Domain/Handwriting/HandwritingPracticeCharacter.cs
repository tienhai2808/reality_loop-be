using Domain.Common;

namespace Domain.Handwriting;

public sealed class HandwritingPracticeCharacter : Entity
{
    private HandwritingPracticeCharacter()
    {
    }

    public long AttemptId { get; private init; }
    public long TemplateId { get; private init; }
    public int Position { get; private init; }
    public InkDrawing Drawing { get; private init; } = null!;
    public int DrawingFormatVersion { get; private init; }
    public HandwritingFeedback Feedback { get; private init; } = null!;
    public int FeedbackFormatVersion { get; private init; }
    public long Version { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private init; }

    internal static HandwritingPracticeCharacter Create(long attemptId, int position,
        HandwritingPracticeAnswer answer, DateTimeOffset timestamp) => new()
    {
        Id = answer.Id,
        AttemptId = attemptId,
        TemplateId = answer.Template.Id,
        Position = position,
        Drawing = answer.Drawing,
        DrawingFormatVersion = InkDrawing.FormatVersion,
        Feedback = answer.Feedback,
        FeedbackFormatVersion = HandwritingFeedback.FormatVersion,
        Version = 1,
        CreatedAt = timestamp,
        UpdatedAt = timestamp
    };
}
