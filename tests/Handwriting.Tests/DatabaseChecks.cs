using Domain.Handwriting;
using Domain.LearningProfiles;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;

namespace Handwriting.Tests;

internal static class DatabaseChecks
{
    public static async Task RunAsync(string database)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(database,
            provider => provider.MigrationsHistoryTable("__ef_migrations_history")).Options;
        ApplicationDbContext Context() => new(options);
        await using var connection = new NpgsqlConnection(database);
        await connection.OpenAsync();
        async Task<long> Scalar(string sql)
        {
            await using var command = new NpgsqlCommand(sql, connection);
            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }
        async Task Reject(string sql, string state, string description)
        {
            await using var command = new NpgsqlCommand(sql, connection);
            try { await command.ExecuteNonQueryAsync(); }
            catch (PostgresException ex) when (ex.SqlState == state) { Check.That(true, description); return; }
            throw new InvalidOperationException(description);
        }
        if (await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public'") != 0)
            throw new InvalidOperationException("The handwriting test database must be empty. Existing data will not be deleted.");

        await using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previous = "20261104064528_AddVocabularyCatalog";
        await migrator.MigrateAsync(previous);
        var now = DomainChecks.Now;
        var profile = LearningProfile.Create(7001, 7000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        db.Users.Add(User.Create(7000, "test@handwriting.invalid", "Test", "User", null, now));
        db.LearningProfiles.Add(profile);
        await db.SaveChangesAsync();
        await migrator.MigrateAsync("20261104142057_AddHandwritingPractice");
        var legacy = await db.HandwritingTemplates.AsNoTracking().OrderBy(x => x.Id).ToArrayAsync();
        var legacyJson = System.Text.Json.JsonSerializer.Serialize(legacy);
        Check.That(legacy.Length == 4, "Previous migration contains four legacy templates.");
        await migrator.MigrateAsync();
        Check.That(System.Text.Json.JsonSerializer.Serialize(await db.HandwritingTemplates.AsNoTracking().OrderBy(x => x.Id).Take(4).ToArrayAsync()) == legacyJson,
            "Coverage upgrade leaves every legacy template field/point unchanged.");
        Check.That(!db.Database.HasPendingModelChanges(), "Runtime model matches snapshot and deterministic JSON seed.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1, "Upgrade preserves private data.");
        Check.That(await db.VocabularyEntries.CountAsync() == 40 && await db.WritingSystemItems.CountAsync() == 92, "Upgrade preserves existing catalog.");

        var templates = await db.HandwritingTemplates.OrderBy(x => x.Id).ToArrayAsync();
        Check.That(templates.Length == 92 && string.Concat(templates.Take(4).Select(x => x.Symbol)) == "あおぬめ", "Full kana coverage with stable legacy identities.");
        Check.That(templates.All(x => x.Version == 1 && x.Revision == 1 && x.IsPublished && x.DrawingFormatVersion == 1), "Initial template versions/publication.");
        Check.That(templates.All(x => x.SourceRevision == "70a0b7ae0c18ceb5cb358274b029cce0234a43bc" && x.License == "CC BY-SA 3.0" && x.Attribution.Contains("Ulrich Apel")), "Pinned source/license/attribution retained.");
        Check.That(templates.Take(4).Select(x => x.Drawing.Strokes.Count).SequenceEqual(new[] { 3, 3, 2, 2 }), "Original ordered stroke counts round-trip.");
        Check.That(templates.All(x => ((x.Id >> 12) & 1023) == 1023 && ((x.Id & 4095) is >= 3000 and <= 3003 or >= 3100 and <= 3145 or >= 3200 and <= 3245)), "Stable Snowflake allocation.");
        var catalogItems = await db.WritingSystemItems.AsNoTracking().ToArrayAsync();
        Check.That(templates.Select(x => x.WritingSystemItemId).Order().SequenceEqual(catalogItems.Select(x => x.Id).Order()),
            "Exactly one reference covers every one of 92 catalog items.");
        var grader = new Application.Handwriting.Scoring.HandwritingGrader(new Application.Handwriting.Scoring.HandwritingScoringPolicy());
        foreach (var reference in templates)
        {
            var feedback = grader.Grade(reference.Drawing, reference.Drawing);
            Check.That(feedback.Passed && feedback.MatchScore == 100, $"Reference self-grade: {reference.Symbol}.");
        }
        var seedIds = db.GetService<IDesignTimeModel>().Model.GetEntityTypes().SelectMany(x => x.GetSeedData()).Select(x => (long)x["Id"]!).ToArray();
        Check.That(seedIds.Distinct().Count() == seedIds.Length, "No seed ID collision across all modules.");
        var migrationAssembly = db.GetService<IMigrationsAssembly>();
        var coverageMigration = migrationAssembly.CreateMigration(migrationAssembly.Migrations
            .Single(x => x.Key.EndsWith("_CompleteKanaHandwritingTemplates")).Value, db.Database.ProviderName!);
        Check.That(coverageMigration.UpOperations.All(x => x is InsertDataOperation { Table: "handwriting_templates" }) &&
            coverageMigration.UpOperations.OfType<InsertDataOperation>().Sum(x => x.Values.GetLength(0)) == 88,
            "Coverage migration only inserts 88 new templates, without schema changes or legacy updates.");
        var migrationType = migrationAssembly.Migrations.Single(x => x.Key.EndsWith("_AddHandwritingPractice")).Value;
        var migration = migrationAssembly.CreateMigration(migrationType, db.Database.ProviderName!);
        var tables = new[] { "handwriting_templates", "handwriting_practice_attempts", "handwriting_practice_characters" };
        Check.That(migration.UpOperations.All(x => x switch
        {
            CreateTableOperation op => tables.Contains(op.Name),
            CreateIndexOperation op => tables.Contains(op.Table),
            InsertDataOperation op => op.Table == "handwriting_templates",
            _ => false
        }), "Migration only affects handwriting schema/seed.");
        Check.That(migration.UpOperations.OfType<CreateIndexOperation>().Count() == 5, "Only five useful indexes, no redundant FK prefixes/JSON GIN indexes.");
        foreach (var type in new[] { typeof(HandwritingTemplate), typeof(HandwritingPracticeAttempt), typeof(HandwritingPracticeCharacter) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(!entity.GetProperties().Any(x => x.IsShadowProperty()), $"No shadow properties: {type.Name}.");
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never, $"Snowflake caller supplied: {type.Name}.");
            Check.That(entity.FindProperty("Version")!.IsConcurrencyToken && entity.FindProperty("Version")!.ValueGenerated == ValueGenerated.Never, $"Version convention: {type.Name}.");
        }
        Check.That(db.Model.FindEntityType(typeof(HandwritingTemplate))!.FindProperty("Drawing")!.GetColumnType() == "jsonb", "Geometry uses JSONB.");

        var language = await db.Languages.SingleAsync(x => x.Code == "ja");
        var system = await db.WritingSystems.SingleAsync(x => x.Code == "hiragana");
        var item = await db.WritingSystemItems.SingleAsync(x => x.Id == templates[0].WritingSystemItemId);
        var template = templates[0];
        var answer = new HandwritingPracticeAnswer(7011, template, item, template.Drawing, DomainChecks.Feedback(3));
        var attempt = HandwritingPracticeAttempt.Create(7010, profile, system, language, "geometry-v1", [answer, answer with { Id = 7012 }], now);
        db.HandwritingPracticeAttempts.Add(attempt);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var loaded = await db.HandwritingPracticeAttempts.Include(x => x.Characters).SingleAsync(x => x.Id == attempt.Id);
        Check.That(loaded.Characters.Count == 2 && loaded.TargetText == "ああ" && loaded.Passed && loaded.MatchScore == 99, "Full immutable aggregate round-trip, including repeated letters.");
        var character = loaded.Characters.OrderBy(x => x.Position).First();
        Check.That(character.Drawing.Strokes[2].SequenceEqual(template.Drawing.Strokes[2]), "Raw points retain precision/order after JSONB conversion.");
        Check.That(character.Feedback.Passed && character.Feedback.Strokes.Count == 3 && character.Feedback.Strokes.All(x => x.Passed), "Detailed evaluated feedback round-trip.");
        Check.That(!db.ChangeTracker.HasChanges(), "Immutable JSON value objects do not cause spurious writes after load.");
        var rejectedAnswer = answer with { Id = 7021, Drawing = new InkDrawing([]), Feedback = HandwritingFeedback.Rejected(3, 0, HandwritingIssue.MissingDrawing) };
        db.HandwritingPracticeAttempts.Add(HandwritingPracticeAttempt.Create(7020, profile, system, language, "geometry-v1", [rejectedAnswer], now));
        foreach (var (issue, key) in new[] { (HandwritingIssue.WrongShape, 7030), (HandwritingIssue.WrongDirection, 7040), (HandwritingIssue.WrongStrokeOrder, 7050) })
            db.HandwritingPracticeAttempts.Add(HandwritingPracticeAttempt.Create(key, profile, system, language, "geometry-v1",
                [answer with { Id = key + 1, Feedback = DomainChecks.Feedback(3, issue) }], now));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var blank = await db.HandwritingPracticeCharacters.SingleAsync(x => x.Id == 7021);
        Check.That(blank.Drawing.Strokes.Count == 0 && !blank.Feedback.Passed && blank.Feedback.Issues.Single() == HandwritingIssue.MissingDrawing, "Empty drawing/rejected feedback round-trip.");
        Check.That((await db.HandwritingPracticeCharacters.SingleAsync(x => x.Id == 7031)).Feedback.Issues.Single() == HandwritingIssue.WrongShape, "Issue names persisted without integer enum values.");

        foreach (var (assignment, state) in new[]
        {
            ("id = 0", PostgresErrorCodes.CheckViolation), ("writing_system_item_id = -1", PostgresErrorCodes.CheckViolation),
            ("revision = 0", PostgresErrorCodes.CheckViolation), ("version = 0", PostgresErrorCodes.CheckViolation),
            ("updated_at = created_at - interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("symbol = E' \\t '", PostgresErrorCodes.CheckViolation), ("source_revision = ''", PostgresErrorCodes.CheckViolation),
            ("attribution = ''", PostgresErrorCodes.CheckViolation), ("license = ''", PostgresErrorCodes.CheckViolation),
            ("source_url = 'http://example.test'", PostgresErrorCodes.CheckViolation),
            ("license_url = 'https://user@example.test'", PostgresErrorCodes.CheckViolation),
            ("drawing_format_version = 2", PostgresErrorCodes.CheckViolation),
            ("drawing = '[]'", PostgresErrorCodes.CheckViolation), ("drawing = '[[]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = 'null'", PostgresErrorCodes.CheckViolation), ("drawing = '{}'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[{\"x\":0,\"y\":1}]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[{\"x\":-1,\"y\":1},{\"x\":0,\"y\":0}]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[{\"x\":0,\"y\":2},{\"x\":0,\"y\":0}]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[{\"x\":\"0\",\"y\":1},{\"x\":0,\"y\":0}]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[{\"x\":0},{\"x\":0,\"y\":0}]]'", PostgresErrorCodes.CheckViolation),
            ("drawing = '[[[0,1],[1,0]]]'", PostgresErrorCodes.CheckViolation),
            ("writing_system_item_id = 999999", PostgresErrorCodes.ForeignKeyViolation)
        }) await Reject($"UPDATE handwriting_templates SET {assignment} WHERE id = {template.Id}", state, $"Invalid template rejected: {assignment}");
        foreach (var assignment in new[]
        {
            "id = -1", "learning_profile_id = 0", "writing_system_id = 0", "character_count = 0", "character_count = 5",
            "match_score = -1", "match_score = 101", "version = 2", "updated_at = created_at + interval '1 second'",
            "algorithm_version = 'Geometry V1'", "target_text = ''"
        }) await Reject($"UPDATE handwriting_practice_attempts SET {assignment} WHERE id = 7010", PostgresErrorCodes.CheckViolation, $"Invalid attempt rejected: {assignment}");
        foreach (var assignment in new[]
        {
            "id = 0", "attempt_id = 0", "template_id = 0", "position = -1", "position = 4", "version = 2",
            "updated_at = created_at - interval '1 second'", "drawing_format_version = 2", "drawing = 'null'",
            "drawing = '[[]]'", "feedback = '{}'", "feedback = 'null'",
            "feedback = jsonb_set(feedback, '{matchScore}', '101')",
            "feedback = jsonb_set(feedback, '{expectedStrokeCount}', '0')",
            "feedback = jsonb_set(feedback, '{actualStrokeCount}', '2')",
            "feedback = jsonb_set(feedback, '{passed}', 'false')",
            "feedback = jsonb_set(feedback, '{issues}', '[\"Unknown\"]')",
            "feedback = jsonb_set(feedback, '{strokes,0,issues}', '[\"0\"]')",
            "feedback = jsonb_set(feedback, '{strokes,0,distance}', '-0.5')",
            "feedback = jsonb_set(feedback, '{strokes,0,lengthRatio}', '-1')",
            "feedback = feedback #- '{strokes,0,index}'", "feedback = feedback #- '{strokes,0,distance}'",
            "feedback = feedback #- '{strokes,0,issues}'", "feedback = feedback - 'passed'",
            "feedback = jsonb_set(feedback, '{strokes,0,index}', '1')",
            "feedback = jsonb_set(feedback, '{strokes,0,passed}', 'false')"
        }) await Reject($"UPDATE handwriting_practice_characters SET {assignment} WHERE id = {character.Id}", PostgresErrorCodes.CheckViolation, $"Invalid character rejected: {assignment}");
        await Reject($"UPDATE handwriting_practice_characters SET position = 1 WHERE id = {character.Id}", PostgresErrorCodes.UniqueViolation, "Duplicate attempt position rejected.");
        await Reject($"UPDATE handwriting_practice_characters SET template_id = 999999 WHERE id = {character.Id}", PostgresErrorCodes.ForeignKeyViolation, "Missing template rejected.");
        await Reject($"DELETE FROM handwriting_templates WHERE id = {template.Id}", PostgresErrorCodes.ForeignKeyViolation, "History prevents reference removal.");
        await Reject($"DELETE FROM writing_system_items WHERE id = {item.Id}", PostgresErrorCodes.ForeignKeyViolation, "Reference prevents catalog removal.");
        var duplicate = HandwritingTemplate.Create(7099, item, 1, template.Drawing, template.SourceUrl, template.SourceRevision,
            template.Attribution, template.License, template.LicenseUrl, true, now);
        await using (var duplicateDb = Context())
        {
            duplicateDb.HandwritingTemplates.Add(duplicate);
            try { await duplicateDb.SaveChangesAsync(); throw new InvalidOperationException("Duplicate revision accepted."); }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            { Check.That(true, "Unique source item/revision enforced."); }
        }
        await using (var first = Context())
        await using (var second = Context())
        {
            var a = await first.HandwritingTemplates.SingleAsync(x => x.Id == template.Id);
            var b = await second.HandwritingTemplates.SingleAsync(x => x.Id == template.Id);
            a.SetPublished(false, now.AddSeconds(1));
            b.SetPublished(false, now.AddSeconds(2));
            await first.SaveChangesAsync();
            try { await second.SaveChangesAsync(); throw new InvalidOperationException("Stale publication accepted."); }
            catch (DbUpdateConcurrencyException) { Check.That(true, "Stale publication fails optimistic concurrency."); }
            Check.That((await db.HandwritingTemplates.AsNoTracking().SingleAsync(x => x.Id == template.Id)).Version == 2, "Winning publication retained.");
        }
        // SaveChanges must atomically insert a root and all characters: force a duplicate child PK midway.
        var template2 = templates[1];
        var item2 = await db.WritingSystemItems.SingleAsync(x => x.Id == template2.WritingSystemItemId);
        var badAttempt = HandwritingPracticeAttempt.Create(7080, profile, system, language, "geometry-v1",
            [new(7081, template2, item2, template2.Drawing, DomainChecks.Feedback(3)), new(character.Id, template2, item2, template2.Drawing, DomainChecks.Feedback(3))], now);
        await using (var atomic = Context())
        {
            atomic.HandwritingPracticeAttempts.Add(badAttempt);
            try { await atomic.SaveChangesAsync(); throw new InvalidOperationException("Duplicate child accepted."); }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            { Check.That(true, "Duplicate child fails aggregate write."); }
        }
        Check.That(await Scalar("SELECT count(*) FROM handwriting_practice_attempts WHERE id = 7080") == 0 &&
                   await Scalar("SELECT count(*) FROM handwriting_practice_characters WHERE id = 7081") == 0, "Failed SaveChanges rolls back root and all newly inserted children.");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM learning_profiles WHERE id = 7001");
        Check.That(await db.HandwritingPracticeAttempts.CountAsync() == 0 && await db.HandwritingPracticeCharacters.CountAsync() == 0, "Profile deletion cascades only private handwriting graph.");
        Check.That(await db.HandwritingTemplates.CountAsync() == 92 && await db.Users.CountAsync() == 1, "Catalog and user unaffected by profile removal.");

        db.ChangeTracker.Clear();
        await migrator.MigrateAsync("20261104142057_AddHandwritingPractice");
        Check.That(await db.HandwritingTemplates.CountAsync() == 4, "Coverage down preserves all four legacy templates.");
        await migrator.MigrateAsync();
        Check.That(await db.HandwritingTemplates.CountAsync() == 92, "Coverage down/up restores exactly 88 added references.");
        await migrator.MigrateAsync(previous);
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name LIKE 'handwriting_%'") == 0, "Down removes handwriting tables.");
        Check.That(await db.Users.CountAsync() == 1 && await db.VocabularyEntries.CountAsync() == 40, "Down preserves previous modules.");
        await migrator.MigrateAsync();
        await migrator.MigrateAsync();
        Check.That(await db.HandwritingTemplates.CountAsync() == 92 && await db.HandwritingPracticeAttempts.CountAsync() == 0, "Reapply and repeated migrate seed exactly once.");
        Check.That(!db.Database.HasPendingModelChanges(), "Model still matches snapshot after down/up.");
    }
}
