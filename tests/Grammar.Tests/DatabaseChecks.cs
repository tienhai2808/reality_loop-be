using Domain.Grammar;
using Domain.LearningProgress;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;

namespace Grammar.Tests;

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
        async Task Reject(string sql, string state = PostgresErrorCodes.CheckViolation)
        {
            await using var command = new NpgsqlCommand(sql, connection);
            try { await command.ExecuteNonQueryAsync(); }
            catch (PostgresException ex) when (ex.SqlState == state) { Check.That(true, "Reject invalid SQL: " + sql); return; }
            throw new InvalidOperationException("Invalid SQL accepted: " + sql);
        }
        if (await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public'") != 0)
            throw new InvalidOperationException("The test database must be empty. Existing data will not be deleted.");
        await using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previous = "20261105023639_AddVocabularyLearningProgress";
        await migrator.MigrateAsync(previous);
        db.Users.Add(User.Create(8000, "test@grammar.invalid", "Test", "User", null, DomainChecks.Now));
        db.LearningProfiles.Add(LearningProfile.Create(8001, 8000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Study, null, DomainChecks.Now));
        await db.SaveChangesAsync();
        var language = await db.Languages.SingleAsync(x => x.Code == "ja");
        var system = await db.WritingSystems.SingleAsync(x => x.Code == "hiragana");
        var item = await db.WritingSystemItems.FirstAsync(x => x.WritingSystemId == system.Id);
        db.WritingSystemItemProgress.Add(WritingSystemItemProgress.Create(8003,
            db.LearningProfiles.Local.Single(), item, system, language, DomainChecks.Now));
        var reading = await db.WritingSystemItemReadings.SingleAsync(x => x.WritingSystemItemId == item.Id);
        reading.Update(reading.Transcription, reading.TranscriptionSystem, "https://example.invalid/custom-kana.mp3", 0, DomainChecks.Now);
        await db.SaveChangesAsync();
        await migrator.MigrateAsync();
        db.ChangeTracker.Clear();
        Check.That(await db.WritingSystemItemProgress.AnyAsync(x => x.Id == 8003 && x.Version == 1) &&
            await db.WritingSystemItemReadings.AnyAsync(x => x.Id == reading.Id && x.Version == 2 &&
                x.AudioUrl == "https://example.invalid/custom-kana.mp3"), "Expansion preserves learning progress and customized audio.");
        Check.That(!db.Database.HasPendingModelChanges(), "Model matches snapshot and deterministic seed.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 &&
            await db.HandwritingTemplates.CountAsync() == 208 && await db.VocabularyEntries.CountAsync() == 200,
            "Upgrade preserves private data and other catalog modules.");
        var points = await db.GrammarPoints.AsNoTracking().OrderBy(x => x.Id).ToArrayAsync();
        var translations = await db.GrammarPointTranslations.AsNoTracking().ToArrayAsync();
        Check.That(points.Length == 50 && translations.Length == 100, "Seed contains 50 points and 100 translations.");
        Check.That(points.All(x => x.Version == 1 && x.IsPublished && x.CreatedAt == x.UpdatedAt && x.Pattern.Length > 0), "Published initial seed metadata.");
        Check.That(translations.All(x => x.Version == 1 && x.CreatedAt == x.UpdatedAt) &&
            points.All(p => translations.Where(t => t.GrammarPointId == p.Id).Select(t => t.Locale).Order().SequenceEqual(new[] { "en", "vi" })),
            "Exactly one Vietnamese and English translation per grammar point.");
        var ids = db.GetService<IDesignTimeModel>().Model.GetEntityTypes().SelectMany(x => x.GetSeedData()).Select(x => (long)x["Id"]!).ToArray();
        Check.That(ids.Distinct().Count() == ids.Length, "No seed Snowflake collisions across modules.");
        Check.That(points.Select(x => x.Id).Concat(translations.Select(x => x.Id)).All(id => ((id >> 12) & 1023) == 1023 &&
            (id < 1563703640072384512L ? (id & 4095) is >= 3500 and <= 3577 : (id & 4095) is >= 2000 and <= 2171)), "Reserved Snowflake generator and sequence range.");
        var migrationAssembly = db.GetService<IMigrationsAssembly>();
        var migration = migrationAssembly.CreateMigration(migrationAssembly.Migrations.Single(x => x.Key.EndsWith("_AddGrammarCatalog")).Value, db.Database.ProviderName!);
        var tables = new[] { "grammar_points", "grammar_point_translations" };
        Check.That(migration.UpOperations.All(x => x switch
        {
            CreateTableOperation t => tables.Contains(t.Name), CreateIndexOperation i => tables.Contains(i.Table),
            InsertDataOperation i => tables.Contains(i.Table), _ => false
        }), "Migration only creates grammar schema and seed.");
        Check.That(migration.UpOperations.OfType<CreateIndexOperation>().Count() == 3, "Three useful indexes, no redundant FK-only indexes.");
        foreach (var type in new[] { typeof(GrammarPoint), typeof(GrammarPointTranslation) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never && entity.FindProperty("Version")!.IsConcurrencyToken,
                "Snowflake and concurrency mapping conventions.");
            Check.That(!entity.GetProperties().Any(x => x.IsShadowProperty()), "No shadow properties.");
            Check.That(entity.GetForeignKeys().Single().DeleteBehavior == DeleteBehavior.Cascade, "Catalog FK cascade.");
            Check.That(!entity.GetIndexes().Any(x => x.Properties.Count == 1), "Parent FK prefix already indexed.");
        }
        var point = points[0];
        var translation = translations.First(x => x.GrammarPointId == point.Id && x.Locale == "vi");
        foreach (var assignment in new[]
        {
            "id = 0", "language_id = 0", "version = 0", "sort_order = -1", "updated_at = created_at - interval '1 second'",
            "code = ''", "code = 'UPPER'", "code = '1bad'", "code = 'bad_code'", "native_name = E' \\t '", "pattern = E'\\t'"
        }) await Reject($"UPDATE grammar_points SET {assignment} WHERE id = {point.Id}");
        foreach (var assignment in new[] { "code = repeat('a',65)", "native_name = repeat('a',151)", "pattern = repeat('a',501)" })
            await Reject($"UPDATE grammar_points SET {assignment} WHERE id = {point.Id}", PostgresErrorCodes.StringDataRightTruncation);
        foreach (var assignment in new[]
        {
            "id = 0", "grammar_point_id = 0", "version = 0", "updated_at = created_at - interval '1 second'",
            "locale = 'VI'", "locale = 'en_US'", "locale = 'en--us'", "locale = 'x'", "name = E'\\t'", "explanation = ''", "usage_note = E' \\t '"
        }) await Reject($"UPDATE grammar_point_translations SET {assignment} WHERE id = {translation.Id}");
        foreach (var assignment in new[] { "name = repeat('a',151)", "explanation = repeat('a',8001)", "usage_note = repeat('a',2001)", "locale = repeat('a',36)" })
            await Reject($"UPDATE grammar_point_translations SET {assignment} WHERE id = {translation.Id}", PostgresErrorCodes.StringDataRightTruncation);
        await Reject($"UPDATE grammar_point_translations SET grammar_point_id = 99999 WHERE id = {translation.Id}", PostgresErrorCodes.ForeignKeyViolation);
        await Reject($"UPDATE grammar_points SET language_id = 99999 WHERE id = {point.Id}", PostgresErrorCodes.ForeignKeyViolation);
        await Reject($"UPDATE grammar_points SET code = '{point.Code}' WHERE id = {points[1].Id}", PostgresErrorCodes.UniqueViolation);
        await Reject($"UPDATE grammar_point_translations SET locale = 'en' WHERE id = {translation.Id}", PostgresErrorCodes.UniqueViolation);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE grammar_point_translations SET usage_note = NULL WHERE id = {translation.Id}");
        Check.That((await db.GrammarPointTranslations.AsNoTracking().SingleAsync(x => x.Id == translation.Id)).UsageNote is null, "Nullable usage notes round-trip.");
        foreach (var isTranslation in new[] { false, true })
        {
            await using var first = Context();
            await using var second = Context();
            if (isTranslation)
            {
                var a = await first.GrammarPointTranslations.SingleAsync(x => x.Id == translation.Id);
                var b = await second.GrammarPointTranslations.SingleAsync(x => x.Id == translation.Id);
                a.Update(a.Name, "First", null, DomainChecks.Now);
                b.Update(b.Name, "Second", null, DomainChecks.Now);
            }
            else
            {
                var a = await first.GrammarPoints.SingleAsync(x => x.Id == point.Id);
                var b = await second.GrammarPoints.SingleAsync(x => x.Id == point.Id);
                a.Update(a.NativeName, a.Pattern, false, a.SortOrder, DomainChecks.Now);
                b.Update(b.NativeName, b.Pattern, false, b.SortOrder, DomainChecks.Now);
            }
            await first.SaveChangesAsync();
            try { await second.SaveChangesAsync(); throw new InvalidOperationException("Stale update accepted."); }
            catch (DbUpdateConcurrencyException) { Check.That(true, "Stale update rejected for point/translation."); }
        }
        await using (var atomic = Context())
        {
            atomic.GrammarPoints.Add(GrammarPoint.Create(8100, point.LanguageId, "atomic", "Test", "N", true, 0, DomainChecks.Now));
            atomic.GrammarPointTranslations.AddRange(
                GrammarPointTranslation.Create(8101, 8100, "en", "A", "First", null, DomainChecks.Now),
                GrammarPointTranslation.Create(8102, 8100, "en", "B", "Second", null, DomainChecks.Now));
            try { await atomic.SaveChangesAsync(); throw new InvalidOperationException("Duplicate locale accepted."); }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            { Check.That(true, "Duplicate locale causes graph write failure."); }
        }
        Check.That(!await db.GrammarPoints.AnyAsync(x => x.Id == 8100) && !await db.GrammarPointTranslations.AnyAsync(x => x.GrammarPointId == 8100),
            "Implicit SaveChanges transaction rolls back entire grammar graph.");
        db.Languages.Add(Language.Create(8200, "en", "English", true, 0, DomainChecks.Now));
        db.GrammarPoints.Add(GrammarPoint.Create(8201, 8200, point.Code, "Test", "N", true, 0, DomainChecks.Now));
        db.GrammarPointTranslations.Add(GrammarPointTranslation.Create(8202, 8201, "en-us", "Regional", "Content", null, DomainChecks.Now));
        await db.SaveChangesAsync();
        Check.That(await db.GrammarPoints.CountAsync(x => x.Code == point.Code) == 2, "Code uniqueness is scoped by language.");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM languages WHERE id = 8200");
        Check.That(!await db.GrammarPoints.AnyAsync(x => x.Id == 8201) && !await db.GrammarPointTranslations.AnyAsync(x => x.Id == 8202), "Language cascade removes only its grammar graph.");
        await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM grammar_points WHERE id = {point.Id}");
        Check.That(!await db.GrammarPointTranslations.AnyAsync(x => x.GrammarPointId == point.Id), "Point cascade removes its translations.");
        db.ChangeTracker.Clear();
        await migrator.MigrateAsync(previous);
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name IN ('grammar_points','grammar_point_translations')") == 0 &&
            await db.Users.CountAsync() == 1 && await db.VocabularyEntries.CountAsync() == 40, "Down preserves unrelated user/catalog data.");
        await migrator.MigrateAsync();
        await migrator.MigrateAsync();
        Check.That(await db.GrammarPoints.CountAsync() == 50 && await db.GrammarPointTranslations.CountAsync() == 100 &&
            !db.Database.HasPendingModelChanges(), "Down/up and repeated migrate seed deterministically exactly once.");
    }
}
