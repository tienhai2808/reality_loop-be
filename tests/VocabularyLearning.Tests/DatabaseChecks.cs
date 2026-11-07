using Domain.LearningProfiles;
using Domain.Users;
using Domain.VocabularyLearning;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;

namespace VocabularyLearning.Tests;

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
            catch (PostgresException ex) when (ex.SqlState == state) { Check.That(true, "Rejected invalid SQL: " + sql); return; }
            throw new InvalidOperationException("Invalid SQL accepted: " + sql);
        }
        if (await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public'") != 0)
            throw new InvalidOperationException("Test database must be empty; existing data will not be deleted.");
        await using var db = Context();
        var migrator = db.GetService<IMigrator>();
        const string previous = "20261104163042_CompleteKanaHandwritingTemplates";
        await migrator.MigrateAsync(previous);
        var now = DomainChecks.Now;
        db.Users.Add(User.Create(8000, "test@vocabulary-learning.invalid", "Test", "User", null, now));
        var profile = LearningProfile.Create(8001, 8000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Study, null, now);
        db.LearningProfiles.Add(profile);
        await db.SaveChangesAsync();
        await migrator.MigrateAsync();
        Check.That(!db.Database.HasPendingModelChanges(), "Model matches snapshot.");
        Check.That(await db.Users.CountAsync() == 1 && await db.LearningProfiles.CountAsync() == 1 &&
            await db.HandwritingTemplates.CountAsync() == 208 && await db.VocabularySenses.CountAsync() == 202, "Upgrade preserves user/catalog data.");
        var assembly = db.GetService<IMigrationsAssembly>();
        var migration = assembly.CreateMigration(assembly.Migrations.Single(x => x.Key.EndsWith("_AddVocabularyLearningProgress")).Value, db.Database.ProviderName!);
        var tables = new[] { "vocabulary_learning_progress", "vocabulary_reviews" };
        Check.That(migration.UpOperations.All(x => x switch
        {
            CreateTableOperation t => tables.Contains(t.Name), CreateIndexOperation i => tables.Contains(i.Table), _ => false
        }), "Migration touches only two new tables, without seed mutations.");
        Check.That(migration.UpOperations.OfType<CreateIndexOperation>().Count() == 6, "Six necessary due/FK/unique indexes.");
        foreach (var type in new[] { typeof(VocabularyLearningProgress), typeof(VocabularyReview) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never && entity.FindProperty("Version")!.IsConcurrencyToken,
                "Snowflake and concurrency conventions.");
            Check.That(!entity.GetProperties().Any(x => x.IsShadowProperty()), "No shadow FK properties.");
            foreach (var property in entity.GetProperties().Where(x => x.ClrType.IsEnum))
                Check.That(property.GetTypeMapping().Converter!.ProviderClrType == typeof(string), "Enum names persisted as strings.");
        }
        var sense = await db.VocabularySenses.OrderBy(x => x.Id).FirstAsync();
        var entry = await db.VocabularyEntries.SingleAsync(x => x.Id == sense.VocabularyEntryId);
        var language = await db.Languages.SingleAsync(x => x.Id == entry.LanguageId);
        var progress = VocabularyLearningProgress.Create(8002, profile, sense, entry, language, now);
        db.VocabularyLearningProgress.Add(progress);
        await db.SaveChangesAsync();
        Check.That(await db.VocabularyLearningProgress.CountAsync(x => x.LearningProfileId == profile.Id && x.NextReviewAt <= now) == 1, "New card is immediately due.");
        var request = Guid.NewGuid();
        var review = progress.RecordReview(8003, request, VocabularyReviewRating.Good, new(VocabularyLearningStatus.Reviewing, 1440, "simple-v1"), now);
        db.VocabularyReviews.Add(review);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        progress = await db.VocabularyLearningProgress.SingleAsync(x => x.Id == 8002);
        var loaded = await db.VocabularyReviews.SingleAsync(x => x.Id == 8003);
        Check.That(progress.NextReviewAt == now.AddDays(1) && progress.Version == 2 && loaded.PreviousDueAt == now && loaded.NextReviewAt == progress.NextReviewAt && loaded.RequestId == request,
            "Root and immutable history round-trip with exact minute scheduling.");
        foreach (var assignment in new[]
        {
            "id = 0", "learning_profile_id = 0", "vocabulary_sense_id = 0", "version = 0", "review_count = -1",
            "review_count = version", "again_count = -1", "consecutive_recall_count = review_count + 1", "status = 'Unknown'",
            "status = 'New'", "status = '1'", "last_reviewed_at = NULL", "interval_minutes = 0", "scheduler_version = NULL",
            "scheduler_version = ''", "scheduler_version = 'BAD'", "updated_at = created_at - interval '1 minute'",
            "next_review_at = last_reviewed_at", "again_count = review_count", "scheduler_version = 'bad_code'"
        }) await Reject($"UPDATE vocabulary_learning_progress SET {assignment} WHERE id = 8002", PostgresErrorCodes.CheckViolation);
        foreach (var assignment in new[]
        {
            "id = 0", "progress_id = 0", "request_id = '00000000-0000-0000-0000-000000000000'", "review_number = 0",
            "rating = 'Unknown'", "rating = '0'", "rating = 'Again'", "status = 'New'", "previous_status = 'Reviewing'",
            "previous_interval_minutes = 1", "interval_minutes = 0", "next_review_at = reviewed_at",
            "progress_version_before = 0", "progress_version_after = progress_version_before", "progress_version_after = 4",
            "again_count = -1", "consecutive_recall_count = 0", "scheduler_version = ''", "scheduler_version = 'BAD'",
            "version = 2", "updated_at = created_at + interval '1 second'", "reviewed_at = created_at + interval '1 second'"
        }) await Reject($"UPDATE vocabulary_reviews SET {assignment} WHERE id = 8003", PostgresErrorCodes.CheckViolation);
        await Reject("UPDATE vocabulary_learning_progress SET learning_profile_id = 99999 WHERE id = 8002", PostgresErrorCodes.ForeignKeyViolation);
        await Reject("UPDATE vocabulary_reviews SET progress_id = 99999 WHERE id = 8003", PostgresErrorCodes.ForeignKeyViolation);
        await Reject($"DELETE FROM vocabulary_senses WHERE id = {sense.Id}", PostgresErrorCodes.ForeignKeyViolation);
        await Reject("INSERT INTO vocabulary_learning_progress SELECT 8090, learning_profile_id, vocabulary_sense_id, status, next_review_at, last_reviewed_at, interval_minutes, review_count, again_count, consecutive_recall_count, scheduler_version, version, created_at, updated_at FROM vocabulary_learning_progress WHERE id = 8002", PostgresErrorCodes.UniqueViolation);
        // A repeated request would update the root if the caller failed to detect replay. The unique
        // receipt key must reject the history insert and roll back that update in the same SaveChanges.
        var duplicate = progress.RecordReview(8004, request, VocabularyReviewRating.Again, new(VocabularyLearningStatus.Learning, 1, "simple-v1"), now.AddMinutes(1));
        db.VocabularyReviews.Add(duplicate);
        try { await db.SaveChangesAsync(); throw new InvalidOperationException("Replay accepted."); }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        { Check.That(true, "Duplicate request rejected."); }
        db.ChangeTracker.Clear();
        Check.That((await db.VocabularyLearningProgress.SingleAsync(x => x.Id == 8002)).Version == 2 &&
            !await db.VocabularyReviews.AnyAsync(x => x.Id == 8004), "Replay rolls back root counters/schedule and history together.");
        await using (var first = Context())
        await using (var second = Context())
        {
            var a = await first.VocabularyLearningProgress.SingleAsync(x => x.Id == 8002);
            var b = await second.VocabularyLearningProgress.SingleAsync(x => x.Id == 8002);
            first.VocabularyReviews.Add(a.RecordReview(8010, Guid.NewGuid(), VocabularyReviewRating.Again,
                new(VocabularyLearningStatus.Learning, 2, "simple-v1"), now.AddMinutes(2)));
            second.VocabularyReviews.Add(b.RecordReview(8011, Guid.NewGuid(), VocabularyReviewRating.Easy,
                new(VocabularyLearningStatus.Reviewing, 60, "simple-v1"), now.AddMinutes(2)));
            await first.SaveChangesAsync();
            try { await second.SaveChangesAsync(); throw new InvalidOperationException("Stale review accepted."); }
            catch (DbUpdateConcurrencyException) { Check.That(true, "Stale root CAS rejected."); }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            { Check.That(true, "Stale history sequence rejected."); }
        }
        db.ChangeTracker.Clear();
        Check.That((await db.VocabularyLearningProgress.SingleAsync(x => x.Id == 8002)).Version == 3 &&
            await db.VocabularyReviews.CountAsync() == 2 && !await db.VocabularyReviews.AnyAsync(x => x.Id == 8011), "Only winning review commits.");
        // Explicit profile deletion removes only personal graph; catalog remains shared.
        await db.Database.ExecuteSqlRawAsync("DELETE FROM learning_profiles WHERE id = 8001");
        Check.That(await db.VocabularyLearningProgress.CountAsync() == 0 && await db.VocabularyReviews.CountAsync() == 0 &&
            await db.VocabularySenses.CountAsync() == 202, "Profile cascade preserves vocabulary catalog.");
        await migrator.MigrateAsync(previous);
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name IN ('vocabulary_learning_progress','vocabulary_reviews')") == 0 &&
            await db.HandwritingTemplates.CountAsync() == 92 && await db.Users.CountAsync() == 1, "Down affects only new module.");
        await migrator.MigrateAsync();
        await migrator.MigrateAsync();
        Check.That(await db.VocabularyLearningProgress.CountAsync() == 0 && await db.VocabularyReviews.CountAsync() == 0 &&
            !db.Database.HasPendingModelChanges(), "Up/down/up is deterministic; no private learning state is seeded.");
    }
}
