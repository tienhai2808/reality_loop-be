using Application.Handwriting.Scoring;
using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingTemplateDto(HandwritingTemplateSummaryDto Template, int DrawingFormatVersion,
    IReadOnlyList<IReadOnlyList<InkPoint>> Strokes, HandwritingScoringPolicy Policy)
{
    public int StrokeCount => Strokes.Count;
}
