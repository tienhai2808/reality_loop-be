using System.Data;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Handwriting.Models;
using Application.Handwriting.Scoring;
using Domain.Handwriting;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class HandwritingRepository(ApplicationDbContext db, HandwritingScoringPolicy policy) : IHandwritingRepository
{
    private IQueryable<WritingSystemItem> VisibleItems(long userId, long profileId, string systemCode) =>
        from item in db.WritingSystemItems.AsNoTracking()
        join system in db.WritingSystems.AsNoTracking() on item.WritingSystemId equals system.Id
        join language in db.Languages.AsNoTracking() on system.LanguageId equals language.Id
        join profile in db.LearningProfiles.AsNoTracking() on language.Code equals profile.TargetLanguage
        where profile.Id == profileId && profile.UserId == userId && system.Code == systemCode &&
              language.IsEnabled && system.IsPublished
        select item;

    private Task<bool> HasVisibleSystemAsync(long userId, long profileId, string systemCode, CancellationToken ct) =>
        (from profile in db.LearningProfiles.AsNoTracking()
         join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
         join system in db.WritingSystems.AsNoTracking() on language.Id equals system.LanguageId
         where profile.Id == profileId && profile.UserId == userId && system.Code == systemCode && language.IsEnabled && system.IsPublished
         select system.Id).AnyAsync(ct);

    private IQueryable<HandwritingPracticeAttempt> OwnedAttempts(long userId, long profileId) =>
        db.HandwritingPracticeAttempts.AsNoTracking().Where(attempt => attempt.LearningProfileId == profileId &&
            db.LearningProfiles.Any(profile => profile.Id == attempt.LearningProfileId && profile.UserId == userId));

    public async Task<HandwritingTemplatePage> GetTemplatesAsync(long userId, long profileId, string systemCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = from template in db.HandwritingTemplates.AsNoTracking()
                    join item in VisibleItems(userId, profileId, systemCode) on template.WritingSystemItemId equals item.Id
                    where template.IsPublished && template.Symbol == item.Symbol && !db.HandwritingTemplates.Any(other =>
                        other.WritingSystemItemId == item.Id && other.IsPublished && other.Symbol == item.Symbol && other.Revision > template.Revision)
                    select new { Template = template, Item = item };
        if (cursor is not null)
            query = query.Where(x => x.Item.SortOrder > cursor.SortOrder || (x.Item.SortOrder == cursor.SortOrder && x.Item.Id > cursor.Id));
        var items = await query.OrderBy(x => x.Item.SortOrder).ThenBy(x => x.Item.Id).Take(pageSize + 1)
            .Select(x => new HandwritingTemplateSummaryDto(new HandwritingReferenceDto(x.Template.Id.ToString(), x.Item.Id.ToString(),
                    x.Template.Symbol, x.Template.Revision, new HandwritingSourceDto(x.Template.SourceUrl, x.Template.SourceRevision,
                        x.Template.Attribution, x.Template.License, x.Template.LicenseUrl)), x.Item.Code, x.Item.SortOrder,
                x.Template.Version, x.Template.UpdatedAt)).ToListAsync(ct);
        if (items.Count == 0 && !await HasVisibleSystemAsync(userId, profileId, systemCode, ct))
            throw new NotFoundException("Learning profile or published writing system was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new HandwritingTemplatePage(items,
            hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Reference.WritingSystemItemId) : null, policy);
    }

    public async Task<HandwritingTemplateDto?> GetTemplateAsync(long userId, long profileId, string systemCode, long templateId, CancellationToken ct)
    {
        var row = await (from template in db.HandwritingTemplates.AsNoTracking()
                         join item in VisibleItems(userId, profileId, systemCode) on template.WritingSystemItemId equals item.Id
                         where template.Id == templateId && template.IsPublished && template.Symbol == item.Symbol
                         select new { Template = template, Item = item }).SingleOrDefaultAsync(ct);
        return row is null ? null : new HandwritingTemplateDto(
            new(HandwritingReferenceDto.From(row.Template), row.Item.Code, row.Item.SortOrder,
                row.Template.Version, row.Template.UpdatedAt),
            row.Template.DrawingFormatVersion, row.Template.Drawing.Strokes, policy);
    }

    public async Task<HandwritingCatalog?> GetCatalogAsync(long userId, long profileId, string systemCode,
        IReadOnlyCollection<long> templateIds, CancellationToken ct)
    {
        var selected = from template in db.HandwritingTemplates.AsNoTracking()
                       join item in db.WritingSystemItems.AsNoTracking() on template.WritingSystemItemId equals item.Id
                       where templateIds.Contains(template.Id) && template.IsPublished && template.Symbol == item.Symbol
                       select new { Template = template, Item = item };
        // One bounded query loads the context and only the requested references (at most four).
        var rows = await (from profile in db.LearningProfiles.AsNoTracking()
                          join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
                          join system in db.WritingSystems.AsNoTracking() on language.Id equals system.LanguageId
                          from reference in selected.Where(x => x.Item.WritingSystemId == system.Id).DefaultIfEmpty()
                          where profile.Id == profileId && profile.UserId == userId && system.Code == systemCode &&
                                language.IsEnabled && system.IsPublished
                          select new { Profile = profile, Language = language, System = system, reference.Template, reference.Item }).ToListAsync(ct);
        if (rows.Count == 0) return null;
        return new(rows[0].Profile, rows[0].Language, rows[0].System,
            rows.Where(x => x.Template is not null).Select(x => new HandwritingCatalogItem(x.Template, x.Item)).ToArray());
    }

    public async Task CreateAsync(long userId, HandwritingPracticeAttempt attempt, HandwritingCatalog catalog, CancellationToken ct)
    {
        var references = catalog.Items.OrderBy(x => x.Template.Id).ToArray();
        if (attempt.Version != 1 || attempt.LearningProfileId != catalog.Profile.Id || attempt.WritingSystemId != catalog.WritingSystem.Id ||
            attempt.Characters.Count != attempt.CharacterCount || references.Length == 0 ||
            !attempt.Characters.Select(x => x.TemplateId).Distinct().Order().SequenceEqual(references.Select(x => x.Template.Id)))
            throw new InvalidOperationException("Only a complete new handwriting aggregate with matching references can be inserted.");
        var templateIds = references.Select(x => x.Template.Id).ToArray();
        var templateVersions = references.Select(x => x.Template.Version).ToArray();
        var itemVersions = references.Select(x => x.Item.Version).ToArray();
        try
        {
            // Scoring ran before this short transaction. Lock/recheck the context and exact reference
            // versions at the write so ownership/publication/catalog changes cannot race the insert.
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            var inserted = await db.Database.ExecuteSqlInterpolatedAsync($"""
                WITH context AS MATERIALIZED (
                    SELECT profile.id AS profile_id, system.id AS system_id
                    FROM learning_profiles AS profile
                    JOIN languages AS language ON language.code = profile.target_language AND language.is_enabled
                    JOIN writing_systems AS system ON system.language_id = language.id AND system.is_published
                    WHERE profile.id = {attempt.LearningProfileId} AND profile.user_id = {userId} AND system.id = {attempt.WritingSystemId}
                    FOR SHARE OF profile, language, system
                ), reference_rows AS MATERIALIZED (
                    SELECT template.id
                    FROM unnest({templateIds}::bigint[], {templateVersions}::bigint[], {itemVersions}::bigint[])
                        AS expected(template_id, template_version, item_version)
                    JOIN handwriting_templates AS template ON template.id = expected.template_id
                    JOIN writing_system_items AS item ON item.id = template.writing_system_item_id
                    JOIN context ON context.system_id = item.writing_system_id
                    WHERE template.is_published AND template.version = expected.template_version
                        AND item.version = expected.item_version AND item.symbol = template.symbol
                    ORDER BY template.id
                    FOR SHARE OF template, item
                )
                INSERT INTO handwriting_practice_attempts
                    (id, learning_profile_id, writing_system_id, target_text, algorithm_version, character_count,
                     passed, match_score, version, created_at, updated_at)
                SELECT {attempt.Id}, context.profile_id, context.system_id, {attempt.TargetText}, {attempt.AlgorithmVersion},
                    {attempt.CharacterCount}, {attempt.Passed}, {attempt.MatchScore}, {attempt.Version}, {attempt.CreatedAt}, {attempt.UpdatedAt}
                FROM context WHERE (SELECT count(*) FROM reference_rows) = {references.Length}
                """, ct);
            if (inserted != 1)
                throw new ConflictException("The learning profile, writing system or reference changed. Reload templates before retrying.");
            db.HandwritingPracticeCharacters.AddRange(attempt.Characters);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("The handwriting attempt could not be saved because its references or identifiers changed.");
        }
        catch (PostgresException ex) when (ex.SqlState is PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException("The handwriting attempt could not be saved because its references or identifiers changed.");
        }
    }

    public async Task<HandwritingAttemptPage> GetAttemptsAsync(long userId, long profileId, string? systemCode, bool? passed,
        int pageSize, long? before, CancellationToken ct)
    {
        var query = OwnedAttempts(userId, profileId);
        if (systemCode is not null)
            query = query.Where(x => db.WritingSystems.Any(system => system.Id == x.WritingSystemId && system.Code == systemCode));
        if (passed is not null) query = query.Where(x => x.Passed == passed);
        if (before is not null) query = query.Where(x => x.Id < before);
        // Summary only: never join or deserialize ink/feedback when listing history.
        var items = await query.OrderByDescending(x => x.Id).Take(pageSize + 1)
            .Select(x => new HandwritingAttemptSummaryDto(x.Id.ToString(), x.LearningProfileId.ToString(), x.WritingSystemId.ToString(),
                x.TargetText, x.AlgorithmVersion, x.CharacterCount, x.Passed, x.MatchScore, x.Version, x.CreatedAt, x.UpdatedAt)).ToListAsync(ct);
        if (items.Count == 0 && !await db.LearningProfiles.AnyAsync(x => x.Id == profileId && x.UserId == userId, ct))
            throw new NotFoundException("Learning profile was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new HandwritingAttemptPage(items, hasNext ? items[^1].Id : null);
    }

    public async Task<HandwritingAttemptDto?> GetAttemptAsync(long userId, long profileId, long attemptId, CancellationToken ct)
    {
        // History remains readable after unpublishing a template/system/language. Ownership is still required.
        var rows = await (from attempt in OwnedAttempts(userId, profileId).Where(x => x.Id == attemptId)
                          join character in db.HandwritingPracticeCharacters.AsNoTracking() on attempt.Id equals character.AttemptId
                          join template in db.HandwritingTemplates.AsNoTracking() on character.TemplateId equals template.Id
                          orderby character.Position
                          select new
                          {
                              Attempt = attempt, Character = character,
                              Reference = new HandwritingReferenceDto(template.Id.ToString(), template.WritingSystemItemId.ToString(),
                                  template.Symbol, template.Revision, new(template.SourceUrl, template.SourceRevision,
                                      template.Attribution, template.License, template.LicenseUrl))
                          }).ToListAsync(ct);
        if (rows.Count == 0) return null;
        return new(HandwritingAttemptSummaryDto.From(rows[0].Attempt), rows.Select(row =>
            new HandwritingAttemptCharacterDto(row.Character.Id.ToString(), row.Character.Position, row.Reference,
                row.Character.DrawingFormatVersion, row.Character.Drawing.Strokes, row.Character.FeedbackFormatVersion,
                HandwritingFeedbackDto.From(row.Character.Feedback))).ToArray());
    }
}
