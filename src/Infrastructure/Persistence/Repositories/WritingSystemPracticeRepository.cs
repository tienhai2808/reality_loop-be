using System.Data;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WritingSystemPractice.Models;
using Domain.WritingSystemPractice;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WritingSystemPracticeRepository(ApplicationDbContext db) : IWritingSystemPracticeRepository
{
    private IQueryable<WritingSystemPracticeSession> OwnedSessions(long userId, long profileId) =>
        db.WritingSystemPracticeSessions.AsNoTracking().Where(session => session.LearningProfileId == profileId &&
            db.LearningProfiles.Any(profile => profile.Id == session.LearningProfileId && profile.UserId == userId));

    public async Task<WritingSystemPracticeCatalog?> GetCatalogAsync(long userId, long profileId, string systemCode, CancellationToken ct)
    {
        // Include every reading of each target so an alternative valid reading cannot become a distractor.
        // Left joins distinguish an owned empty catalog (400 when generating) from a missing/hidden one (404).
        var rows = await (
            from profile in db.LearningProfiles.AsNoTracking()
            join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
            join system in db.WritingSystems.AsNoTracking() on language.Id equals system.LanguageId
            from item in db.WritingSystemItems.AsNoTracking().Where(x => x.WritingSystemId == system.Id).DefaultIfEmpty()
            from reading in db.WritingSystemItemReadings.AsNoTracking()
                .Where(x => x.WritingSystemItemId == (long?)item.Id).DefaultIfEmpty()
            where profile.Id == profileId && profile.UserId == userId && language.IsEnabled &&
                system.IsPublished && system.Code == systemCode
            select new { Profile = profile, Language = language, System = system, Item = item, Reading = reading })
            .ToListAsync(ct);
        if (rows.Count == 0) return null;
        var items = rows.Where(x => x.Item is not null).GroupBy(x => x.Item.Id).Select(group =>
            new WritingSystemPracticeCatalogItem(group.First().Item,
                group.Where(x => x.Reading is not null).Select(x => x.Reading).ToArray())).ToArray();
        return new WritingSystemPracticeCatalog(rows[0].Profile, rows[0].Language, rows[0].System, items);
    }

    public async Task CreateAsync(long userId, WritingSystemPracticeSession session, CancellationToken ct)
    {
        if (session.Version != 1 || session.Status != WritingSystemPracticeSessionStatus.InProgress ||
            session.Items.Count != session.QuestionCount || session.Items.Any(x => x.SessionId != session.Id || x.AnsweredAt.HasValue))
            throw new InvalidOperationException("Only a new complete practice aggregate can be inserted.");
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            // Recheck owner, target language and publication at the write, then hold these catalog/profile
            // rows until all questions are inserted. Generation runs before the short write transaction.
            var inserted = await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO writing_system_practice_sessions
                    (id, learning_profile_id, writing_system_id, question_count, status, version, created_at, updated_at)
                SELECT {session.Id}, profile.id, system.id, {session.QuestionCount}, {session.Status.ToString()},
                    {session.Version}, {session.CreatedAt}, {session.UpdatedAt}
                FROM learning_profiles AS profile
                JOIN languages AS language ON language.code = profile.target_language AND language.is_enabled
                JOIN writing_systems AS system ON system.language_id = language.id AND system.is_published
                WHERE profile.id = {session.LearningProfileId} AND profile.user_id = {userId} AND system.id = {session.WritingSystemId}
                FOR SHARE OF profile, language, system
                """, ct);
            if (inserted != 1) throw new NotFoundException("Learning profile or published writing system is no longer available.");
            db.WritingSystemPracticeSessionItems.AddRange(session.Items);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            throw new NotFoundException("Learning profile or writing system item is no longer available.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("The practice session could not be created because its identifiers already exist.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException("The practice session identifier already exists.");
        }
    }

    public Task<WritingSystemPracticeSession?> FindOwnedAsync(long userId, long profileId, long sessionId, CancellationToken ct) =>
        OwnedSessions(userId, profileId).Where(x => x.Id == sessionId).Include(x => x.Items).AsSingleQuery().SingleOrDefaultAsync(ct);

    public async Task<WritingSystemPracticeSessionPage> GetPageAsync(long userId, long profileId, string? systemCode,
        WritingSystemPracticeSessionStatus? status, int pageSize, long? before, CancellationToken ct)
    {
        var query = OwnedSessions(userId, profileId);
        if (systemCode is not null)
            query = query.Where(x => db.WritingSystems.Any(system => system.Id == x.WritingSystemId && system.Code == systemCode));
        if (status is not null) query = query.Where(x => x.Status == status);
        if (before is { } cursor) query = query.Where(x => x.Id < cursor);
        // Only the requested history page is projected. Each count touches at most 20 indexed child rows.
        var items = await query.OrderByDescending(x => x.Id).Take(pageSize + 1)
            .Select(x => new WritingSystemPracticeSessionSummaryDto(x.Id.ToString(), x.LearningProfileId.ToString(),
                x.WritingSystemId.ToString(), x.QuestionCount, x.Status,
                x.Items.Count(item => item.AnsweredAt != null),
                x.Items.Count(item => item.SelectedOptionIndex != null && item.SelectedOptionIndex == item.CorrectOptionIndex),
                x.Version, x.CreatedAt, x.UpdatedAt, x.EndedAt)).ToListAsync(ct);
        if (items.Count == 0 && !await db.LearningProfiles.AnyAsync(x => x.Id == profileId && x.UserId == userId, ct))
            throw new NotFoundException("Learning profile was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new WritingSystemPracticeSessionPage(items, hasNext ? items[^1].Id : null);
    }

    public async Task SaveAnswerAsync(long userId, WritingSystemPracticeSession session, WritingSystemPracticeSessionItem item,
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
            .Where(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == WritingSystemPracticeSessionStatus.InProgress)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, session.Status).SetProperty(x => x.EndedAt, session.EndedAt)
                .SetProperty(x => x.Version, session.Version).SetProperty(x => x.UpdatedAt, session.UpdatedAt), ct);
        if (updated != 1) throw Changed();
        var answered = await db.WritingSystemPracticeSessionItems
            .Where(x => x.Id == item.Id && x.SessionId == session.Id && x.Version == 1 && x.SelectedOptionIndex == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SelectedOptionIndex, item.SelectedOptionIndex).SetProperty(x => x.AnsweredAt, item.AnsweredAt)
                .SetProperty(x => x.Version, item.Version).SetProperty(x => x.UpdatedAt, item.UpdatedAt), ct);
        if (answered != 1) throw Changed();
        await transaction.CommitAsync(ct);
    }

    public async Task SaveAbandonAsync(long userId, WritingSystemPracticeSession session, long expectedVersion, CancellationToken ct)
    {
        if (session.Version == expectedVersion)
        {
            await CheckCurrentAsync(userId, session, expectedVersion, ct);
            return;
        }
        // Only one row changes: a single conditional UPDATE is already atomic.
        var updated = await OwnedSessions(userId, session.LearningProfileId)
            .Where(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == WritingSystemPracticeSessionStatus.InProgress)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, session.Status).SetProperty(x => x.EndedAt, session.EndedAt)
                .SetProperty(x => x.Version, session.Version).SetProperty(x => x.UpdatedAt, session.UpdatedAt), ct);
        if (updated != 1) throw Changed();
    }

    private async Task CheckCurrentAsync(long userId, WritingSystemPracticeSession session, long expectedVersion, CancellationToken ct)
    {
        if (!await OwnedSessions(userId, session.LearningProfileId)
                .AnyAsync(x => x.Id == session.Id && x.Version == expectedVersion && x.Status == session.Status, ct))
            throw Changed();
    }

    private static ConflictException Changed() => new("The practice session changed. Reload it before retrying.");
}
