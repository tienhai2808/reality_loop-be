using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystems;

namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeCatalog(LearningProfile Profile, Language Language, WritingSystem WritingSystem,
    IReadOnlyList<WritingSystemPracticeCatalogItem> Items);
