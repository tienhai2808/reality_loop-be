using System.Text.Json;
using Domain.Handwriting;

namespace Infrastructure.Persistence.Seeding.German;

internal static class HandwritingSeedData
{
    public static readonly HandwritingTemplate[] Templates = Build();

    private static HandwritingTemplate[] Build()
    {
        using var stream = typeof(HandwritingSeedData).Assembly.GetManifestResourceStream("Handwriting.de.templates.json")
            ?? throw new InvalidOperationException("The German handwriting seed is missing.");
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;
        if (root.GetProperty("sourceRevision").GetString() != "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1")
            throw new InvalidOperationException("A source change requires a reviewed German template revision.");
        var characters = root.GetProperty("characters").EnumerateArray().ToDictionary(row => row.GetProperty("key").GetInt32());
        if (characters.Count != FoundationSeedData.WritingSystemItemRows.Length)
            throw new InvalidOperationException("Unexpected German reference coverage.");
        return FoundationSeedData.WritingSystemItemRows.Select(item =>
        {
            var key = checked((int)(item.Id - FoundationSeedData.GermanId - 100));
            var character = characters[key];
            if (character.GetProperty("symbol").GetString() != item.Symbol)
                throw new InvalidOperationException("The German reference does not match its catalog item.");
            var drawing = new InkDrawing(character.GetProperty("strokes").EnumerateArray().Select(stroke =>
                stroke.EnumerateArray().Select(point => new InkPoint(point.GetProperty("x").GetDouble(), point.GetProperty("y").GetDouble()))));
            return HandwritingTemplate.Create(FoundationSeedData.GermanId + 300 + key, item, 1, drawing,
                root.GetProperty("sourceUrl").GetString()!, root.GetProperty("sourceRevision").GetString()!,
                root.GetProperty("attribution").GetString()!, root.GetProperty("license").GetString()!, root.GetProperty("licenseUrl").GetString()!,
                true, FoundationSeedData.PublishedAt);
        }).ToArray();
    }
}
