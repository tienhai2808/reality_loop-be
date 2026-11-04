using Domain.Handwriting;

namespace Application.Handwriting.Scoring;

// This version pins the entire policy. Changing any rule requires a new algorithm version.
public sealed record HandwritingScoringPolicy
{
    public string Version => "geometry-v1";
    public int SamplesPerStroke => 48;
    public double ShapeTolerance => 0.10;
    public double SimplificationTolerance => 0.01;
    public double MinimumDrawingExtent => 0.18;
    public double MinimumRawStrokeLength => 0.008;
    public double MinimumLengthRatio => 0.65;
    public double MaximumLengthRatio => 1.50;
    public int MaximumCharacters => HandwritingPracticeAttempt.MaxCharacters;
    public int MaximumStrokesPerCharacter => InkDrawing.MaxStrokes;
    public int MaximumPointsPerStroke => InkDrawing.MaxPointsPerStroke;
    public int MaximumTotalPoints => InkDrawing.MaxTotalPoints;
}
