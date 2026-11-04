namespace HandwritingLab.Models;

public sealed class CharacterTemplate
{
    public CharacterTemplate(string id, string symbol, IReadOnlyList<IReadOnlyList<InkPoint>> strokes)
    {
        Id = id;
        Symbol = symbol;
        Strokes = Array.AsReadOnly(strokes.Select(stroke =>
            (IReadOnlyList<InkPoint>)Array.AsReadOnly(stroke.ToArray())).ToArray());
    }

    public string Id { get; }
    public string Symbol { get; }
    public IReadOnlyList<IReadOnlyList<InkPoint>> Strokes { get; }
}
