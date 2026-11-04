using System.Text.Json.Serialization;

namespace Api.Contracts.Handwriting;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed record HandwritingPointRequest
{
    [JsonRequired] public double X { get; init; }
    [JsonRequired] public double Y { get; init; }
}
