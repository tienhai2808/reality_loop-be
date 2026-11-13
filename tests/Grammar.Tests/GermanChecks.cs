using System.Net.Http.Json;
using System.Text.Json;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Grammar.Tests;

internal static class GermanChecks
{
    private const long GermanId = 1563703640076578816L;

    public static void RunModelChecks()
    {
        using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused").Options);
        var model = db.GetService<IDesignTimeModel>().Model;
        var items = model.FindEntityType(typeof(Domain.WritingSystems.WritingSystemItem))!.GetSeedData().Where(row => (long)row["WritingSystemId"]! == GermanId + 10).ToArray();
        Check.That(items.Length == 60 && items.Select(row => (string)row["Symbol"]!).Distinct().Count() == 60,
            "German has 60 distinct upper/lower case items, including umlauts and eszett.");
        Check.That(new[] { "Ä", "Ö", "Ü", "ẞ", "ä", "ö", "ü", "ß" }.All(symbol => items.Any(row => (string)row["Symbol"]! == symbol)),
            "NFC special characters retain their case and identity.");
        var migrationAssembly = db.GetService<IMigrationsAssembly>();
        var migration = migrationAssembly.CreateMigration(migrationAssembly.Migrations.Single(row => row.Key.EndsWith("_AddGermanLearningContent")).Value,
            db.Database.ProviderName!);
        Check.That(migration.UpOperations.All(operation => operation is InsertDataOperation) &&
            migration.UpOperations.Cast<InsertDataOperation>().Sum(operation => operation.Values.GetLength(0)) == 1232,
            "German migration inserts only its 1232 catalog rows.");
        var previous = migrationAssembly.CreateMigration(migrationAssembly.Migrations.Single(row => row.Key.EndsWith("_ExpandJapaneseLearningSeed")).Value,
            db.Database.ProviderName!);
        var oldModel = db.GetService<IModelRuntimeInitializer>().Initialize(previous.TargetModel, designTime: true);
        foreach (var type in oldModel.GetEntityTypes())
        {
            var current = model.GetEntityTypes().Single(entity => entity.Name == type.Name).GetSeedData(providerValues: true).ToDictionary(row => row["Id"]!);
            foreach (var old in type.GetSeedData(providerValues: true))
                Check.That(old.All(field => Equals(current[old["Id"]!][field.Key], field.Value)), "Every released Japanese seed field is preserved.");
        }
    }

