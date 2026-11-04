namespace HandwritingLab.Models;

public sealed record PracticeTarget(string Id, string Text, IReadOnlyList<string> CharacterIds);
