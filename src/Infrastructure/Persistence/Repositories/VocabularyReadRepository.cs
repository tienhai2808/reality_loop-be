using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Vocabulary.Models;
using Domain.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class VocabularyReadRepository(ApplicationDbContext db) : IVocabularyReadRepository
{
    private IQueryable<VocabularyEntry> PublishedEntries(string languageCode) =>
        from entry in db.VocabularyEntries.AsNoTracking()
        join language in db.Languages.AsNoTracking() on entry.LanguageId equals language.Id
        where language.Code == languageCode && language.IsEnabled && entry.IsPublished
        select entry;

    public async Task<VocabularyEntryPage> GetPageAsync(string languageCode, string locale, PartOfSpeech? partOfSpeech,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = PublishedEntries(languageCode);
        if (partOfSpeech is not null)
            query = query.Where(entry => db.VocabularySenses.Any(sense =>
                sense.VocabularyEntryId == entry.Id && sense.PartOfSpeech == partOfSpeech));
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        // Page entries first, then select one (matching) sense and one translation per entry.
        // Neither full sense lists nor long usage notes are loaded for catalog cards.
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await (
            from entry in page
            from sense in db.VocabularySenses.AsNoTracking()
                .Where(x => x.VocabularyEntryId == entry.Id && (partOfSpeech == null || x.PartOfSpeech == partOfSpeech))
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(1).DefaultIfEmpty()
            from translation in db.VocabularySenseTranslations.AsNoTracking()
                .Where(x => x.VocabularySenseId == (long?)sense.Id && (x.Locale == locale || x.Locale == "en"))
                .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
            orderby entry.SortOrder, entry.Id
            select new VocabularyEntrySummaryDto(entry.Id.ToString(), languageCode, entry.Code, entry.Term,
                entry.Reading, entry.AudioUrl,
                sense == null ? null : new VocabularySenseSummaryDto(sense.Id.ToString(), sense.Code, sense.PartOfSpeech,
                    translation == null ? null : translation.Meaning, translation == null ? null : translation.Locale,
                    translation == null || translation.Locale != locale, sense.SortOrder, sense.Version,
                    translation != null && translation.UpdatedAt > sense.UpdatedAt ? translation.UpdatedAt : sense.UpdatedAt),
                entry.SortOrder, entry.Version,
                translation != null && translation.UpdatedAt > entry.UpdatedAt && translation.UpdatedAt > sense.UpdatedAt
                    ? translation.UpdatedAt : sense != null && sense.UpdatedAt > entry.UpdatedAt ? sense.UpdatedAt : entry.UpdatedAt))
            .ToListAsync(ct);
        if (items.Count == 0 && !await db.Languages.AsNoTracking().AnyAsync(x => x.Code == languageCode && x.IsEnabled, ct))
            throw new NotFoundException("Language was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new VocabularyEntryPage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    public Task<VocabularyEntryDto?> GetByCodeAsync(string languageCode, string entryCode, CancellationToken ct) =>
        PublishedEntries(languageCode).Where(x => x.Code == entryCode)
            .Select(x => new VocabularyEntryDto(x.Id.ToString(), languageCode, x.Code, x.Term, x.Reading,
                x.AudioUrl, x.SortOrder, x.Version, x.UpdatedAt)).SingleOrDefaultAsync(ct);

    public async Task<VocabularySensePage> GetSensesPageAsync(string languageCode, string entryCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var visibleEntries = PublishedEntries(languageCode).Where(x => x.Code == entryCode);
        var query = from sense in db.VocabularySenses.AsNoTracking()
                    join entry in visibleEntries on sense.VocabularyEntryId equals entry.Id
                    select sense;
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await (
            from sense in page
            from translation in db.VocabularySenseTranslations.AsNoTracking()
                .Where(x => x.VocabularySenseId == sense.Id && (x.Locale == locale || x.Locale == "en"))
                .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
            orderby sense.SortOrder, sense.Id
            select new VocabularySenseDto(sense.Id.ToString(), sense.Code, sense.PartOfSpeech,
                translation == null ? null : translation.Meaning, translation == null ? null : translation.UsageNote,
                translation == null ? null : translation.Locale, translation == null || translation.Locale != locale,
                sense.SortOrder, sense.Version,
                translation != null && translation.UpdatedAt > sense.UpdatedAt ? translation.UpdatedAt : sense.UpdatedAt))
            .ToListAsync(ct);
        if (items.Count == 0 && !await visibleEntries.AnyAsync(ct))
            throw new NotFoundException("Vocabulary entry was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new VocabularySensePage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }
}
