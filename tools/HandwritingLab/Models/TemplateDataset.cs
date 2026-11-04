namespace HandwritingLab.Models;

public sealed record TemplateDataset(string SourceRevision, string SourceUrl, string Attribution,
    string License, string LicenseUrl, IReadOnlyList<CharacterTemplate> Characters);
