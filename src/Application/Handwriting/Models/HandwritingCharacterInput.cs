using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingCharacterInput(string TemplateId, IReadOnlyList<IReadOnlyList<InkPoint>> Strokes);
