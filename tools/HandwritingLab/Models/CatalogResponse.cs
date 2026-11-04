namespace HandwritingLab.Models;

public sealed record CatalogResponse(ScoringPolicy Policy, TemplateDataset Dataset, IReadOnlyList<PracticeTarget> Targets);
