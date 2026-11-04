using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Application.Handwriting.Models;

public sealed record HandwritingCatalog(LearningProfile Profile, Language Language, WritingSystem WritingSystem,
    IReadOnlyList<HandwritingCatalogItem> Items);
