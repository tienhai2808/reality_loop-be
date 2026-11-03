using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.LearningProgress.Models;
using Domain.LearningProgress;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WritingSystemItemProgressRepository(ApplicationDbContext db) : IWritingSystemItemProgressRepository
{
    private IQueryable<WritingSystem> OwnedSystems(long userId, long profileId) =>
        from profile in db.LearningProfiles.AsNoTracking()
        join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
        join system in db.WritingSystems.AsNoTracking() on language.Id equals system.LanguageId
        where profile.UserId == userId && profile.Id == profileId && language.IsEnabled && system.IsPublished
        select system;

    private IQueryable<WritingSystemItem> OwnedItems(long userId, long profileId, string systemCode) =>
        from system in OwnedSystems(userId, profileId).Where(x => x.Code == systemCode)
        join item in db.WritingSystemItems.AsNoTracking() on system.Id equals item.WritingSystemId
        select item;

    public async Task<LearningProgressSummaryDto> GetSummaryAsync(long userId, long profileId, CancellationToken ct)
    {
        // Left joins retain the owned profile even if its language has no published catalog yet.
        // All counts are aggregated by PostgreSQL in one query, rather than loading individual progress rows.
        var rows = await (
            from profile in db.LearningProfiles.AsNoTracking()
            where profile.Id == profileId && profile.UserId == userId
            from language in db.Languages.AsNoTracking()
                .Where(x => x.Code == profile.TargetLanguage && x.IsEnabled).DefaultIfEmpty()
            from system in db.WritingSystems.AsNoTracking()
                .Where(x => x.LanguageId == (long?)language.Id && x.IsPublished).DefaultIfEmpty()
            from item in db.WritingSystemItems.AsNoTracking()
                .Where(x => x.WritingSystemId == (long?)system.Id).DefaultIfEmpty()
            from progress in db.WritingSystemItemProgress.AsNoTracking()
                .Where(x => x.LearningProfileId == profile.Id && x.WritingSystemItemId == (long?)item.Id).DefaultIfEmpty()
            group new { item, progress } by new
            {
                ProfileId = profile.Id, profile.TargetLanguage, SystemId = (long?)system.Id,
                SystemCode = system.Code, SortOrder = (int?)system.SortOrder
            } into grouped
            orderby grouped.Key.SortOrder, grouped.Key.SystemId
            select new
            {
                grouped.Key.ProfileId, grouped.Key.TargetLanguage, grouped.Key.SystemId, grouped.Key.SystemCode,
                Total = grouped.Count(x => x.item != null),
                Learning = grouped.Count(x => x.progress != null && x.progress.Status == WritingSystemItemProgressStatus.Learning),
                Completed = grouped.Count(x => x.progress != null && x.progress.Status == WritingSystemItemProgressStatus.Completed),
                LastStudiedAt = grouped.Max(x => (DateTimeOffset?)x.progress.LastStudiedAt)
            }).ToListAsync(ct);
        if (rows.Count == 0) throw new NotFoundException("Learning profile was not found.");
        var systems = rows.Where(x => x.SystemId.HasValue).Select(x => new WritingSystemProgressSummaryDto(
            x.SystemId!.Value.ToString(), x.SystemCode, x.Total, x.Total - x.Learning - x.Completed,
            x.Learning, x.Completed, x.LastStudiedAt)).ToArray();
        return new LearningProgressSummaryDto(rows[0].ProfileId.ToString(), rows[0].TargetLanguage, systems);
    }

    public async Task<WritingSystemItemProgressPage> GetPageAsync(long userId, long profileId, string systemCode,
        WritingSystemItemLearningState? status, int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = from item in OwnedItems(userId, profileId, systemCode)
                    from progress in db.WritingSystemItemProgress.AsNoTracking()
                        .Where(x => x.LearningProfileId == profileId && x.WritingSystemItemId == item.Id).DefaultIfEmpty()
                    select new { Item = item, Progress = progress };
        if (status == WritingSystemItemLearningState.NotStarted) query = query.Where(x => x.Progress == null);
        else if (status is not null)
        {
            var storedStatus = status == WritingSystemItemLearningState.Completed
                ? WritingSystemItemProgressStatus.Completed : WritingSystemItemProgressStatus.Learning;
            query = query.Where(x => x.Progress != null && x.Progress.Status == storedStatus);
        }
        if (cursor is not null)
            query = query.Where(x => x.Item.SortOrder > cursor.SortOrder ||
                (x.Item.SortOrder == cursor.SortOrder && x.Item.Id > cursor.Id));
        var items = await query.OrderBy(x => x.Item.SortOrder).ThenBy(x => x.Item.Id).Take(pageSize + 1)
            .Select(x => new WritingSystemItemProgressDto(profileId.ToString(), x.Item.Id.ToString(),
                x.Item.Code, x.Item.SortOrder,
                x.Progress == null ? WritingSystemItemLearningState.NotStarted :
                    x.Progress.Status == WritingSystemItemProgressStatus.Completed
                        ? WritingSystemItemLearningState.Completed : WritingSystemItemLearningState.Learning,
                x.Progress == null ? 0 : x.Progress.Version,
                x.Progress == null ? null : (DateTimeOffset?)x.Progress.LastStudiedAt,
                x.Progress == null ? null : x.Progress.CompletedAt,
                x.Progress == null ? null : (DateTimeOffset?)x.Progress.CreatedAt,
                x.Progress == null ? null : (DateTimeOffset?)x.Progress.UpdatedAt)).ToListAsync(ct);
        if (items.Count == 0 && !await OwnedSystems(userId, profileId).AnyAsync(x => x.Code == systemCode, ct))
            throw new NotFoundException("Learning profile or published writing system was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new WritingSystemItemProgressPage(items, hasNext
            ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].WritingSystemItemId) : null);
    }

    public Task<WritingSystemItemProgressDto?> GetItemAsync(long userId, long profileId, string systemCode,
        long itemId, CancellationToken ct) =>
        (from item in OwnedItems(userId, profileId, systemCode).Where(x => x.Id == itemId)
         from progress in db.WritingSystemItemProgress.AsNoTracking()
             .Where(x => x.LearningProfileId == profileId && x.WritingSystemItemId == item.Id).DefaultIfEmpty()
         select new WritingSystemItemProgressDto(profileId.ToString(), item.Id.ToString(), item.Code, item.SortOrder,
             progress == null ? WritingSystemItemLearningState.NotStarted :
                 progress.Status == WritingSystemItemProgressStatus.Completed
                     ? WritingSystemItemLearningState.Completed : WritingSystemItemLearningState.Learning,
             progress == null ? 0 : progress.Version,
             progress == null ? null : (DateTimeOffset?)progress.LastStudiedAt,
             progress == null ? null : progress.CompletedAt,
             progress == null ? null : (DateTimeOffset?)progress.CreatedAt,
             progress == null ? null : (DateTimeOffset?)progress.UpdatedAt)).SingleOrDefaultAsync(ct);

    public Task<WritingSystemItemProgressContext?> FindContextAsync(long userId, long profileId, string systemCode,
        long itemId, CancellationToken ct) =>
        (from profile in db.LearningProfiles.AsNoTracking()
         join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
         join system in db.WritingSystems.AsNoTracking() on language.Id equals system.LanguageId
         join item in db.WritingSystemItems.AsNoTracking() on system.Id equals item.WritingSystemId
         from progress in db.WritingSystemItemProgress.AsNoTracking()
             .Where(x => x.LearningProfileId == profile.Id && x.WritingSystemItemId == item.Id).DefaultIfEmpty()
         where profile.UserId == userId && profile.Id == profileId && language.IsEnabled && system.IsPublished &&
             system.Code == systemCode && item.Id == itemId
         select new WritingSystemItemProgressContext(profile, language, system, item, progress)).SingleOrDefaultAsync(ct);

    public async Task SaveAsync(long userId, string systemCode, WritingSystemItemProgress progress,
        long expectedVersion, CancellationToken ct)
    {
        try
        {
            if (expectedVersion == 0)
            {
                // One atomic INSERT...SELECT checks ownership/publication again and arbitrates concurrent creates.
                // PostgreSQL's FK/unique locks protect the write; no separate read/write transaction is needed.
                var inserted = await db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO writing_system_item_progress
                        (id, learning_profile_id, writing_system_item_id, status, last_studied_at,
                         completed_at, version, created_at, updated_at)
                    SELECT {progress.Id}, profile.id, item.id, {progress.Status.ToString()}, {progress.LastStudiedAt},
                        CAST({progress.CompletedAt} AS timestamptz), {progress.Version}, {progress.CreatedAt}, {progress.UpdatedAt}
                    FROM learning_profiles AS profile
                    JOIN languages AS language ON language.code = profile.target_language AND language.is_enabled
                    JOIN writing_systems AS system ON system.language_id = language.id AND system.is_published
                    JOIN writing_system_items AS item ON item.writing_system_id = system.id
                    WHERE profile.id = {progress.LearningProfileId} AND profile.user_id = {userId}
                        AND system.code = {systemCode} AND item.id = {progress.WritingSystemItemId}
                    ON CONFLICT (learning_profile_id, writing_system_item_id) DO NOTHING
                    """, ct);
                if (inserted == 1) return;
            }
            else
            {
                var eligibleItems = OwnedItems(userId, progress.LearningProfileId, systemCode);
                var query = db.WritingSystemItemProgress.Where(x => x.Id == progress.Id &&
                    x.LearningProfileId == progress.LearningProfileId && x.WritingSystemItemId == progress.WritingSystemItemId &&
                    x.Version == expectedVersion && eligibleItems.Any(item => item.Id == x.WritingSystemItemId));
                if (progress.Version == expectedVersion)
                {
                    // No-op still validates the current version/owner, without rewriting the row.
                    if (await query.AnyAsync(ct)) return;
                }
                else
                {
                    var updated = await query.ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, progress.Status)
                        .SetProperty(x => x.LastStudiedAt, progress.LastStudiedAt)
                        .SetProperty(x => x.CompletedAt, progress.CompletedAt)
                        .SetProperty(x => x.Version, progress.Version)
                        .SetProperty(x => x.UpdatedAt, progress.UpdatedAt), ct);
                    if (updated == 1) return;
                }
            }
            throw new ConflictException("The learning progress or catalog changed. Reload this item before retrying.");
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw new NotFoundException("Learning profile or writing system item is no longer available.");
        }
    }
}
