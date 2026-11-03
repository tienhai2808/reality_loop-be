using Domain.WritingSystems;

namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeCatalogItem(WritingSystemItem Item, IReadOnlyList<WritingSystemItemReading> Readings);
