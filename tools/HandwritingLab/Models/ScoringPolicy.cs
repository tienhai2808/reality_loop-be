namespace HandwritingLab.Models;

public sealed record ScoringPolicy
{
    private static readonly Application.Handwriting.Scoring.HandwritingScoringPolicy Core = new();
    public string Version => Core.Version;
    public int SamplesPerStroke => Core.SamplesPerStroke;
    public double ShapeTolerance => Core.ShapeTolerance;
    public double SimplificationTolerance => Core.SimplificationTolerance;
    public double MinimumDrawingExtent => Core.MinimumDrawingExtent;
    public double MinimumRawStrokeLength => Core.MinimumRawStrokeLength;
    public double MinimumLengthRatio => Core.MinimumLengthRatio;
    public double MaximumLengthRatio => Core.MaximumLengthRatio;
    public int MaximumCharacters => Core.MaximumCharacters;
    public int MaximumStrokesPerCharacter => Core.MaximumStrokesPerCharacter;
    public int MaximumPointsPerStroke => Core.MaximumPointsPerStroke;
    public int MaximumTotalPoints => Core.MaximumTotalPoints;
}
