using Domain.Languages;
using Domain.LearningProfiles;
using Domain.LearningProgress;
using Domain.WritingSystems;

namespace Application.LearningProgress.Models;

// Loaded in one owner-qualified query; ancestry is supplied by the catalog, not the client.
public sealed record WritingSystemItemProgressContext(LearningProfile Profile, Language Language,
    WritingSystem WritingSystem, WritingSystemItem Item, WritingSystemItemProgress? Progress);
