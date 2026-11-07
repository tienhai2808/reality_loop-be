using System.Text.Json;
using Domain.Handwriting;

namespace Infrastructure.Persistence.Seeding;

internal static class HandwritingSeedData
{
    // Same reserved Snowflake millisecond/generator as FoundationSeedData, sequences 3000..3003 (legacy), 3100..3145 and 3200..3245.
    // Keys are explicit; changes in file ordering must not change identities.
    private static readonly (string Key, int Sequence, int ItemSequence)[] Keys =
    [
        ("u3042", 3000, 100),
        ("u304a", 3001, 104),
        ("u306c", 3002, 122),
        ("u3081", 3003, 133),
        ("u3044", 3101, 101),
        ("u3046", 3102, 102),
        ("u3048", 3103, 103),
        ("u304b", 3105, 105),
        ("u304d", 3106, 106),
        ("u304f", 3107, 107),
        ("u3051", 3108, 108),
        ("u3053", 3109, 109),
        ("u3055", 3110, 110),
        ("u3057", 3111, 111),
        ("u3059", 3112, 112),
        ("u305b", 3113, 113),
        ("u305d", 3114, 114),
        ("u305f", 3115, 115),
        ("u3061", 3116, 116),
        ("u3064", 3117, 117),
        ("u3066", 3118, 118),
        ("u3068", 3119, 119),
        ("u306a", 3120, 120),
        ("u306b", 3121, 121),
        ("u306d", 3123, 123),
        ("u306e", 3124, 124),
        ("u306f", 3125, 125),
        ("u3072", 3126, 126),
        ("u3075", 3127, 127),
        ("u3078", 3128, 128),
        ("u307b", 3129, 129),
        ("u307e", 3130, 130),
        ("u307f", 3131, 131),
        ("u3080", 3132, 132),
        ("u3082", 3134, 134),
        ("u3084", 3135, 135),
        ("u3086", 3136, 136),
        ("u3088", 3137, 137),
        ("u3089", 3138, 138),
        ("u308a", 3139, 139),
        ("u308b", 3140, 140),
        ("u308c", 3141, 141),
        ("u308d", 3142, 142),
        ("u308f", 3143, 143),
        ("u3092", 3144, 144),
        ("u3093", 3145, 145),
        ("u30a2", 3200, 200),
        ("u30a4", 3201, 201),
        ("u30a6", 3202, 202),
        ("u30a8", 3203, 203),
        ("u30aa", 3204, 204),
        ("u30ab", 3205, 205),
        ("u30ad", 3206, 206),
        ("u30af", 3207, 207),
        ("u30b1", 3208, 208),
        ("u30b3", 3209, 209),
        ("u30b5", 3210, 210),
        ("u30b7", 3211, 211),
        ("u30b9", 3212, 212),
        ("u30bb", 3213, 213),
        ("u30bd", 3214, 214),
        ("u30bf", 3215, 215),
        ("u30c1", 3216, 216),
        ("u30c4", 3217, 217),
        ("u30c6", 3218, 218),
        ("u30c8", 3219, 219),
        ("u30ca", 3220, 220),
        ("u30cb", 3221, 221),
        ("u30cc", 3222, 222),
        ("u30cd", 3223, 223),
        ("u30ce", 3224, 224),
        ("u30cf", 3225, 225),
        ("u30d2", 3226, 226),
        ("u30d5", 3227, 227),
        ("u30d8", 3228, 228),
        ("u30db", 3229, 229),
        ("u30de", 3230, 230),
        ("u30df", 3231, 231),
        ("u30e0", 3232, 232),
        ("u30e1", 3233, 233),
        ("u30e2", 3234, 234),
        ("u30e4", 3235, 235),
        ("u30e6", 3236, 236),
        ("u30e8", 3237, 237),
        ("u30e9", 3238, 238),
        ("u30ea", 3239, 239),
        ("u30eb", 3240, 240),
        ("u30ec", 3241, 241),
        ("u30ed", 3242, 242),
        ("u30ef", 3243, 243),
        ("u30f2", 3244, 244),
        ("u30f3", 3245, 245),
    ];

    public static readonly HandwritingTemplate[] Templates = Build();

    private static HandwritingTemplate[] Build()
    {
        using var stream = typeof(HandwritingSeedData).Assembly.GetManifestResourceStream("Handwriting.templates.json")
            ?? throw new InvalidOperationException("The pinned handwriting seed is missing.");
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;
        if (root.GetProperty("sourceRevision").GetString() != "70a0b7ae0c18ceb5cb358274b029cce0234a43bc")
            throw new InvalidOperationException("A dataset change requires a new template revision and reviewed migration.");
        var characters = root.GetProperty("characters").EnumerateArray().ToDictionary(x => x.GetProperty("id").GetString()!);
        var originalTimestamp = new DateTimeOffset(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);
        var keys = Keys.Select(key => (Key: key.Key, Id: FoundationSeedData.JapaneseId + key.Sequence,
                ItemId: FoundationSeedData.JapaneseId + key.ItemSequence, Timestamp: originalTimestamp))
            .Concat(FoundationSeedData.WritingSystemItemRows
                .Where(item => item.Id >= FoundationSeedData.ExpansionId && item.Id < FoundationSeedData.ExpansionId + 200)
                .Select(item => (Key: "u" + string.Join("u", item.Symbol.Select(c => ((int)c).ToString("x4"))),
                    Id: item.Id + 3000, ItemId: item.Id, Timestamp: FoundationSeedData.ExpandedAt)))
            .ToArray();
        if (characters.Count != keys.Length) throw new InvalidOperationException("Unexpected seed characters.");
        return keys.Select(key =>
        {
            var character = characters[key.Key];
            var item = FoundationSeedData.WritingSystemItemRows.Single(x => x.Id == key.ItemId);
            if (item.Symbol != character.GetProperty("symbol").GetString())
                throw new InvalidOperationException("The template does not match its catalog item.");
            var drawing = new InkDrawing(character.GetProperty("strokes").EnumerateArray().Select(stroke =>
                stroke.EnumerateArray().Select(point => new InkPoint(point.GetProperty("x").GetDouble(), point.GetProperty("y").GetDouble()))));
            return HandwritingTemplate.Create(key.Id, item, 1, drawing,
                root.GetProperty("sourceUrl").GetString()!, root.GetProperty("sourceRevision").GetString()!,
                root.GetProperty("attribution").GetString()!, root.GetProperty("license").GetString()!,
                root.GetProperty("licenseUrl").GetString()!, true, key.Timestamp);
        }).ToArray();
    }
}
