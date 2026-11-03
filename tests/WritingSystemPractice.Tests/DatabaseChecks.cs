using Domain.LearningProfiles;
using Domain.Users;
using Domain.WritingSystemPractice;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace WritingSystemPractice.Tests;

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
        async Task Reject(string sql, string sqlState, string message)
        {
            await using var command = new NpgsqlCommand(sql, connection);
            try { await command.ExecuteNonQueryAsync(); }
            catch (PostgresException ex) when (ex.SqlState == sqlState) { Check.That(true, message); return; }
            throw new InvalidOperationException(message);
        }
        if (await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'public'") != 0)
            throw new InvalidOperationException("The practice test database must be empty. No existing data will be deleted.");
        await using var db = Context();
        await db.Database.MigrateAsync();
        Check.That(!db.Database.HasPendingModelChanges(), "Runtime model must match migration snapshot.");
        Check.That(await db.WritingSystemPracticeSessions.CountAsync() == 0 && await db.WritingSystemPracticeSessionItems.CountAsync() == 0, "No personal practice seed.");
        Check.That(await db.WritingSystemItems.CountAsync() == 92, "Existing catalog seed preserved.");
        var migrator = db.GetService<IMigrator>();
        await migrator.MigrateAsync("20261027114751_AddWritingSystemItemProgress");
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name LIKE 'writing_system_practice_%'") == 0, "Migration down removes both tables.");
        Check.That(await db.WritingSystemItems.CountAsync() == 92, "Migration down preserves existing catalog.");
        await migrator.MigrateAsync();
        Check.That(await Scalar("SELECT count(*) FROM information_schema.tables WHERE table_name LIKE 'writing_system_practice_%'") == 2, "Migration reapply recreates both tables.");

        foreach (var type in new[] { typeof(WritingSystemPracticeSession), typeof(WritingSystemPracticeSessionItem) })
        {
            var entity = db.Model.FindEntityType(type)!;
            Check.That(entity.FindProperty("Id")!.ValueGenerated == ValueGenerated.Never, $"Snowflake ID is caller-supplied for {type.Name}.");
            Check.That(entity.FindProperty("Version")!.IsConcurrencyToken, $"Version concurrency for {type.Name}.");
            Check.That(!entity.GetProperties().Any(x => x.IsShadowProperty()), $"No unintended shadow properties for {type.Name}.");
        }
        var sessionMetadata = db.Model.FindEntityType(typeof(WritingSystemPracticeSession))!;
        var itemMetadata = db.Model.FindEntityType(typeof(WritingSystemPracticeSessionItem))!;
        Check.That(sessionMetadata.FindProperty("Status")!.GetTypeMapping().Converter!.ProviderClrType == typeof(string), "Enum names persisted as strings.");
        Check.That(itemMetadata.FindProperty("Options")!.GetColumnType() == "character varying(200)[]", "Native bounded PostgreSQL array mapping.");
        Check.That(!sessionMetadata.GetIndexes().Any(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { "LearningProfileId" })), "No redundant standalone profile index.");
        Check.That(!itemMetadata.GetIndexes().Any(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { "SessionId" })), "Unique indexes already cover session prefix.");

        var now = new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
        db.Users.AddRange(User.Create(1000, "a@practice.invalid", "A", "Test", null, now),
            User.Create(2000, "b@practice.invalid", "B", "Test", null, now));
        var profile = LearningProfile.Create(1001, 1000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        var otherProfile = LearningProfile.Create(2001, 2000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        db.LearningProfiles.AddRange(profile, otherProfile);
        await db.SaveChangesAsync();
        var language = await db.Languages.SingleAsync(x => x.Code == "ja");
        var system = await db.WritingSystems.SingleAsync(x => x.Code == "hiragana");
        var catalog = await db.WritingSystemItems.Where(x => x.WritingSystemId == system.Id).OrderBy(x => x.SortOrder).Take(2).ToArrayAsync();
        var readings = await db.WritingSystemItemReadings.Where(x => x.WritingSystemItemId == catalog[0].Id || x.WritingSystemItemId == catalog[1].Id).ToArrayAsync();
        var nextId = 10000L;
        WritingSystemPracticeSession Session(LearningProfile? owner = null)
        {
            var questions = catalog.Select(item =>
            {
                var itemReadings = readings.Where(x => x.WritingSystemItemId == item.Id).ToArray();
                var correct = itemReadings.Single();
                var distractor = readings.Single(x => x.WritingSystemItemId != item.Id).Transcription;
                return WritingSystemPracticeQuestion.Create(++nextId, item, correct, itemReadings, [correct.Transcription, distractor]);
            }).ToArray();
            return WritingSystemPracticeSession.Create(++nextId, owner ?? profile, system, language, questions, now);
        }
        var session = Session();
        var other = Session(otherProfile);
        db.WritingSystemPracticeSessions.AddRange(session, other);
        await db.SaveChangesAsync();
        var sessionId = session.Id;
        var firstId = session.Items.OrderBy(x => x.Position).First().Id;
        var secondId = session.Items.OrderBy(x => x.Position).Last().Id;
        db.ChangeTracker.Clear();
        session = await db.WritingSystemPracticeSessions.Include(x => x.Items).SingleAsync(x => x.Id == sessionId);
        Check.That(session.Items.Count == 2 && session.Items.First().Options.Length == 2 && session.Items.All(x => x.IsCorrect is null), "EF round-trip of backing fields and unanswered zero-index questions.");
        Check.That(session.Items.Single(x => x.Id == firstId).CorrectOptionIndex == 0, "Correct answer index zero round-trips.");
        Check.That(await db.WritingSystemPracticeSessions.Where(x => x.LearningProfileId == profile.Id).CountAsync() == 1, "Separate profile ownership via FK.");
        var detachedOptions = session.Items.First().Options;
        detachedOptions[0] = "tampered";
        db.ChangeTracker.DetectChanges();
        Check.That(db.ChangeTracker.Entries().All(x => x.State == EntityState.Unchanged), "Defensive option getter does not mutate EF backing array.");

        foreach (var (assignment, state) in new[]
        {
            ("id = -1", PostgresErrorCodes.CheckViolation),
            ("learning_profile_id = 0", PostgresErrorCodes.CheckViolation),
            ("version = 0", PostgresErrorCodes.CheckViolation),
            ("question_count = 0", PostgresErrorCodes.CheckViolation),
            ("question_count = 21", PostgresErrorCodes.CheckViolation),
            ("status = 'Unknown'", PostgresErrorCodes.CheckViolation),
            ("status = 'Completed'", PostgresErrorCodes.CheckViolation),
            ("status = 'Abandoned'", PostgresErrorCodes.CheckViolation),
            ("ended_at = updated_at", PostgresErrorCodes.CheckViolation),
            ("updated_at = created_at - interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("learning_profile_id = 999999", PostgresErrorCodes.ForeignKeyViolation),
            ("writing_system_id = 999999", PostgresErrorCodes.ForeignKeyViolation),
            ("status = NULL", PostgresErrorCodes.NotNullViolation)
        }) await Reject($"UPDATE writing_system_practice_sessions SET {assignment} WHERE id = {sessionId}", state, $"Reject invalid session: {assignment}");
        foreach (var (assignment, state) in new[]
        {
            ("id = -1", PostgresErrorCodes.CheckViolation),
            ("position = -1", PostgresErrorCodes.CheckViolation),
            ("position = 20", PostgresErrorCodes.CheckViolation),
            ("symbol = ' '", PostgresErrorCodes.CheckViolation),
            ("transcription_system = 'Invalid'", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY[]::varchar(200)[]", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a']", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a','b','c','d','e']", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a',NULL]", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a','a']", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a','b','a']", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a','b','c','a']", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY['a',E'\\t']", PostgresErrorCodes.CheckViolation),
            ("options = '[0:1]={a,b}'::varchar(200)[]", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY[['a','b'],['c','d']]", PostgresErrorCodes.CheckViolation),
            ("options = ARRAY[repeat('a',201),'b']", PostgresErrorCodes.StringDataRightTruncation),
            ("options = NULL", PostgresErrorCodes.NotNullViolation),
            ("correct_option_index = -1", PostgresErrorCodes.CheckViolation),
            ("correct_option_index = 2", PostgresErrorCodes.CheckViolation),
            ("selected_option_index = 0", PostgresErrorCodes.CheckViolation),
            ("answered_at = created_at", PostgresErrorCodes.CheckViolation),
            ("version = 0", PostgresErrorCodes.CheckViolation),
            ("version = 2", PostgresErrorCodes.CheckViolation),
            ("updated_at = created_at + interval '1 second'", PostgresErrorCodes.CheckViolation),
            ("selected_option_index = 2, answered_at = created_at, version = 2", PostgresErrorCodes.CheckViolation),
            ("selected_option_index = 0, answered_at = created_at - interval '1 second', version = 2", PostgresErrorCodes.CheckViolation),
            ("session_id = 999999", PostgresErrorCodes.ForeignKeyViolation),
            ("writing_system_item_id = 999999", PostgresErrorCodes.ForeignKeyViolation)
        }) await Reject($"UPDATE writing_system_practice_session_items SET {assignment} WHERE id = {firstId}", state, $"Reject invalid item: {assignment}");
        await Reject($"UPDATE writing_system_practice_session_items SET position = 0 WHERE id = {secondId}", PostgresErrorCodes.UniqueViolation, "No duplicate position in a session.");
        await Reject($"UPDATE writing_system_practice_session_items SET writing_system_item_id = {catalog[0].Id} WHERE id = {secondId}", PostgresErrorCodes.UniqueViolation, "No repeated target in a session.");
        await Reject($"DELETE FROM writing_system_items WHERE id = {catalog[0].Id}", PostgresErrorCodes.ForeignKeyViolation, "Source item deletion cannot erase practice history.");
        await Reject($"DELETE FROM writing_systems WHERE id = {system.Id}", PostgresErrorCodes.ForeignKeyViolation, "Source system deletion cannot erase practice history.");

        await using (var partial = Context())
        {
            var unloaded = await partial.WritingSystemPracticeSessions.SingleAsync(x => x.Id == sessionId);
            Check.Throws<InvalidOperationException>(() => unloaded.Answer(firstId, 0, now.AddSeconds(1)), "EF unloaded navigation cannot accept answers.");
        }
        // Two writers answer different questions. Session Version must serialize them as one aggregate.
        await using (var left = Context())
        await using (var right = Context())
        {
            var winner = await left.WritingSystemPracticeSessions.Include(x => x.Items).SingleAsync(x => x.Id == sessionId);
            var stale = await right.WritingSystemPracticeSessions.Include(x => x.Items).SingleAsync(x => x.Id == sessionId);
            winner.Answer(firstId, 0, now.AddSeconds(1));
            stale.Answer(secondId, 1, now.AddSeconds(1));
            await left.SaveChangesAsync();
            await Check.ThrowsAsync<DbUpdateConcurrencyException>(() => right.SaveChangesAsync(), "Concurrent answers must conflict.");
        }
        db.ChangeTracker.Clear();
        session = await db.WritingSystemPracticeSessions.Include(x => x.Items).SingleAsync(x => x.Id == sessionId);
        Check.That(session.Version == 2 && session.Items.Single(x => x.Id == firstId).IsCorrect == true, "Winning answer persisted.");
        Check.That(session.Items.Single(x => x.Id == secondId).SelectedOptionIndex is null, "Losing SaveChanges transaction cannot partially persist its answer.");
        session.Answer(firstId, 0, now.AddSeconds(2));
        Check.That(await db.SaveChangesAsync() == 0, "Repeated answer causes no SQL updates.");
        session.Answer(secondId, 1, now.AddSeconds(3));
        await db.SaveChangesAsync();
        Check.That(session.Status == WritingSystemPracticeSessionStatus.Completed && session.Items.Count(x => x.IsCorrect == true) == 1, "Completed session score round-trips.");
        Check.That(await db.LearningProfiles.Where(x => x.Id == profile.Id).Select(x => x.Version).SingleAsync() == 1 && !await db.WritingSystemItemProgress.AnyAsync(), "Practice does not write learning progress/profile state.");

        // Answered-item Version also protects direct EF writes against stale snapshots.
        await using (var staleItems = Context())
        {
            var stale = await staleItems.WritingSystemPracticeSessionItems.SingleAsync(x => x.Id == firstId);
            staleItems.Entry(stale).Property(x => x.Version).OriginalValue = 1;
            staleItems.Entry(stale).Property(x => x.UpdatedAt).IsModified = true;
            await Check.ThrowsAsync<DbUpdateConcurrencyException>(() => staleItems.SaveChangesAsync(), "Stale individual item update must conflict.");
        }

        var snapshotItem = session.Items.Single(x => x.Id == firstId);
        var originalSymbol = snapshotItem.Symbol;
        var originalAnswer = snapshotItem.Options[0];
        var sourceItem = await db.WritingSystemItems.SingleAsync(x => x.Id == catalog[0].Id);
        var sourceReading = await db.WritingSystemItemReadings.SingleAsync(x => x.WritingSystemItemId == sourceItem.Id);
        sourceItem.Update("改", 0, null, null, now.AddSeconds(4));
        sourceReading.Update("changed", "hepburn", null, 0, now.AddSeconds(4));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        snapshotItem = await db.WritingSystemPracticeSessionItems.SingleAsync(x => x.Id == firstId);
        Check.That(snapshotItem.Symbol == originalSymbol && snapshotItem.Options[0] == originalAnswer, "Persisted question is independent of catalog edits.");

        await db.WritingSystemPracticeSessions.Where(x => x.Id == sessionId).ExecuteDeleteAsync();
        Check.That(!await db.WritingSystemPracticeSessionItems.AnyAsync(x => x.SessionId == sessionId), "Deleting session cascades its questions.");
        Check.That(await db.WritingSystemPracticeSessionItems.CountAsync(x => x.SessionId == other.Id) == 2, "Session cascade is scoped to owner/session.");
        var profileCascade = Session();
        db.WritingSystemPracticeSessions.Add(profileCascade);
        await db.SaveChangesAsync();
        await db.LearningProfiles.Where(x => x.Id == profile.Id).ExecuteDeleteAsync();
        Check.That(!await db.WritingSystemPracticeSessions.AnyAsync(x => x.Id == profileCascade.Id) &&
                   !await db.WritingSystemPracticeSessionItems.AnyAsync(x => x.SessionId == profileCascade.Id), "Profile deletion cascades sessions and questions.");
        await db.Users.Where(x => x.Id == 2000).ExecuteDeleteAsync();
        Check.That(!await db.WritingSystemPracticeSessions.AnyAsync() && !await db.WritingSystemPracticeSessionItems.AnyAsync(), "User deletion cascades owned practice through profile.");
        Check.That(await db.WritingSystemItems.CountAsync() == 92, "Practice cleanup does not delete catalog data.");
    }
}
