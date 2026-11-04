using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingAttemptCharacterDto(string Id, int Position, HandwritingReferenceDto Template,
    int DrawingFormatVersion, IReadOnlyList<IReadOnlyList<InkPoint>> Strokes, int FeedbackFormatVersion,
    HandwritingFeedbackDto Feedback);
