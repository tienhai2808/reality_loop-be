using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.VocabularyLearning.Models;
using Domain.VocabularyLearning;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

internal sealed class VocabularyLearningRepository(ApplicationDbContext db) : IVocabularyLearningRepository
{
    private IQueryable<VocabularyLearningProgress> Owned(long userId, long profileId) =>
        db.VocabularyLearningProgress.AsNoTracking().Where(p => p.LearningProfileId == profileId &&
            db.LearningProfiles.Any(profile => profile.Id == p.LearningProfileId && profile.UserId == userId));

    private IQueryable<VocabularyLearningProgress> Available(long userId, long profileId) =>
        from progress in Owned(userId, profileId)
        join profile in db.LearningProfiles.AsNoTracking() on progress.LearningProfileId equals profile.Id
        join sense in db.VocabularySenses.AsNoTracking() on progress.VocabularySenseId equals sense.Id
        join entry in db.VocabularyEntries.AsNoTracking() on sense.VocabularyEntryId equals entry.Id
        join language in db.Languages.AsNoTracking() on entry.LanguageId equals language.Id
        where entry.IsPublished && language.IsEnabled && language.Code == profile.TargetLanguage
        select progress;

    public Task<VocabularyLearningContext?> GetAddContextAsync(long userId, long profileId, long senseId, CancellationToken ct) =>
        (from profile in db.LearningProfiles.AsNoTracking()
         join language in db.Languages.AsNoTracking() on profile.TargetLanguage equals language.Code
         join entry in db.VocabularyEntries.AsNoTracking() on language.Id equals entry.LanguageId
         join sense in db.VocabularySenses.AsNoTracking() on entry.Id equals sense.VocabularyEntryId
         from progress in db.VocabularyLearningProgress.AsNoTracking()
             .Where(x => x.LearningProfileId == profile.Id && x.VocabularySenseId == sense.Id).DefaultIfEmpty()
         where profile.Id == profileId && profile.UserId == userId && sense.Id == senseId && entry.IsPublished && language.IsEnabled
         select new VocabularyLearningContext(profile, language, entry, sense, progress)).SingleOrDefaultAsync(ct);

