using System.Data;
using System.Linq.Expressions;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.LearningProfiles.Models;
using Domain.LearningProfiles;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class LearningProfileRepository(ApplicationDbContext db) : ILearningProfileRepository
{
    private static readonly Expression<Func<LearningProfile, LearningProfileDto>> Projection = profile => new(
        profile.Id.ToString(), profile.TargetLanguage, profile.NativeLanguage, profile.ExplanationLanguage,
        profile.ProficiencyLevel, profile.PrimaryGoal, profile.GoalNote, profile.Version, profile.CreatedAt, profile.UpdatedAt);

    public Task<LearningProfile?> FindOwnedAsync(long userId, long id, CancellationToken ct) =>
        db.LearningProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId && x.Id == id, ct);

    public Task<LearningProfileDto?> GetOwnedAsync(long userId, long id, CancellationToken ct) =>
        db.LearningProfiles.AsNoTracking().Where(x => x.UserId == userId && x.Id == id)
            .Select(Projection).SingleOrDefaultAsync(ct);

    public async Task<LearningProfilePage> GetPageAsync(long userId, int pageSize, long? after, CancellationToken ct)
    {
        var query = db.LearningProfiles.AsNoTracking().Where(x => x.UserId == userId);
        if (after is { } cursor) query = query.Where(x => x.Id > cursor);
        // The owner/ID alternate key supports this cursor without COUNT or OFFSET.
        var items = await query.OrderBy(x => x.Id).Select(Projection).Take(pageSize + 1).ToListAsync(ct);
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new LearningProfilePage(items, hasNext ? items[^1].Id : null);
    }

    public async Task CreateAsync(LearningProfile profile, CancellationToken ct)
    {
        db.LearningProfiles.Add(profile);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "ux_learning_profiles_user_id_target_language" })
        {
            throw new ConflictException("A learning profile already exists for this target language.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.ForeignKeyViolation, ConstraintName: "FK_learning_profiles_users_user_id" })
        {
            throw new AuthenticationException("The user account is no longer available.");
        }
    }

    public async Task UpdatePreferencesAsync(LearningProfile profile, long expectedVersion, CancellationToken ct)
    {
        var count = await db.LearningProfiles
            .Where(x => x.UserId == profile.UserId && x.Id == profile.Id && x.Version == expectedVersion)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.NativeLanguage, profile.NativeLanguage)
                .SetProperty(x => x.ExplanationLanguage, profile.ExplanationLanguage)
                .SetProperty(x => x.ProficiencyLevel, profile.ProficiencyLevel)
                .SetProperty(x => x.PrimaryGoal, profile.PrimaryGoal)
                .SetProperty(x => x.GoalNote, profile.GoalNote)
                .SetProperty(x => x.Version, profile.Version)
                .SetProperty(x => x.UpdatedAt, profile.UpdatedAt), ct);
        if (count != 1) throw new ConflictException("The learning profile changed. Reload it before retrying.");
    }

    public async Task DeleteAsync(long userId, long id, long version, CancellationToken ct)
    {
        // PostgreSQL also removes any matching selection atomically through the cascade FK.
        var count = await db.LearningProfiles.Where(x => x.UserId == userId && x.Id == id && x.Version == version)
            .ExecuteDeleteAsync(ct);
        if (count == 1) return;
        if (!await db.LearningProfiles.AsNoTracking().AnyAsync(x => x.UserId == userId && x.Id == id, ct))
            throw new NotFoundException("Learning profile was not found.");
        throw new ConflictException("The learning profile changed. Reload it before deleting.");
    }

    public Task<CurrentLearningProfileDto?> GetCurrentAsync(long userId, CancellationToken ct) =>
        (from selection in db.LearningProfileSelections.AsNoTracking()
         join profile in db.LearningProfiles.AsNoTracking()
             on new { selection.UserId, Id = selection.ActiveProfileId } equals new { profile.UserId, profile.Id }
         where selection.UserId == userId
         select new CurrentLearningProfileDto(new LearningProfileDto(profile.Id.ToString(), profile.TargetLanguage,
             profile.NativeLanguage, profile.ExplanationLanguage, profile.ProficiencyLevel, profile.PrimaryGoal,
             profile.GoalNote, profile.Version, profile.CreatedAt, profile.UpdatedAt), selection.UpdatedAt))
        .SingleOrDefaultAsync(ct);

    public async Task<CurrentLearningProfileDto> SetCurrentAsync(long userId, long profileId, DateTimeOffset now, CancellationToken ct)
    {
        now = now.ToUniversalTime();
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            // Lock the owned target before the selection, matching profile deletion's lock order.
            // The upsert serializes selections from the same user without a read-before-write race.
            var count = await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO learning_profile_selections (user_id, active_profile_id, updated_at)
                SELECT p.user_id, p.id, {now} FROM learning_profiles AS p
                WHERE p.user_id = {userId} AND p.id = {profileId}
                FOR KEY SHARE OF p
                ON CONFLICT (user_id) DO UPDATE SET active_profile_id = EXCLUDED.active_profile_id,
                    updated_at = GREATEST(learning_profile_selections.updated_at, EXCLUDED.updated_at)
                """, ct);
            if (count != 1) throw new NotFoundException("Learning profile was not found.");
            // Keep the selection write lock until this response snapshot has been read.
            var result = await GetCurrentAsync(userId, ct)
                ?? throw new NotFoundException("Learning profile was not found.");
            await transaction.CommitAsync(ct);
            return result;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw new NotFoundException("Learning profile was not found.");
        }
    }
}
