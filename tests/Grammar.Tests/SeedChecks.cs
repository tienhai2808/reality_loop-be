using System.Reflection;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Grammar.Tests;

internal static class SeedChecks
{
    public static void Run()
    {
        using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=unused_seed_checks;Username=unused").Options);
        var model = db.GetService<IDesignTimeModel>().Model;
        Dictionary<string, object?>[] Rows(string name) => model.GetEntityTypes()
            .Single(type => type.ClrType.Name == name).GetSeedData(providerValues: true).Select(row => new Dictionary<string, object?>(row)).ToArray();
        var all = model.GetEntityTypes().SelectMany(type => type.GetSeedData(providerValues: true)).ToArray();
        Check.That(all.Select(row => (long)row["Id"]!).Distinct().Count() == all.Length, "All content Snowflake IDs are globally unique.");
        Check.That(all.All(row => ((long)row["Id"]! >> 12 & 1023) == 1023), "Seed uses the reserved Snowflake generator.");
        var systems = Rows("WritingSystem");
        var items = Rows("WritingSystemItem");
        var readings = Rows("WritingSystemItemReading");
        var templates = Rows("HandwritingTemplate");
        Check.That(systems.Length == 2 && items.Length == 208 && readings.Length == 208 && templates.Length == 208,
            "Both writing systems have catalog, reading and handwriting coverage.");
        foreach (var system in systems)
        {
            var selected = items.Where(row => Equals(row["WritingSystemId"], system["Id"])).ToArray();
            Check.That(selected.Length == 104 && selected.Select(row => row["SortOrder"]).Distinct().Count() == 104,
                "Each chart has 104 items in a deterministic order.");
            Check.That(selected.Count(row => ((string)row["Code"]!).StartsWith("kana-r")) == 46 &&
                selected.Count(row => ((string)row["Code"]!).StartsWith("kana-voiced-")) == 25 &&
                selected.Count(row => ((string)row["Code"]!).StartsWith("kana-contracted-")) == 33,
                "Basic, voiced/p and common contracted kana are all present.");
        }
        Check.That(templates.Select(row => row["WritingSystemItemId"]).OrderBy(x => x)
            .SequenceEqual(items.Select(row => row["Id"]).OrderBy(x => x)), "Exactly one template per chart item.");
        foreach (var pair in new[] { ("じ", "ぢ", "ji"), ("ず", "づ", "zu"), ("ジ", "ヂ", "ji"), ("ズ", "ヅ", "zu") })
        {
            var ids = items.Where(row => (string)row["Symbol"]! == pair.Item1 || (string)row["Symbol"]! == pair.Item2)
                .Select(row => row["Id"]).ToArray();
            Check.That(readings.Where(row => ids.Contains(row["WritingSystemItemId"]))
                .All(row => (string)row["Transcription"]! == pair.Item3), "Hepburn preserves valid duplicate readings.");
        }
        Check.That(Rows("VocabularyEntry").Length == 200 && Rows("VocabularySense").Length == 202 &&
            Rows("VocabularySenseTranslation").Length == 404, "Expanded vocabulary has 200 entries and aligned meanings.");
        Check.That(Rows("GrammarPoint").Length == 50 && Rows("PronunciationRule").Length == 12 &&
            Rows("PronunciationExample").Length == 60, "Expanded grammar and pronunciation coverage.");
        foreach (var (parent, translation, foreignKey) in new[]
        {
            ("VocabularySense", "VocabularySenseTranslation", "VocabularySenseId"),
            ("GrammarPoint", "GrammarPointTranslation", "GrammarPointId"),
            ("PronunciationRule", "PronunciationRuleTranslation", "PronunciationRuleId")
        })
            Check.That(Rows(parent).All(row => Rows(translation).Where(t => Equals(t[foreignKey], row["Id"]))
                .Select(t => (string)t["Locale"]!).Order().SequenceEqual(new[] { "en", "vi" })),
                "Every learning concept has exactly one English and Vietnamese translation.");
        var assembly = db.GetService<IMigrationsAssembly>();
        var migration = assembly.CreateMigration(assembly.Migrations.Single(row => row.Key.EndsWith("_ExpandJapaneseLearningSeed")).Value,
            db.Database.ProviderName!);
        Check.That(migration.UpOperations.All(operation => operation is InsertDataOperation or UpdateDataOperation),
            "Expansion migration contains content changes only.");
        Check.That(migration.UpOperations.OfType<UpdateDataOperation>().All(operation =>
            operation.Table is "language_translations" or "writing_system_translations"), "Only catalog descriptions are updated.");
        var previous = assembly.CreateMigration(assembly.Migrations.Single(row => row.Key.EndsWith("_AddGrammarCatalog")).Value,
            db.Database.ProviderName!);
        var previousModel = db.GetService<IModelRuntimeInitializer>().Initialize(previous.TargetModel, designTime: true);
        foreach (var type in previousModel.GetEntityTypes())
        {
            var current = model.GetEntityTypes().Single(row => row.Name == type.Name).GetSeedData(providerValues: true)
                .ToDictionary(row => row["Id"]!);
            foreach (var old in type.GetSeedData(providerValues: true))
            {
                Check.That(current.ContainsKey(old["Id"]!), "Every released content identity is retained.");
                foreach (var field in old)
                {
                    if (type.GetTableName() is "language_translations" or "writing_system_translations" &&
                        field.Key is "Description" or "Version" or "UpdatedAt") continue;
                    Check.That(Equals(current[old["Id"]!][field.Key], field.Value), "Released content fields remain stable.");
                }
            }
        }
        foreach (var type in new[] { "LanguageTranslation", "WritingSystemTranslation" })
            Check.That(Rows(type).All(row => (long)row["Version"]! == 2 &&
                (DateTimeOffset)row["UpdatedAt"]! > (DateTimeOffset)row["CreatedAt"]!), "Edited descriptions advance version and timestamp.");
        var loader = typeof(ApplicationDbContext).Assembly.GetType("Infrastructure.Persistence.Seeding.Audio.FoundationAudioCatalog")!
            .GetMethod("Load", BindingFlags.Static | BindingFlags.Public)!;
        var audio = (Array)loader.Invoke(null, null)!;
        Check.That(audio.Length == 71, "All 71 embedded recordings pass mapping, license and checksum validation.");
        Check.That(!db.Database.HasPendingModelChanges(), "Deterministic content matches the generated migration snapshot.");
    }
}
