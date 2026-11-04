using Application.Handwriting.Scoring;

namespace Application.Handwriting.Models;

public sealed record HandwritingTemplatePage(IReadOnlyList<HandwritingTemplateSummaryDto> Items, string? NextCursor,
    HandwritingScoringPolicy Policy);
