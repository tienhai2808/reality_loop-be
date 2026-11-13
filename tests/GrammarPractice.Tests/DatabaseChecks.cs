using Domain.GrammarPractice;
using Domain.LearningProfiles;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;

namespace GrammarPractice.Tests;

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
        async Task Reject(string sql, string state)
        {
            await using var command = new NpgsqlCommand(sql, connection);
            try { await command.ExecuteNonQueryAsync(); }
            catch (PostgresException exception) when (exception.SqlState == state)
            {
                Check.That(true, "Database rejects invalid persisted state.");
                return;
            }
            throw new InvalidOperationException("Database accepted invalid SQL: " + sql);
        }
        if (await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public'") != 0)
            throw new InvalidOperationException("Test database must be empty. Existing data will not be deleted.");
        await using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previous = "20261107132245_AddGermanLearningContent";
        await migrator.MigrateAsync(previous);
        var now = DomainChecks.Now;
        db.Users.Add(User.Create(6000, "grammar-practice@test.invalid", "Grammar", "Practice", null, now));
        var profile = LearningProfile.Create(6001, 6000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Study, null, now);
        db.LearningProfiles.Add(profile);
        await db.SaveChangesAsync();
        await migrator.MigrateAsync();
        Check.That(!db.Database.HasPendingModelChanges(), "Deterministic model matches migration snapshot.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 &&
            await db.GrammarPoints.CountAsync() == 100 && await db.HandwritingTemplates.CountAsync() == 268,
            "Upgrade preserves existing users, profiles and language catalogs.");
        var exercises = await db.GrammarExercises.AsNoTracking().OrderBy(row => row.Id).ToArrayAsync();
        var translations = await db.GrammarExerciseTranslations.AsNoTracking().ToArrayAsync();
        Check.That(exercises.Length == 200 && translations.Length == 400, "Seed contains 200 exercises and 400 translations.");
        Check.That(exercises.GroupBy(row => row.GrammarPointId).Count() == 20 && exercises.GroupBy(row => row.GrammarPointId).All(group => group.Count() == 10),
            "Twenty introductory points each support a ten-question session.");
        Check.That(exercises.All(exercise => translations.Where(t => t.GrammarExerciseId == exercise.Id)
            .Select(t => t.Locale).Order().SequenceEqual(new[] { "en", "vi" })), "Every exercise has exactly one aligned VI/EN explanation.");
        Check.That(await db.GrammarPracticeSessions.CountAsync() == 0 && await db.GrammarPracticeSessionItems.CountAsync() == 0,
            "No private learning data is seeded.");
        var ids = db.GetService<IDesignTimeModel>().Model.GetEntityTypes().SelectMany(type => type.GetSeedData())
            .Select(row => (long)row["Id"]!).ToArray();
        Check.That(ids.Distinct().Count() == ids.Length && ids.All(id => ((id >> 12) & 1023) == 1023), "All seed IDs retain unique reserved Snowflake allocation.");
        var assembly = db.GetService<IMigrationsAssembly>();
        var migration = assembly.CreateMigration(assembly.Migrations.Single(row => row.Key.EndsWith("_AddGrammarPractice")).Value, db.Database.ProviderName!);
        var tables = new[] { "grammar_exercises", "grammar_exercise_translations", "grammar_practice_sessions", "grammar_practice_session_items" };
        Check.That(migration.UpOperations.All(operation => operation switch
        {
            CreateTableOperation table => tables.Contains(table.Name),
            CreateIndexOperation index => tables.Contains(index.Table),
            InsertDataOperation seed => seed.Table is "grammar_exercises" or "grammar_exercise_translations",
            _ => false
        }), "Migration affects only the new module schema and seed.");
        foreach (var type in new[] { typeof(GrammarExercise), typeof(GrammarExerciseTranslation), typeof(GrammarPracticeSession), typeof(GrammarPracticeSessionItem) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(!entity.GetProperties().Any(property => property.IsShadowProperty()), "Relationships create no hidden columns.");
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never && entity.FindProperty("Version")!.IsConcurrencyToken,
                "IDs and optimistic concurrency follow project conventions.");
        }
        var language = await db.Languages.SingleAsync(row => row.Code == "ja");
        var point = await db.GrammarPoints.SingleAsync(row => row.LanguageId == language.Id && row.Code == "noun-copula");
        var selected = exercises.Where(exercise => exercise.GrammarPointId == point.Id).Take(5).ToArray();
        var questions = selected.Select((exercise, index) => GrammarPracticeQuestion.Create(6200 + index, exercise,
            translations.Single(t => t.GrammarExerciseId == exercise.Id && t.Locale == "vi"), point, language, [3, 1, 0, 2])).ToArray();
        var session = GrammarPracticeSession.Create(6100, profile, language, questions, now, point);
        db.GrammarPracticeSessions.Add(session);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var loaded = await db.GrammarPracticeSessions.Include(root => root.Items).SingleAsync(root => root.Id == 6100);
        Check.That(loaded.Items.Count == 5 && loaded.Items.OrderBy(item => item.Position).Select(item => item.Id).SequenceEqual(questions.Select(q => q.Id)),
            "Aggregate and ordered navigation round-trip through EF.");
        var first = loaded.Items.OrderBy(item => item.Position).First();
        var promptSnapshot = first.Prompt;
        var optionsSnapshot = first.Options;
        var explanationSnapshot = first.Explanation;
        var source = await db.GrammarExercises.SingleAsync(exercise => exercise.Id == first.GrammarExerciseId);
        source.Update("Đã đổi {blank}.", ["changed", "other"], 1, false, 0, now.AddMinutes(1));
        var sourceTranslation = await db.GrammarExerciseTranslations.SingleAsync(t => t.GrammarExerciseId == source.Id && t.Locale == "vi");
        sourceTranslation.Update("Đã đổi", "Giải thích đã đổi.", now.AddMinutes(1));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        first = await db.GrammarPracticeSessionItems.SingleAsync(item => item.Id == first.Id);
        Check.That(first.Prompt == promptSnapshot && first.Options.SequenceEqual(optionsSnapshot) && first.Explanation == explanationSnapshot &&
            first.ExerciseVersion == 1 && first.TranslationVersion == 1, "Editing/unpublishing source content cannot rewrite practice history.");
        await Reject($"UPDATE grammar_exercises SET prompt = 'no blank' WHERE id = {source.Id}", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_exercises SET options = ARRAY['a','a'] WHERE id = {source.Id}", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_exercises SET options = ARRAY['','b'] WHERE id = {source.Id}", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_exercises SET options = ARRAY[['a','b'],['c','d']] WHERE id = {source.Id}", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_exercises SET correct_option_index = 4 WHERE id = {source.Id}", PostgresErrorCodes.CheckViolation);
        await Reject("UPDATE grammar_practice_sessions SET status = 'Unknown' WHERE id = 6100", PostgresErrorCodes.CheckViolation);
        await Reject("UPDATE grammar_practice_sessions SET question_count = 4 WHERE id = 6100", PostgresErrorCodes.CheckViolation);
        await Reject("UPDATE grammar_practice_sessions SET status = 'Completed' WHERE id = 6100", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_practice_session_items SET grammar_point_id = {point.Id + 1} WHERE id = {first.Id}", PostgresErrorCodes.ForeignKeyViolation);
        await Reject($"DELETE FROM grammar_exercises WHERE id = {source.Id}", PostgresErrorCodes.ForeignKeyViolation);
        await Reject($"UPDATE grammar_practice_session_items SET selected_option_index = 0 WHERE id = {first.Id}", PostgresErrorCodes.CheckViolation);
        await Reject($"UPDATE grammar_practice_session_items SET translation_locale = 'vi_vi' WHERE id = {first.Id}", PostgresErrorCodes.CheckViolation);
        await using (var partial = Context())
        {
            var root = await partial.GrammarPracticeSessions.SingleAsync(row => row.Id == 6100);
            Check.Throws<InvalidOperationException>(() => root.Answer(first.Id, first.CorrectOptionIndex, now.AddMinutes(2)),
                "Partial navigation cannot complete or modify a session.");
        }
        await using (var winner = Context())
        await using (var stale = Context())
        {
            var current = await winner.GrammarPracticeSessions.Include(root => root.Items).SingleAsync(root => root.Id == 6100);
            var outdated = await stale.GrammarPracticeSessions.Include(root => root.Items).SingleAsync(root => root.Id == 6100);
            var currentItem = current.Items.OrderBy(item => item.Position).First();
            current.Answer(currentItem.Id, currentItem.CorrectOptionIndex, now.AddMinutes(2));
            await winner.SaveChangesAsync();
            var other = outdated.Items.OrderBy(item => item.Position).Skip(1).First();
            outdated.Answer(other.Id, other.CorrectOptionIndex, now.AddMinutes(2));
            try
            {
                await stale.SaveChangesAsync();
                throw new InvalidOperationException("A stale aggregate update must fail.");
            }
            catch (DbUpdateConcurrencyException) { Check.That(true, "Stale aggregate CAS rejects the losing writer."); }
        }
        db.ChangeTracker.Clear();
        loaded = await db.GrammarPracticeSessions.Include(root => root.Items).SingleAsync(root => root.Id == 6100);
        Check.That(loaded.Version == 2 && loaded.Items.Count(item => item.AnsweredAt is not null) == 1 &&
            loaded.Items.Count(item => item.Version == 2) == 1, "Failed concurrent SaveChanges rolls back every losing child mutation.");
        foreach (var item in loaded.Items.Where(item => item.AnsweredAt is null)) loaded.Answer(item.Id, item.CorrectOptionIndex, now.AddMinutes(3));
        await db.SaveChangesAsync();
        Check.That(loaded.Status == GrammarPracticeSessionStatus.Completed && loaded.Version == 6, "A persisted session completes consistently.");
        await db.LearningProfiles.Where(row => row.Id == 6001).ExecuteDeleteAsync();
        Check.That(!await db.GrammarPracticeSessions.AnyAsync() && !await db.GrammarPracticeSessionItems.AnyAsync() && await db.GrammarExercises.CountAsync() == 200,
            "Profile deletion removes only its private graph.");
        db.ChangeTracker.Clear();
        await migrator.MigrateAsync(previous);
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name IN ('grammar_exercises','grammar_exercise_translations','grammar_practice_sessions','grammar_practice_session_items')") == 0 &&
            await db.GrammarPoints.CountAsync() == 100 && await db.Users.CountAsync() == 1, "Module rollback preserves earlier schema and users.");
        await migrator.MigrateAsync();
        await migrator.MigrateAsync();
        Check.That(await db.GrammarExercises.CountAsync() == 200 && await db.GrammarExerciseTranslations.CountAsync() == 400 &&
            !db.Database.HasPendingModelChanges(), "Reapply and repeated migration seed exactly once.");
    }
}