    public static async Task RunApiChecks(string database)
    {
        await using var app = TestApiHost.Build(database);
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.Add(User.Create(93000, "multilang@seed.invalid", "Multi", "Language", null, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var http = TestApiHost.Client(app);
        http.DefaultRequestHeaders.Authorization = new("Bearer", "grammar-test-multilang");
        async Task<JsonElement> Send(string method, string path, object? body = null, int status = 200, string? actor = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1" + path);
            if (body is not null) request.Content = JsonContent.Create(body);
            if (actor is not null) request.Headers.Authorization = new("Bearer", actor);
            using var response = await http.SendAsync(request);
            var text = await response.Content.ReadAsStringAsync();
            Check.That((int)response.StatusCode == status, $"{method} {path} expected {status}, got {(int)response.StatusCode}: {text}");
            return string.IsNullOrEmpty(text) ? default : JsonDocument.Parse(text).RootElement.Clone();
        }
        var languages = await Send("GET", "/languages?locale=vi&pageSize=100");
        Check.That(languages.GetProperty("items").EnumerateArray().Select(row => row.GetProperty("code").GetString()).ToHashSet().SetEquals(["ja", "de"]),
            "FE language discovery returns both published language catalogs.");
        var deProfile = await Send("POST", "/learning-profiles", new
        {
            targetLanguage = " DE ", nativeLanguage = "vi", explanationLanguage = "vi", proficiencyLevel = "Beginner", primaryGoal = "Study"
        }, 201);
        var jaProfile = await Send("POST", "/learning-profiles", new
        {
            targetLanguage = "ja", nativeLanguage = "vi", explanationLanguage = "vi", proficiencyLevel = "Beginner", primaryGoal = "Study"
        }, 201);
        var deId = deProfile.GetProperty("id").GetString()!;
        var jaId = jaProfile.GetProperty("id").GetString()!;
        foreach (var id in new[] { deId, jaId, deId })
        {
            var current = await Send("PUT", "/learning-profiles/current", new { profileId = id });
            Check.That(current.GetProperty("profile").GetProperty("id").GetString() == id, "Selection switches between language profiles.");
        }
        Check.That((await Send("GET", "/learning-profiles/" + deId)).GetProperty("version").GetInt64() == 1,
            "Selection changes do not modify profile preferences.");
        var systems = await Send("GET", "/languages/de/writing-systems?locale=vi");
        Check.That(systems.GetProperty("items").GetArrayLength() == 1 && systems.GetProperty("items")[0].GetProperty("code").GetString() == "latin",
            "German discovers its Latin chart through the existing writing-system contract.");
        var items = await Send("GET", "/languages/de/writing-systems/latin/items?pageSize=100");
        var letterItems = items.GetProperty("items").EnumerateArray().ToArray();
        Check.That(letterItems.Length == 60 && items.GetProperty("nextCursor").ValueKind == JsonValueKind.Null, "All German letters fit in one catalog page.");
        Check.That(letterItems.All(item => item.GetProperty("primaryReading").GetProperty("transcriptionSystem").GetString() == "ipa" &&
            item.GetProperty("primaryReading").GetProperty("audioUrl").GetString()!.Contains("/foundation/de/", StringComparison.Ordinal)),
            "German letter names use IPA and German audio targets.");
        var seenWords = new HashSet<string>();
        string? after = null;
        do
        {
            var page = await Send("GET", "/languages/de/vocabulary?locale=vi&pageSize=37" + (after is null ? "" : "&after=" + after));
            foreach (var word in page.GetProperty("items").EnumerateArray())
                Check.That(seenWords.Add(word.GetProperty("id").GetString()!), "German word pagination has no duplicate identity.");
            after = page.GetProperty("nextCursor").GetString();
        } while (after is not null);
        Check.That(seenWords.Count == 200, "All 200 German words are reachable.");
        var meanings = await Send("GET", "/languages/de/vocabulary/man/senses?locale=vi");
        var sense = meanings.GetProperty("items")[0];
        var note = sense.GetProperty("usageNote").GetString()!;
        Check.That(note.Contains("der Mann", StringComparison.Ordinal) && note.Contains("Männer", StringComparison.Ordinal),
            "German cards expose noun article and plural notes.");
        var grammar = await Send("GET", "/languages/de/grammar-points?locale=vi&pageSize=100");
        Check.That(grammar.GetProperty("items").GetArrayLength() == 50, "All German grammar points are discoverable.");
        var rule = await Send("GET", "/languages/de/grammar-points/definite-articles?locale=vi");
        Check.That(rule.GetProperty("explanation").GetString()!.Contains("der Tisch", StringComparison.Ordinal), "Grammar has localized German examples.");
        var pronunciation = await Send("GET", "/languages/de/pronunciation-rules?locale=vi&pageSize=100");
        Check.That(pronunciation.GetProperty("items").GetArrayLength() == 12, "All German pronunciation rules are localized and available.");
        var pronunciationExamples = await Send("GET", "/languages/de/pronunciation-rules/umlauts/examples?pageSize=100");
        Check.That(pronunciationExamples.GetProperty("items").GetArrayLength() == 5 &&
            pronunciationExamples.GetProperty("items").EnumerateArray().All(example => example.GetProperty("transcriptionSystem").GetString() == "ipa"),
            "Pronunciation examples preserve IPA without a Japanese transcription dependency.");
        var deRoot = "/learning-profiles/" + deId;
        var jaRoot = "/learning-profiles/" + jaId;
        var a = letterItems.Single(item => item.GetProperty("code").GetString() == "upper-a").GetProperty("id").GetString()!;
        await Send("POST", deRoot + "/writing-systems/latin/items/" + a + "/progress", new { action = "Complete", version = 0 });
        var deSummary = await Send("GET", deRoot + "/writing-system-progress");
        var jaSummary = await Send("GET", jaRoot + "/writing-system-progress");
        Check.That(deSummary.GetProperty("writingSystems")[0].GetProperty("totalItems").GetInt32() == 60 &&
            deSummary.GetProperty("writingSystems")[0].GetProperty("completedItems").GetInt32() == 1 &&
            jaSummary.GetProperty("writingSystems").EnumerateArray().All(system => system.GetProperty("completedItems").GetInt32() == 0),
            "Study progress remains isolated by language profile.");
        await Send("GET", jaRoot + "/writing-systems/hiragana/items/" + a + "/progress", status: 404);
        var deSenseId = sense.GetProperty("id").GetString()!;
        var progress = await Send("PUT", deRoot + "/vocabulary-senses/" + deSenseId + "/learning-progress", status: 201);
        await Send("PUT", jaRoot + "/vocabulary-senses/" + deSenseId + "/learning-progress", status: 404);
        var cardId = progress.GetProperty("id").GetString();
        var card = await Send("GET", deRoot + "/vocabulary-learning/" + cardId + "?locale=vi");
        Check.That(card.GetProperty("term").GetString() == "Mann", "The review card uses the correct language's term.");
        await Send("POST", deRoot + "/vocabulary-learning/" + cardId + "/reviews",
            new { requestId = Guid.NewGuid(), version = 1, rating = "Good" });
        Check.That((await Send("GET", jaRoot + "/vocabulary-learning/summary")).GetProperty("total").GetInt32() == 0,
            "German vocabulary reviews do not enter the Japanese queue.");
        var session = await Send("POST", deRoot + "/writing-systems/latin/practice-sessions", new { questionCount = 20 }, 201);
        Dictionary<string, string> correctReadings = letterItems.ToDictionary(item => item.GetProperty("id").GetString()!,
            item => item.GetProperty("primaryReading").GetProperty("transcription").GetString()!);
        foreach (var question in session.GetProperty("items").EnumerateArray().ToArray())
        {
            var correct = correctReadings[question.GetProperty("writingSystemItemId").GetString()!];
            var options = question.GetProperty("options").EnumerateArray().Select(value => value.GetString()).ToArray();
            Check.That(question.GetProperty("transcriptionSystem").GetString() == "ipa" && options.Count(option => option == correct) == 1,
                "IPA distractors retain exactly one correct letter-name option.");
            session = await Send("POST", deRoot + "/practice-sessions/" + session.GetProperty("session").GetProperty("id").GetString() +
                "/items/" + question.GetProperty("id").GetString() + "/answer", new
                {
                    selectedOptionIndex = Array.IndexOf(options, correct), version = session.GetProperty("session").GetProperty("version").GetInt64()
                });
        }
        Check.That(session.GetProperty("session").GetProperty("status").GetString() == "Completed", "German practice completes through the normal API.");
        await Send("GET", jaRoot + "/practice-sessions/" + session.GetProperty("session").GetProperty("id").GetString(), status: 404);
        var templates = await Send("GET", deRoot + "/writing-systems/latin/handwriting-templates?pageSize=100");
        Check.That(templates.GetProperty("items").GetArrayLength() == 60, "German has complete letter reference coverage.");
        string? firstTemplate = null;
        foreach (var template in templates.GetProperty("items").EnumerateArray())
        {
            var templateId = template.GetProperty("reference").GetProperty("id").GetString()!;
            firstTemplate ??= templateId;
            var drawing = await Send("GET", deRoot + "/writing-systems/latin/handwriting-templates/" + templateId);
            var attempt = await Send("POST", deRoot + "/writing-systems/latin/handwriting-attempts", new
            {
                characters = new[] { new { templateId, strokes = drawing.GetProperty("strokes") } }
            }, 201);
            Check.That(attempt.GetProperty("attempt").GetProperty("passed").GetBoolean(), "German reference passes its server grading round-trip.");
        }
        await Send("POST", jaRoot + "/writing-systems/hiragana/handwriting-attempts",
            new { characters = new[] { new { templateId = firstTemplate, strokes = Array.Empty<object>() } } }, status: 404);
        await Send("GET", deRoot + "/handwriting-attempts", status: 404, actor: "grammar-test-b");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                await db.GetService<IMigrator>().MigrateAsync("20261107120741_ExpandJapaneseLearningSeed");
                throw new InvalidOperationException("Rollback must protect live German profiles.");
            }
            catch (PostgresException exception) when (exception.SqlState == "P0001")
            {
                Check.That(await db.HandwritingPracticeAttempts.CountAsync(attempt => attempt.LearningProfileId == long.Parse(deId)) == 60 &&
                    await db.VocabularyReviews.CountAsync(review => review.ProgressId == long.Parse(cardId!)) == 1,
                    "Rejected rollback preserves German attempts and vocabulary review history.");
            }
            await db.Users.Where(user => user.Id == 93000).ExecuteDeleteAsync();
            Check.That(await db.VocabularyEntries.CountAsync() == 400 && await db.HandwritingTemplates.CountAsync() == 268,
                "Deleting a learner preserves both shared language catalogs.");
        }
    }
}
