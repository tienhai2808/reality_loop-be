using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Handwriting;

namespace Infrastructure.Persistence.Serialization;

internal static class HandwritingJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter<HandwritingIssue>(allowIntegerValues: false) }
    };

    public static string WriteDrawing(InkDrawing drawing) => JsonSerializer.Serialize(drawing.ToArray(), Options);

    public static InkDrawing ReadDrawing(string json)
    {
        using var document = JsonDocument.Parse(json);
        // GetProperty rejects absent coordinates rather than defaulting a missing x/y to zero.
        return new InkDrawing(document.RootElement.EnumerateArray().Select(stroke => stroke.EnumerateArray()
            .Select(point => new InkPoint(point.GetProperty("x").GetDouble(), point.GetProperty("y").GetDouble()))));
    }

    public static string WriteFeedback(HandwritingFeedback feedback) => JsonSerializer.Serialize(feedback, Options);

    public static HandwritingFeedback ReadFeedback(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var expected = root.GetProperty("expectedStrokeCount").GetInt32();
        var actual = root.GetProperty("actualStrokeCount").GetInt32();
        var strokes = root.GetProperty("strokes").EnumerateArray().Select(stroke =>
        {
            var value = new HandwritingStrokeFeedback(stroke.GetProperty("index").GetInt32(),
                stroke.GetProperty("matchScore").GetInt32(), stroke.GetProperty("distance").GetDouble(),
                stroke.GetProperty("lengthRatio").GetDouble(), ReadIssues(stroke.GetProperty("issues")));
            if (value.Passed != stroke.GetProperty("passed").GetBoolean())
                throw new JsonException("Inconsistent stroke outcome.");
            return value;
        }).ToArray();
        var issues = ReadIssues(root.GetProperty("issues"));
        var result = strokes.Length == 0
            ? HandwritingFeedback.Rejected(expected, actual, issues.Single())
            : HandwritingFeedback.Evaluated(expected, strokes);
        if (result.ActualStrokeCount != actual || result.Passed != root.GetProperty("passed").GetBoolean() ||
            result.MatchScore != root.GetProperty("matchScore").GetInt32() || !result.Issues.SequenceEqual(issues))
            throw new JsonException("Inconsistent character outcome.");
        return result;
    }

    private static HandwritingIssue[] ReadIssues(JsonElement element) => element.EnumerateArray()
        .Select(x => JsonSerializer.Deserialize<HandwritingIssue>(x.GetRawText(), Options)).ToArray();
}
