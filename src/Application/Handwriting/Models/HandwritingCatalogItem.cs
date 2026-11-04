using Domain.Handwriting;
using Domain.WritingSystems;

namespace Application.Handwriting.Models;

public sealed record HandwritingCatalogItem(HandwritingTemplate Template, WritingSystemItem Item);
