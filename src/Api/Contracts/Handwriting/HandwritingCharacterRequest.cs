using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Handwriting;

namespace Api.Contracts.Handwriting;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record HandwritingCharacterRequest
{
    [JsonRequired, Required, StringLength(19, MinimumLength = 1)] public string TemplateId { get; init; } = null!;
    [JsonRequired, Required, MaxLength(InkDrawing.MaxStrokes)] public List<List<HandwritingPointRequest>> Strokes { get; init; } = null!;
}
