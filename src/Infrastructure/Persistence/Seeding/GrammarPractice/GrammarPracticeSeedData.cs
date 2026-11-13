using Domain.GrammarPractice;

namespace Infrastructure.Persistence.Seeding.GrammarPractice;

internal static class GrammarPracticeSeedData
{
    private static readonly DateTimeOffset PublishedAt = new(2026, 11, 11, 0, 0, 0, TimeSpan.Zero);
    // Reserved generator 1023, milliseconds +3/+4 after the original Japanese allocation.
    private const long JapaneseBase = 1_563_703_640_080_773_120L;
    private const long GermanBase = 1_563_703_640_084_967_424L;
    private static readonly (string Language, long Base, GrammarExerciseSeedDefinition[] Rows)[] Bundles =
    [
        ("ja", JapaneseBase, Japanese.GrammarExerciseSeedData.Rows),
        ("de", GermanBase, German.GrammarExerciseSeedData.Rows)
    ];

    public static readonly GrammarExercise[] ExerciseRows = BuildExercises();
    public static readonly GrammarExerciseTranslation[] TranslationRows = BuildTranslations();

    private static GrammarExercise[] BuildExercises()
    {
        var languages = CatalogSeedData.LanguageRows.ToDictionary(row => row.Code, row => row.Id);
        var points = CatalogSeedData.GrammarPointRows.ToDictionary(row => (row.LanguageId, row.Code));
        return Bundles.SelectMany(bundle => bundle.Rows.Select(row => GrammarExercise.Create(bundle.Base + row.Key,
            points[(languages[bundle.Language], row.GrammarPointCode)].Id, $"fill-{row.Key}", row.Prompt,
            row.Options, row.CorrectOptionIndex, true, row.Key, PublishedAt))).ToArray();
    }

    private static GrammarExerciseTranslation[] BuildTranslations() => Bundles.SelectMany(bundle => bundle.Rows.SelectMany(row => new[]
    {
        GrammarExerciseTranslation.Create(bundle.Base + 200 + row.Key * 2, bundle.Base + row.Key, "vi",
            row.VietnameseInstruction, $"{row.VietnameseExplanation} Câu hoàn chỉnh là {row.Prompt.Replace("{blank}", row.Options[row.CorrectOptionIndex], StringComparison.Ordinal)}", PublishedAt),
        GrammarExerciseTranslation.Create(bundle.Base + 201 + row.Key * 2, bundle.Base + row.Key, "en",
            row.EnglishInstruction, $"{row.EnglishExplanation} The completed sentence is {row.Prompt.Replace("{blank}", row.Options[row.CorrectOptionIndex], StringComparison.Ordinal)}", PublishedAt)
    })).ToArray();
}
