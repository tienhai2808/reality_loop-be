using Domain.WritingSystems;

namespace Domain.Handwriting;

// Creation input only; IDs are supplied by the existing Snowflake generator.
public sealed record HandwritingPracticeAnswer(long Id, HandwritingTemplate Template, WritingSystemItem Item,
    InkDrawing Drawing, HandwritingFeedback Feedback);
