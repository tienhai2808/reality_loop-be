using System.Globalization;
using Application.Common.Exceptions;
using Application.Handwriting.Models;
using Domain.Handwriting;

namespace Application.Handwriting.Validation;

internal static class HandwritingValidation
{
    public static void Id(long value)
    {
        if (value <= 0) throw new RequestValidationException("A positive profile, template or attempt ID is required.");
    }

    public static long ParseId(string value)
    {
        if (value is null || value.Length > 19 ||
            !long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new RequestValidationException("IDs must be positive decimal strings within the Int64 range.");
        return id;
    }

    public static long? Page(int pageSize, string? before)
    {
        if (pageSize is < 1 or > 100) throw new RequestValidationException("Page size must be between 1 and 100.");
        return before is null ? null : ParseId(before);
    }

    public static IReadOnlyList<(long TemplateId, InkDrawing Drawing)> Drawings(IReadOnlyList<HandwritingCharacterInput> characters)
    {
        if (characters is null || characters.Count is < 1 or > HandwritingPracticeAttempt.MaxCharacters)
            throw new RequestValidationException($"Supply 1 to {HandwritingPracticeAttempt.MaxCharacters} characters in target order.");
        var result = new List<(long, InkDrawing)>();
        var total = 0;
        foreach (var character in characters)
        {
            if (character is null) throw new RequestValidationException("Characters cannot contain null.");
            var id = ParseId(character.TemplateId);
            InkDrawing drawing;
            try { drawing = new InkDrawing(character.Strokes); }
            catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }
            total += drawing.PointCount;
            if (total > InkDrawing.MaxTotalPoints)
                throw new RequestValidationException($"An attempt can contain at most {InkDrawing.MaxTotalPoints} points.");
            result.Add((id, drawing));
        }
        return result;
    }
}
