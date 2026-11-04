using System.Text.Json;
using HandwritingLab.Models;

namespace HandwritingLab;

public sealed class TemplateCatalog
{
    public TemplateCatalog()
    {
        using var stream = typeof(TemplateCatalog).Assembly.GetManifestResourceStream("HandwritingLab.templates.json")
            ?? throw new InvalidOperationException("Bundled handwriting templates were not found.");
        Dataset = JsonSerializer.Deserialize<TemplateDataset>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ?? throw new InvalidOperationException("Invalid handwriting dataset.");
        Dataset = Dataset with { Characters = Array.AsReadOnly(Dataset.Characters.ToArray()) };
        if (Dataset.Characters.Count != 4 || Dataset.Characters.Any(x => x.Strokes.Count is < 1 or > 3))
            throw new InvalidOperationException("Unexpected prototype template coverage.");
        Targets = Array.AsReadOnly(new[]
        {
            Target("a", "あ", "u3042"), Target("o", "お", "u304a"),
            Target("nu", "ぬ", "u306c"), Target("me", "め", "u3081"), Target("ao", "あお", "u3042", "u304a")
        });
    }

    public TemplateDataset Dataset { get; }
    public IReadOnlyList<PracticeTarget> Targets { get; }
    public PracticeTarget? Find(string id) => Targets.SingleOrDefault(x => x.Id == id);
    public CharacterTemplate Character(string id) => Dataset.Characters.Single(x => x.Id == id);

    private static PracticeTarget Target(string id, string text, params string[] characterIds) =>
        new(id, text, Array.AsReadOnly(characterIds));
}
