using System.Data;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Domain.GrammarPractice;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class GrammarPracticeRepository(ApplicationDbContext db) : IGrammarPracticeRepository
{
    private IQueryable<GrammarPracticeSession> OwnedSessions(long userId, long profileId) =>
        db.GrammarPracticeSessions.AsNoTracking().Where(session => session.LearningProfileId == profileId &&
            db.LearningProfiles.Any(profile => profile.Id == session.LearningProfileId && profile.UserId == userId));

    public async Task<GrammarPracticeOptionsDto?> GetOptionsAsync(long userId, long profileId, CancellationToken ct)
    {
        // A left join retains the owned profile when its language has no eligible questions.
        var rows = await (
            from profile in db.LearningProfiles.AsNoTracking()
            join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
            from point in db.GrammarPoints.AsNoTracking().Where(point => point.LanguageId == language.Id && point.IsPublished &&
                db.GrammarExercises.Any(exercise => exercise.GrammarPointId == point.Id && exercise.IsPublished &&
                    db.GrammarExerciseTranslations.Any(translation => translation.GrammarExerciseId == exercise.Id &&
                        (translation.Locale == profile.ExplanationLanguage || translation.Locale == "en")))).DefaultIfEmpty()
            from name in db.GrammarPointTranslations.AsNoTracking()
                .Where(translation => translation.GrammarPointId == (long?)point.Id &&
                    (translation.Locale == profile.ExplanationLanguage || translation.Locale == "en"))
                .OrderBy(translation => translation.Locale == profile.ExplanationLanguage ? 0 : 1).Take(1).DefaultIfEmpty()
            where profile.Id == profileId && profile.UserId == userId && language.IsEnabled
            orderby point.SortOrder, point.Id
            select new
            {
                LanguageId = language.Id, language.Code, profile.ExplanationLanguage,
                PointId = (long?)point.Id, PointCode = point.Code, point.NativeName, SortOrder = (int?)point.SortOrder,
                Name = name == null ? point.NativeName : name.Name, TranslationLocale = name == null ? null : name.Locale,
                Count = db.GrammarExercises.Count(exercise => exercise.GrammarPointId == point.Id && exercise.IsPublished &&
                    db.GrammarExerciseTranslations.Any(translation => translation.GrammarExerciseId == exercise.Id &&
                        (translation.Locale == profile.ExplanationLanguage || translation.Locale == "en")))
            }).ToListAsync(ct);
        if (rows.Count == 0) return null;
        var points = rows.Where(row => row.PointId.HasValue).Select(row => new GrammarPracticePointOptionDto(
            row.PointId!.Value.ToString(), row.PointCode, row.NativeName, row.Name, row.TranslationLocale,
            row.TranslationLocale != row.ExplanationLanguage, row.Count, row.SortOrder!.Value)).ToArray();
        return new GrammarPracticeOptionsDto(profileId.ToString(), rows[0].LanguageId.ToString(), rows[0].Code,
            rows[0].ExplanationLanguage, points);
    }

    public Task<GrammarPracticeContext?> GetContextAsync(long userId, long profileId, string? pointCode, CancellationToken ct) =>
        (from profile in db.LearningProfiles.AsNoTracking()
         join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
         from point in db.GrammarPoints.AsNoTracking()
             .Where(point => point.LanguageId == language.Id && point.Code == pointCode && point.IsPublished).DefaultIfEmpty()
         where profile.Id == profileId && profile.UserId == userId && language.IsEnabled && (pointCode == null || point != null)
         select new GrammarPracticeContext(profile, language, point)).SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<GrammarPracticeCandidate>> SampleAsync(GrammarPracticeContext context, int count, CancellationToken ct)
    {
        var pointId = context.SelectedPoint?.Id;
        var locale = context.Profile.ExplanationLanguage;
        // PostgreSQL selects at most ten rows. A lateral lookup chooses one translation per exercise.
        return await (
            from exercise in db.GrammarExercises.AsNoTracking()
            join point in db.GrammarPoints.AsNoTracking() on exercise.GrammarPointId equals point.Id
            from translation in db.GrammarExerciseTranslations.AsNoTracking()
                .Where(translation => translation.GrammarExerciseId == exercise.Id && (translation.Locale == locale || translation.Locale == "en"))
                .OrderBy(translation => translation.Locale == locale ? 0 : 1).Take(1)
            where point.LanguageId == context.Language.Id && point.IsPublished && exercise.IsPublished &&
                (pointId == null || point.Id == pointId)
            orderby EF.Functions.Random()
            select new GrammarPracticeCandidate(exercise, translation, point)).Take(count).ToListAsync(ct);
    }

    public async Task CreateAsync(long userId, GrammarPracticeSession session, CancellationToken ct)
    {
        if (session.Version != 1 || session.Status != GrammarPracticeSessionStatus.InProgress ||
            session.Items.Count != session.QuestionCount || session.Items.Any(item => item.SessionId != session.Id || item.AnsweredAt.HasValue))
            throw new InvalidOperationException("Only a new complete grammar practice aggregate can be inserted.");
        var items = session.Items.OrderBy(item => item.GrammarPointId).ThenBy(item => item.GrammarExerciseId).ToArray();
        var exerciseIds = items.Select(item => item.GrammarExerciseId).ToArray();
        var exerciseVersions = items.Select(item => item.ExerciseVersion).ToArray();
        var pointIds = items.Select(item => item.GrammarPointId).ToArray();
        var translationVersions = items.Select(item => item.TranslationVersion).ToArray();
        var locales = items.Select(item => item.TranslationLocale).ToArray();
        var selectedPointId = session.GrammarPointId ?? 0;
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            // Validate source revisions and hold shared row locks through the item batch insert.
            // The ownership/locale/language check runs in the same statement as root insertion.
            var inserted = await db.Database.ExecuteSqlInterpolatedAsync($"""
                WITH sources AS MATERIALIZED (
                    SELECT exercise.id
                    FROM unnest({exerciseIds}::bigint[], {exerciseVersions}::bigint[], {pointIds}::bigint[],
                        {translationVersions}::bigint[], {locales}::text[])
                        AS expected(exercise_id, exercise_version, point_id, translation_version, locale)
                    JOIN grammar_exercises AS exercise ON exercise.id = expected.exercise_id
                        AND exercise.grammar_point_id = expected.point_id AND exercise.version = expected.exercise_version
                        AND exercise.is_published
                    JOIN grammar_points AS point ON point.id = exercise.grammar_point_id
                        AND point.language_id = {session.LanguageId} AND point.is_published
                    JOIN grammar_exercise_translations AS translation ON translation.grammar_exercise_id = exercise.id
                        AND translation.locale = expected.locale AND translation.version = expected.translation_version
                        AND (translation.locale = {session.ExplanationLanguage} OR translation.locale = 'en')
                    WHERE ({selectedPointId} = 0 OR point.id = {selectedPointId})
                    ORDER BY point.id, exercise.id
                    FOR SHARE OF exercise, point, translation
                )
                INSERT INTO grammar_practice_sessions
                    (id, learning_profile_id, language_id, grammar_point_id, explanation_language,
                        question_count, status, version, created_at, updated_at)
                SELECT {session.Id}, profile.id, language.id, NULLIF({selectedPointId}, 0), {session.ExplanationLanguage},
                    {session.QuestionCount}, {session.Status.ToString()}, {session.Version}, {session.CreatedAt}, {session.UpdatedAt}
                FROM learning_profiles AS profile
                JOIN languages AS language ON language.code = profile.target_language AND language.is_enabled
                WHERE profile.id = {session.LearningProfileId} AND profile.user_id = {userId}
                    AND profile.explanation_language = {session.ExplanationLanguage} AND language.id = {session.LanguageId}
                    AND (SELECT count(*) FROM sources) = {session.QuestionCount}
                FOR SHARE OF profile, language
                """, ct);
            if (inserted != 1) throw new ConflictException("The learning profile or grammar content changed. Reload practice options before retrying.");
            db.GrammarPracticeSessionItems.AddRange(session.Items);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            throw new ConflictException("The learning profile or grammar content is no longer available.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("The grammar practice identifiers already exist.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException("The grammar practice session identifier already exists.");
        }
    }

    public Task<GrammarPracticeSession?> FindOwnedAsync(long userId, long profileId, long sessionId, CancellationToken ct) =>
        OwnedSessions(userId, profileId).Where(session => session.Id == sessionId).Include(session => session.Items)
            .AsSingleQuery().SingleOrDefaultAsync(ct);

    public async Task<GrammarPracticeSessionPage> GetPageAsync(long userId, long profileId, string? pointCode,
        GrammarPracticeSessionStatus? status, int pageSize, long? before, CancellationToken ct)
    {
        var query = OwnedSessions(userId, profileId);
        if (pointCode is not null)
            query = query.Where(session => session.GrammarPointId != null && db.GrammarPoints
                .Any(point => point.Id == session.GrammarPointId && point.Code == pointCode));
        if (status is not null) query = query.Where(session => session.Status == status);
        if (before is { } cursor) query = query.Where(session => session.Id < cursor);
        var items = await query.OrderByDescending(session => session.Id).Take(pageSize + 1)
            .Select(session => new GrammarPracticeSessionSummaryDto(session.Id.ToString(), session.LearningProfileId.ToString(),
                session.LanguageId.ToString(), session.GrammarPointId == null ? null : session.GrammarPointId.Value.ToString(),
                session.ExplanationLanguage, session.QuestionCount, session.Status,
                session.Items.Count(item => item.AnsweredAt != null),
                session.Items.Count(item => item.SelectedOptionIndex != null && item.SelectedOptionIndex == item.CorrectOptionIndex),
                session.Version, session.CreatedAt, session.UpdatedAt, session.EndedAt)).ToListAsync(ct);
        if (items.Count == 0 && !await db.LearningProfiles.AnyAsync(profile => profile.Id == profileId && profile.UserId == userId, ct))
            throw new NotFoundException("Learning profile was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new GrammarPracticeSessionPage(items, hasNext ? items[^1].Id : null);
    }
    public async Task SaveAnswerAsync(long userId, GrammarPracticeSession session, GrammarPracticeSessionItem item,
        long expectedVersion, CancellationToken ct)
    {
        if (item.SessionId != session.Id) throw new InvalidOperationException("The question must belong to the session.");
        if (session.Version == expectedVersion)
        {
            await CheckCurrentAsync(userId, session, expectedVersion, ct);
            return;
        }

        // Root CAS and the answer must commit together. A failed child CAS also rolls back the root update.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        var updated = await OwnedSessions(userId, session.LearningProfileId)
            .Where(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == GrammarPracticeSessionStatus.InProgress)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, session.Status).SetProperty(x => x.EndedAt, session.EndedAt)
                .SetProperty(x => x.Version, session.Version).SetProperty(x => x.UpdatedAt, session.UpdatedAt), ct);
        if (updated != 1) throw Changed();
        var answered = await db.GrammarPracticeSessionItems
            .Where(x => x.Id == item.Id && x.SessionId == session.Id && x.Version == 1 && x.SelectedOptionIndex == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SelectedOptionIndex, item.SelectedOptionIndex).SetProperty(x => x.AnsweredAt, item.AnsweredAt)
                .SetProperty(x => x.Version, item.Version).SetProperty(x => x.UpdatedAt, item.UpdatedAt), ct);
        if (answered != 1) throw Changed();
        await transaction.CommitAsync(ct);
    }

    public async Task SaveAbandonAsync(long userId, GrammarPracticeSession session, long expectedVersion, CancellationToken ct)
    {
        if (session.Version == expectedVersion)
        {
            await CheckCurrentAsync(userId, session, expectedVersion, ct);
            return;
        }
        // Only one row changes: a single conditional UPDATE is already atomic.
        var updated = await OwnedSessions(userId, session.LearningProfileId)
            .Where(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == GrammarPracticeSessionStatus.InProgress)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, session.Status).SetProperty(x => x.EndedAt, session.EndedAt)
                .SetProperty(x => x.Version, session.Version).SetProperty(x => x.UpdatedAt, session.UpdatedAt), ct);
        if (updated != 1) throw Changed();
    }

    private async Task CheckCurrentAsync(long userId, GrammarPracticeSession session, long expectedVersion, CancellationToken ct)
    {
        if (!await OwnedSessions(userId, session.LearningProfileId)
                .AnyAsync(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == session.Status, ct))
            throw Changed();
    }

    private static ConflictException Changed() => new("The practice session changed. Reload it before retrying.");
}
