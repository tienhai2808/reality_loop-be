using Domain.Languages;
using Domain.LearningProfiles;
using Domain.LearningProgress;
using Domain.Users;
using Domain.Vocabulary;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;

namespace Vocabulary.Tests;

internal static class DatabaseChecks
{
    public static async Task RunAsync(string database)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(database).Options;
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
            throw new InvalidOperationException("The vocabulary test database must be empty. No existing data will be deleted.");
        await using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previousMigration = "20261103121729_AddWritingSystemPracticeSessions";
        await migrator.MigrateAsync(previousMigration);
        var now = new DateTimeOffset(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);
        var japanese = await db.Languages.SingleAsync(x => x.Code == "ja");
        var system = await db.WritingSystems.SingleAsync(x => x.Code == "hiragana");
        var item = await db.WritingSystemItems.FirstAsync(x => x.WritingSystemId == system.Id);
        var profile = LearningProfile.Create(4501, 4500, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        db.Users.Add(User.Create(4500, "test@vocabulary.invalid", "Test", "User", null, now));
        db.LearningProfiles.Add(profile);
        db.WritingSystemItemProgress.Add(WritingSystemItemProgress.Create(4502, profile, item, system, japanese, now));
        await db.SaveChangesAsync();
        await migrator.MigrateAsync();
        Check.That(!db.Database.HasPendingModelChanges(), "Runtime model matches snapshot, including deterministic seed.");
        Check.That(await db.VocabularyEntries.CountAsync() == 40 && await db.VocabularySenses.CountAsync() == 42 &&
                   await db.VocabularySenseTranslations.CountAsync() == 84, "Migration seeds 40 entries, 42 senses, 84 translations.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 && await db.WritingSystemItemProgress.CountAsync() == 1,
            "Upgrade preserves existing user/profile/progress data.");
        Check.That(await db.WritingSystemItems.CountAsync() == 92 && await db.PronunciationRules.CountAsync() == 5, "Upgrade preserves old catalog seed.");

        var entries = await db.VocabularyEntries.AsNoTracking().OrderBy(x => x.Id).ToArrayAsync();
        var senses = await db.VocabularySenses.AsNoTracking().OrderBy(x => x.Id).ToArrayAsync();
        var translations = await db.VocabularySenseTranslations.AsNoTracking().OrderBy(x => x.Id).ToArrayAsync();
        var seedIds = entries.Select(x => x.Id).Concat(senses.Select(x => x.Id)).Concat(translations.Select(x => x.Id)).ToArray();
        Check.That(seedIds.Distinct().Count() == seedIds.Length && seedIds.All(x => x > 0), "Seed Snowflake IDs are positive and globally distinct within vocabulary.");
        var allSeedIds = db.GetService<IDesignTimeModel>().Model.GetEntityTypes().SelectMany(x => x.GetSeedData())
            .Select(row => (long)row["Id"]!).ToArray();
        Check.That(allSeedIds.Distinct().Count() == allSeedIds.Length, "Vocabulary Snowflakes do not overlap any existing seed allocation.");
        Check.That(seedIds.All(x => ((x >> 12) & 1023) == 1023 && (x & 4095) is >= 2000 and <= 2283), "Seed uses the reserved generator/sequence range.");
        Check.That(entries.All(x => x.LanguageId == japanese.Id && x.IsPublished && x.AudioUrl is null && x.Reading is not null && x.Version == 1), "Published Japanese entries have reading, nullable audio and initial versions.");
        var publishedAt = new DateTimeOffset(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);
        Check.That(entries.All(x => x.CreatedAt == publishedAt && x.UpdatedAt == publishedAt) &&
                   senses.All(x => x.CreatedAt == publishedAt && x.UpdatedAt == publishedAt && x.Version == 1) &&
                   translations.All(x => x.CreatedAt == publishedAt && x.UpdatedAt == publishedAt && x.Version == 1), "Seed timestamps/versions are fixed, not generated by clock.");
        Check.That(senses.All(sense => translations.Count(x => x.VocabularySenseId == sense.Id && x.Locale == "vi") == 1 &&
                                       translations.Count(x => x.VocabularySenseId == sense.Id && x.Locale == "en") == 1), "Every sense has one aligned Vietnamese/English pair.");
        Check.That(!await db.Languages.AnyAsync(x => x.Code == "vi" || x.Code == "en"), "Explanation locales do not require target-language catalog rows.");
        var gohanId = entries.Single(x => x.Code == "gohan").Id;
        var kikuId = entries.Single(x => x.Code == "kiku").Id;
        Check.That(senses.Where(x => x.VocabularyEntryId == gohanId).Select(x => x.Code).ToHashSet().SetEquals(["cooked-rice", "meal"]), "Gohan has separate rice/meal senses.");
        Check.That(senses.Where(x => x.VocabularyEntryId == kikuId).Select(x => x.Code).ToHashSet().SetEquals(["listen", "ask"]), "Kiku has separate listening/asking senses.");
        Check.That(senses.All(s => entries.Any(e => e.Id == s.VocabularyEntryId)) && translations.All(t => senses.Any(s => s.Id == t.VocabularySenseId)), "All seed ancestry references resolve.");
        await migrator.MigrateAsync(previousMigration);
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_name LIKE 'vocabulary_%'") == 0, "Down removes exactly the vocabulary tables.");
        Check.That(await db.Users.CountAsync() == 1 && await db.WritingSystemItemProgress.CountAsync() == 1 && await db.WritingSystemItems.CountAsync() == 92,
            "Down preserves unrelated data and schema.");
        await migrator.MigrateAsync();
        await migrator.MigrateAsync();
        var reappliedIds = (await db.VocabularyEntries.Select(x => x.Id).OrderBy(x => x).ToArrayAsync())
            .Concat(await db.VocabularySenses.Select(x => x.Id).OrderBy(x => x).ToArrayAsync())
            .Concat(await db.VocabularySenseTranslations.Select(x => x.Id).OrderBy(x => x).ToArrayAsync());
        Check.That(reappliedIds.SequenceEqual(seedIds) && await db.VocabularySenseTranslations.CountAsync() == 84, "Down/up preserves stable identities; repeated migrate does not duplicate seed.");

        var assembly = db.GetService<IMigrationsAssembly>();
        var migrationType = assembly.Migrations.Single(x => x.Key.EndsWith("_AddVocabularyCatalog"));
        var migration = assembly.CreateMigration(migrationType.Value, db.Database.ProviderName!);
        var tables = new[] { "vocabulary_entries", "vocabulary_senses", "vocabulary_sense_translations" };
        Check.That(migration.UpOperations.OfType<CreateTableOperation>().Select(x => x.Name).ToHashSet().SetEquals(tables), "Migration creates exactly three tables.");
        Check.That(migration.UpOperations.All(operation => operation switch
        {
            CreateTableOperation op => tables.Contains(op.Name),
            CreateIndexOperation op => tables.Contains(op.Table),
            InsertDataOperation op => tables.Contains(op.Table),
            _ => false
        }), "Migration has no changes or seed writes outside vocabulary.");
        Check.That(migration.UpOperations.OfType<CreateIndexOperation>().Count() == 5, "Only five indexes are added.");
        foreach (var type in new[] { typeof(VocabularyEntry), typeof(VocabularySense), typeof(VocabularySenseTranslation) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never, $"Snowflake ID is caller-supplied for {type.Name}.");
            Check.That(entity.FindProperty("Version")!.IsConcurrencyToken && entity.FindProperty("Version")!.ValueGenerated == ValueGenerated.Never, $"Version concurrency for {type.Name}.");
            Check.That(!entity.GetProperties().Any(x => x.IsShadowProperty()), $"No accidental shadow properties for {type.Name}.");
            Check.That(entity.GetForeignKeys().Single().DeleteBehavior == DeleteBehavior.Cascade, $"Cascade ancestry for {type.Name}.");
            Check.That(!entity.GetIndexes().Any(x => x.Properties.Count == 1), $"No redundant standalone parent FK index for {type.Name}.");
        }
        Check.That(db.Model.FindEntityType(typeof(VocabularySense))!.FindProperty("PartOfSpeech")!.GetTypeMapping().Converter!.ProviderClrType == typeof(string), "PartOfSpeech persists names as strings.");
        var water = entries.Single(x => x.Code == "mizu");
        var waterSense = senses.Single(x => x.VocabularyEntryId == water.Id);
        var waterVi = translations.Single(x => x.VocabularySenseId == waterSense.Id && x.Locale == "vi");
        foreach (var (assignment, state) in new[]
        {
            ("id = -1", PostgresErrorCodes.CheckViolation), ("language_id = 0", PostgresErrorCodes.CheckViolation),
            ("version = 0", PostgresErrorCodes.CheckViolation), ("updated_at = created_at - interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("sort_order = -1", PostgresErrorCodes.CheckViolation), ("code = 'UPPER'", PostgresErrorCodes.CheckViolation),
            ("code = '1bad'", PostgresErrorCodes.CheckViolation), ("code = ''", PostgresErrorCodes.CheckViolation),
            ("code = repeat('a',65)", PostgresErrorCodes.StringDataRightTruncation),
            ("term = E' \\t '", PostgresErrorCodes.CheckViolation), ("term = NULL", PostgresErrorCodes.NotNullViolation),
            ("term = repeat('a',201)", PostgresErrorCodes.StringDataRightTruncation),
            ("reading = E'\\t'", PostgresErrorCodes.CheckViolation), ("reading = repeat('a',201)", PostgresErrorCodes.StringDataRightTruncation),
            ("audio_url = ''", PostgresErrorCodes.CheckViolation), ("audio_url = 'http://media.example/a.mp3'", PostgresErrorCodes.CheckViolation),
            ("audio_url = 'https://user:pass@media.example/a.mp3'", PostgresErrorCodes.CheckViolation),
            ("audio_url = 'https://media.example/a.mp3#fragment'", PostgresErrorCodes.CheckViolation),
            ("audio_url = 'https://media.example/a b.mp3'", PostgresErrorCodes.CheckViolation),
            ("audio_url = 'https://media.example/' || repeat('a',1024)", PostgresErrorCodes.StringDataRightTruncation),
            ("language_id = 999999", PostgresErrorCodes.ForeignKeyViolation)
        }) await Reject($"UPDATE vocabulary_entries SET {assignment} WHERE id = {water.Id}", state, $"Reject invalid entry: {assignment}");
        foreach (var (assignment, state) in new[]
        {
            ("id = -1", PostgresErrorCodes.CheckViolation), ("vocabulary_entry_id = 0", PostgresErrorCodes.CheckViolation),
            ("version = 0", PostgresErrorCodes.CheckViolation), ("updated_at = created_at - interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("sort_order = -1", PostgresErrorCodes.CheckViolation), ("code = 'BAD'", PostgresErrorCodes.CheckViolation),
            ("part_of_speech = 'noun'", PostgresErrorCodes.CheckViolation), ("part_of_speech = 'Unknown'", PostgresErrorCodes.CheckViolation),
            ("part_of_speech = '0'", PostgresErrorCodes.CheckViolation), ("part_of_speech = NULL", PostgresErrorCodes.NotNullViolation),
            ("vocabulary_entry_id = 999999", PostgresErrorCodes.ForeignKeyViolation)
        }) await Reject($"UPDATE vocabulary_senses SET {assignment} WHERE id = {waterSense.Id}", state, $"Reject invalid sense: {assignment}");
        foreach (var (assignment, state) in new[]
        {
            ("id = -1", PostgresErrorCodes.CheckViolation), ("vocabulary_sense_id = 0", PostgresErrorCodes.CheckViolation),
            ("version = 0", PostgresErrorCodes.CheckViolation), ("updated_at = created_at - interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("locale = 'EN'", PostgresErrorCodes.CheckViolation), ("locale = 'en_US'", PostgresErrorCodes.CheckViolation),
            ("locale = 'en--us'", PostgresErrorCodes.CheckViolation), ("locale = 'x'", PostgresErrorCodes.CheckViolation),
            ("locale = 'en-12345678-12345678-12345678-12345678'", PostgresErrorCodes.StringDataRightTruncation),
            ("meaning = E' \\t '", PostgresErrorCodes.CheckViolation), ("meaning = NULL", PostgresErrorCodes.NotNullViolation),
            ("meaning = repeat('a',501)", PostgresErrorCodes.StringDataRightTruncation),
            ("usage_note = E'\\t'", PostgresErrorCodes.CheckViolation), ("usage_note = repeat('a',2001)", PostgresErrorCodes.StringDataRightTruncation),
            ("vocabulary_sense_id = 999999", PostgresErrorCodes.ForeignKeyViolation)
        }) await Reject($"UPDATE vocabulary_sense_translations SET {assignment} WHERE id = {waterVi.Id}", state, $"Reject invalid translation: {assignment}");
        var other = entries.Single(x => x.Code == "ocha");
        await Reject($"UPDATE vocabulary_entries SET code = 'mizu' WHERE id = {other.Id}", PostgresErrorCodes.UniqueViolation, "Entry code unique within language.");
        var meal = senses.Single(x => x.VocabularyEntryId == gohanId && x.Code == "meal");
        await Reject($"UPDATE vocabulary_senses SET code = 'cooked-rice' WHERE id = {meal.Id}", PostgresErrorCodes.UniqueViolation, "Sense code unique within entry.");
        await Reject($"UPDATE vocabulary_sense_translations SET locale = 'en' WHERE id = {waterVi.Id}", PostgresErrorCodes.UniqueViolation, "One translation per sense/locale.");

        db.Languages.Add(Language.Create(9000, "en", "English", true, 0, now));
        var homograph = VocabularyEntry.Create(9001, japanese.Id, "water-variant", water.Term, "すい", null, false, 0, now);
        db.VocabularyEntries.AddRange(homograph, VocabularyEntry.Create(9002, 9000, "mizu", "water", null, "https://media.example/water.mp3", true, 0, now));
        db.VocabularySenses.Add(VocabularySense.Create(9003, homograph.Id, "water", PartOfSpeech.Noun, 0, now));
        db.VocabularySenseTranslations.Add(VocabularySenseTranslation.Create(9004, 9003, "EN-us", "water", null, now));
        foreach (var pos in Enum.GetValues<PartOfSpeech>())
            db.VocabularySenses.Add(VocabularySense.Create(9100 + (int)pos, 9002, "pos-" + pos.ToString().ToLowerInvariant(), pos, 0, now));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        Check.That(await db.VocabularyEntries.CountAsync(x => x.LanguageId == japanese.Id && x.Term == water.Term) == 2, "Homographs are supported through distinct stable codes.");
        Check.That(await db.VocabularyEntries.CountAsync(x => x.Code == "mizu") == 2, "Same code in different languages is allowed.");
        Check.That(!(await db.VocabularyEntries.SingleAsync(x => x.Id == 9001)).IsPublished && (await db.VocabularyEntries.SingleAsync(x => x.Id == 9002)).Reading is null, "False publication and nullable reading survive EF round-trip.");
        Check.That((await db.VocabularySenseTranslations.SingleAsync(x => x.Id == 9004)).Locale == "en-us", "Normalized regional locale round-trips.");
        Check.That((await db.VocabularySenses.Where(x => x.VocabularyEntryId == 9002).Select(x => x.PartOfSpeech).ToArrayAsync()).ToHashSet().SetEquals(Enum.GetValues<PartOfSpeech>()), "All enum names round-trip through PostgreSQL string conversion/CHECK.");

        await using (var left = Context())
        await using (var right = Context())
        {
            var winner = await left.VocabularyEntries.SingleAsync(x => x.Id == water.Id);
            var stale = await right.VocabularyEntries.SingleAsync(x => x.Id == water.Id);
            winner.Update("水", "みず", "https://media.example/water.mp3", true, 1, now);
            stale.Update("changed", null, null, false, 0, now);
            await left.SaveChangesAsync();
            await Check.ThrowsAsync<DbUpdateConcurrencyException>(() => right.SaveChangesAsync(), "Stale entry update cannot overwrite committed content.");
        }
        await using (var left = Context())
        await using (var right = Context())
        {
            var winner = await left.VocabularySenses.SingleAsync(x => x.Id == waterSense.Id);
            var stale = await right.VocabularySenses.SingleAsync(x => x.Id == waterSense.Id);
            winner.Update(PartOfSpeech.Noun, 1, now);
            stale.Update(PartOfSpeech.Other, 2, now);
            await left.SaveChangesAsync();
            await Check.ThrowsAsync<DbUpdateConcurrencyException>(() => right.SaveChangesAsync(), "Stale sense update cannot overwrite committed POS/order.");
        }
        await using (var left = Context())
        await using (var right = Context())
        {
            var winner = await left.VocabularySenseTranslations.SingleAsync(x => x.Id == waterVi.Id);
            var stale = await right.VocabularySenseTranslations.SingleAsync(x => x.Id == waterVi.Id);
            winner.Update("Nước uống.", "Dữ liệu kiểm thử.", now);
            stale.Update("changed", null, now);
            await left.SaveChangesAsync();
            await Check.ThrowsAsync<DbUpdateConcurrencyException>(() => right.SaveChangesAsync(), "Stale translation update cannot overwrite committed text.");
        }
        Check.That((await db.VocabularyEntries.AsNoTracking().SingleAsync(x => x.Id == water.Id)).Version == 2 &&
                   (await db.VocabularySenses.AsNoTracking().SingleAsync(x => x.Id == waterSense.Id)).PartOfSpeech == PartOfSpeech.Noun &&
                   (await db.VocabularySenseTranslations.AsNoTracking().SingleAsync(x => x.Id == waterVi.Id)).Meaning == "Nước uống.", "Winning content survives all stale writes.");
        await using (var failed = Context())
        {
            failed.VocabularyEntries.Add(VocabularyEntry.Create(9500, japanese.Id, "rollback", "test", null, null, false, 0, now));
            failed.VocabularySenses.Add(VocabularySense.Create(9501, 9500, "sense", PartOfSpeech.Noun, 0, now));
            failed.VocabularySenseTranslations.AddRange(VocabularySenseTranslation.Create(9502, 9501, "vi", "one", null, now),
                VocabularySenseTranslation.Create(9503, 9501, "vi", "duplicate", null, now));
            await Check.ThrowsAsync<DbUpdateException>(() => failed.SaveChangesAsync(), "Duplicate child locale rejects entire graph insert.");
        }
        Check.That(!await db.VocabularyEntries.AnyAsync(x => x.Id == 9500) && !await db.VocabularySenses.AnyAsync(x => x.Id == 9501), "Failed child insert rolls back parent and sense.");
        await db.VocabularySenses.Where(x => x.Id == 9003).ExecuteDeleteAsync();
        Check.That(!await db.VocabularySenseTranslations.AnyAsync(x => x.Id == 9004) && await db.VocabularyEntries.AnyAsync(x => x.Id == 9001), "Sense deletion cascades only its translations.");
        await db.VocabularyEntries.Where(x => x.Id == gohanId).ExecuteDeleteAsync();
        Check.That(!await db.VocabularySenses.AnyAsync(x => x.VocabularyEntryId == gohanId) &&
                   !await db.VocabularySenseTranslations.AnyAsync(x => x.VocabularySenseId == meal.Id), "Entry deletion cascades all senses/translations.");
        Check.That(await db.VocabularySenses.CountAsync(x => x.VocabularyEntryId == kikuId) == 2, "Cascade does not delete another entry's senses.");
        await db.Languages.Where(x => x.Id == 9000).ExecuteDeleteAsync();
        Check.That(!await db.VocabularyEntries.AnyAsync(x => x.Id == 9002) && !await db.VocabularySenses.AnyAsync(x => x.VocabularyEntryId == 9002), "Language deletion cascades its vocabulary.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 && await db.WritingSystemItemProgress.CountAsync() == 1,
            "Vocabulary edits do not write personal learning state.");
    }
}
