using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Application.Handwriting.Models;
using Domain.Handwriting;

namespace Api.Contracts.Handwriting;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CreateHandwritingAttemptRequest
{
    [JsonRequired, Required, MinLength(1), MaxLength(HandwritingPracticeAttempt.MaxCharacters)]
    public List<HandwritingCharacterRequest> Characters { get; init; } = null!;

    public IReadOnlyList<HandwritingCharacterInput> ToInput() => Characters.Select(character =>
    {
        if (character is null || character.Strokes is null)
            throw new RequestValidationException("Each character must contain a template ID and a stroke array.");
        var strokes = character.Strokes.Select(stroke =>
        {
            if (stroke is null) throw new RequestValidationException("Strokes cannot contain null.");
            return stroke.Select(point => point is null
                ? throw new RequestValidationException("Points cannot contain null.") : new InkPoint(point.X, point.Y)).ToArray();
        }).ToArray();
        return new HandwritingCharacterInput(character.TemplateId, strokes);
    }).ToArray();
}