    public async Task<VocabularyProgressCreationResult> CreateAsync(long userId, VocabularyLearningProgress progress, CancellationToken ct)
    {
        // Single atomic insert arbitrates concurrent adds; existing cards are never reset or updated.
        int count;
        try
        {
            count = await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO vocabulary_learning_progress
                    (id, learning_profile_id, vocabulary_sense_id, status, next_review_at, last_reviewed_at,
                     interval_minutes, review_count, again_count, consecutive_recall_count, scheduler_version, version, created_at, updated_at)
                SELECT {progress.Id}, profile.id, sense.id, 'New', {progress.NextReviewAt}, NULL,
                    0, 0, 0, 0, NULL, 1, {progress.CreatedAt}, {progress.UpdatedAt}
                FROM learning_profiles AS profile
                JOIN languages AS language ON language.code = profile.target_language AND language.is_enabled
                JOIN vocabulary_entries AS entry ON entry.language_id = language.id AND entry.is_published
                JOIN vocabulary_senses AS sense ON sense.vocabulary_entry_id = entry.id
                WHERE profile.id = {progress.LearningProfileId} AND profile.user_id = {userId} AND sense.id = {progress.VocabularySenseId}
                FOR SHARE OF profile, language, entry, sense
                ON CONFLICT (learning_profile_id, vocabulary_sense_id) DO NOTHING
                """, ct);
        }
        catch (PostgresException ex) when (ex.SqlState is PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.UniqueViolation)
        {
            throw new ConflictException("Learning profile, vocabulary content or progress identity changed. Reload before retrying.");
        }
        if (count == 1) return new(VocabularyProgressDto.From(progress), true);
        var existing = await Available(userId, progress.LearningProfileId).SingleOrDefaultAsync(x => x.VocabularySenseId == progress.VocabularySenseId, ct);
        return existing is null ? throw new ConflictException("Learning profile or vocabulary content changed. Reload before retrying.")
            : new(VocabularyProgressDto.From(existing), false);
    }

    public Task<VocabularyReviewContext?> GetReviewContextAsync(long userId, long profileId, long progressId, Guid requestId, CancellationToken ct) =>
        (from progress in Owned(userId, profileId).Where(x => x.Id == progressId)
         from review in db.VocabularyReviews.AsNoTracking().Where(x => x.ProgressId == progress.Id && x.RequestId == requestId).DefaultIfEmpty()
         select new VocabularyReviewContext(progress, Available(userId, profileId).Any(x => x.Id == progress.Id), review)).SingleOrDefaultAsync(ct);

    public async Task<VocabularyReviewDto> SaveReviewAsync(long userId, VocabularyLearningProgress progress, VocabularyReview r, CancellationToken ct)
    {
        if (r.ProgressId != progress.Id || r.ProgressVersionAfter != progress.Version || r.NextReviewAt != progress.NextReviewAt)
            throw new InvalidOperationException("Review and progress must be the same complete domain mutation.");
        try
        {
            // One PostgreSQL statement is the transaction boundary: source locks, root CAS and log insert
            // either succeed together or all roll back. No read/write transaction spans user think time.
            var count = await db.Database.ExecuteSqlInterpolatedAsync($"""
                WITH eligible AS MATERIALIZED (
                    SELECT p.id
                    FROM vocabulary_learning_progress AS p
                    JOIN learning_profiles AS profile ON profile.id = p.learning_profile_id
                    JOIN vocabulary_senses AS sense ON sense.id = p.vocabulary_sense_id
                    JOIN vocabulary_entries AS entry ON entry.id = sense.vocabulary_entry_id AND entry.is_published
                    JOIN languages AS language ON language.id = entry.language_id AND language.is_enabled
                    WHERE p.id = {progress.Id} AND profile.id = {progress.LearningProfileId} AND profile.user_id = {userId}
                        AND language.code = profile.target_language
                    FOR SHARE OF profile, sense, entry, language
                ), changed AS (
                    UPDATE vocabulary_learning_progress AS p
                    SET status = {progress.Status.ToString()}, next_review_at = {progress.NextReviewAt}, last_reviewed_at = {progress.LastReviewedAt},
                        interval_minutes = {progress.IntervalMinutes}, review_count = {progress.ReviewCount}, again_count = {progress.AgainCount},
                        consecutive_recall_count = {progress.ConsecutiveRecallCount}, scheduler_version = {progress.SchedulerVersion},
                        version = {progress.Version}, updated_at = {progress.UpdatedAt}
                    FROM eligible WHERE p.id = eligible.id AND p.version = {r.ProgressVersionBefore}
                    RETURNING p.id
                )
                INSERT INTO vocabulary_reviews
                    (id, progress_id, request_id, review_number, rating, previous_status, status, previous_due_at, next_review_at,
                     previous_interval_minutes, interval_minutes, again_count, consecutive_recall_count, progress_version_before,
                     progress_version_after, scheduler_version, reviewed_at, version, created_at, updated_at)
                SELECT {r.Id}, changed.id, {r.RequestId}, {r.ReviewNumber}, {r.Rating.ToString()}, {r.PreviousStatus.ToString()},
                    {r.Status.ToString()}, {r.PreviousDueAt}, {r.NextReviewAt}, {r.PreviousIntervalMinutes}, {r.IntervalMinutes},
                    {r.AgainCount}, {r.ConsecutiveRecallCount}, {r.ProgressVersionBefore}, {r.ProgressVersionAfter},
                    {r.SchedulerVersion}, {r.ReviewedAt}, 1, {r.CreatedAt}, {r.UpdatedAt}
                FROM changed
                """, ct);
            if (count == 1) return VocabularyReviewDto.From(r);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // The statement (including its root update) was rolled back. Another caller may have
            // committed this request between our initial read and CAS; verify the persisted receipt.
        }
        var existing = await (from review in db.VocabularyReviews.AsNoTracking()
                              join p in Owned(userId, progress.LearningProfileId) on review.ProgressId equals p.Id
                              where review.ProgressId == progress.Id && review.RequestId == r.RequestId
                              select review).SingleOrDefaultAsync(ct);
        if (existing is not null && existing.Rating == r.Rating && existing.ProgressVersionBefore == r.ProgressVersionBefore)
            return VocabularyReviewDto.From(existing);
        throw new ConflictException("The learning progress, request or catalog changed. Reload the card before retrying.");
    }

    private async Task<List<VocabularyFlashcardDto>> LoadCardsAsync(IQueryable<VocabularyLearningProgress> query, string? locale, CancellationToken ct)
    {
        var rows = await (from p in query
                          join profile in db.LearningProfiles.AsNoTracking() on p.LearningProfileId equals profile.Id
                          join sense in db.VocabularySenses.AsNoTracking() on p.VocabularySenseId equals sense.Id
                          join entry in db.VocabularyEntries.AsNoTracking() on sense.VocabularyEntryId equals entry.Id
                          from translation in db.VocabularySenseTranslations.AsNoTracking()
                              .Where(t => t.VocabularySenseId == sense.Id && (t.Locale == (locale ?? profile.ExplanationLanguage) || t.Locale == "en"))
                              .OrderBy(t => t.Locale == (locale ?? profile.ExplanationLanguage) ? 0 : 1).Take(1).DefaultIfEmpty()
                          orderby p.NextReviewAt, p.Id
                          select new
                          {
                              Progress = p, EntryId = entry.Id, entry.Code, entry.Term, entry.Reading, entry.AudioUrl,
                              SenseCode = sense.Code, sense.PartOfSpeech, Requested = locale ?? profile.ExplanationLanguage,
                              Meaning = translation == null ? null : translation.Meaning,
                              UsageNote = translation == null ? null : translation.UsageNote,
                              Locale = translation == null ? null : translation.Locale
                          }).ToListAsync(ct);
        return rows.Select(x => new VocabularyFlashcardDto(VocabularyProgressDto.From(x.Progress), x.EntryId.ToString(),
            x.Code, x.Term, x.Reading, x.AudioUrl, x.SenseCode, x.PartOfSpeech, x.Meaning, x.UsageNote,
            x.Requested, x.Locale, x.Locale != x.Requested)).ToList();
    }

    public async Task<VocabularyFlashcardDto?> GetCardAsync(long userId, long profileId, long progressId, string? locale, CancellationToken ct) =>
        (await LoadCardsAsync(Available(userId, profileId).Where(x => x.Id == progressId), locale, ct)).SingleOrDefault();

    public async Task<VocabularyLearningPage> GetPageAsync(long userId, long profileId, bool dueOnly, VocabularyLearningStatus? status,
        int pageSize, VocabularyDueCursor? cursor, DateTimeOffset asOf, string? locale, CancellationToken ct)
    {
        var query = Available(userId, profileId);
        if (dueOnly) query = query.Where(x => x.NextReviewAt <= asOf);
        if (status is not null) query = query.Where(x => x.Status == status);
        if (cursor is not null) query = query.Where(x => x.NextReviewAt > cursor.DueAt || (x.NextReviewAt == cursor.DueAt && x.Id > cursor.Id));
        var items = await LoadCardsAsync(query.OrderBy(x => x.NextReviewAt).ThenBy(x => x.Id).Take(pageSize + 1), locale, ct);
        if (items.Count == 0 && !await db.LearningProfiles.AnyAsync(x => x.Id == profileId && x.UserId == userId, ct))
            throw new NotFoundException("Learning profile was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new(items, hasNext ? new VocabularyDueCursor(asOf, items[^1].Progress.NextReviewAt, long.Parse(items[^1].Progress.Id)).Encode() : null, asOf);
    }

    public Task<VocabularyLearningSummaryDto?> GetSummaryAsync(long userId, long profileId, DateTimeOffset asOf, CancellationToken ct) =>
        (from profile in db.LearningProfiles.AsNoTracking().Where(x => x.Id == profileId && x.UserId == userId)
         from p in Available(userId, profileId).DefaultIfEmpty()
         group p by profile.Id into cards
         select new VocabularyLearningSummaryDto(cards.Key.ToString(), cards.Count(x => x != null),
             cards.Count(x => x != null && x.Status == VocabularyLearningStatus.New),
             cards.Count(x => x != null && x.Status == VocabularyLearningStatus.Learning),
             cards.Count(x => x != null && x.Status == VocabularyLearningStatus.Reviewing),
             cards.Count(x => x != null && x.NextReviewAt <= asOf),
             cards.Min(x => x != null && x.NextReviewAt > asOf ? (DateTimeOffset?)x.NextReviewAt : null), asOf)).SingleOrDefaultAsync(ct);

    public async Task<VocabularyReviewPage> GetHistoryAsync(long userId, long profileId, long progressId, int pageSize, long? before, CancellationToken ct)
    {
        var query = from review in db.VocabularyReviews.AsNoTracking()
                    join p in Owned(userId, profileId).Where(x => x.Id == progressId) on review.ProgressId equals p.Id
                    select review;
        if (before is not null) query = query.Where(x => x.ReviewNumber < before);
        var rows = await query.OrderByDescending(x => x.ReviewNumber).Take(pageSize + 1).ToListAsync(ct);
        if (rows.Count == 0 && !await Owned(userId, profileId).AnyAsync(x => x.Id == progressId, ct))
            throw new NotFoundException("Vocabulary learning progress was not found.");
        var hasNext = rows.Count > pageSize;
        if (hasNext) rows.RemoveAt(rows.Count - 1);
        return new(rows.Select(VocabularyReviewDto.From).ToArray(), hasNext ? rows[^1].ReviewNumber.ToString(System.Globalization.CultureInfo.InvariantCulture) : null);
    }
}
