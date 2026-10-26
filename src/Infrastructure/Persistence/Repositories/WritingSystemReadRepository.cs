using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.WritingSystems.Models;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WritingSystemReadRepository(ApplicationDbContext db) : IWritingSystemReadRepository
{
    private IQueryable<WritingSystem> PublishedSystems(string languageCode) =>
        from system in db.WritingSystems.AsNoTracking()
        join language in db.Languages.AsNoTracking() on system.LanguageId equals language.Id
        where language.Code == languageCode && language.IsEnabled && system.IsPublished
        select system;

    private IQueryable<WritingSystemItem> PublishedItems(string languageCode, string systemCode) =>
        from item in db.WritingSystemItems.AsNoTracking()
        join system in PublishedSystems(languageCode).Where(x => x.Code == systemCode)
            on item.WritingSystemId equals system.Id
        select item;

    public async Task<WritingSystemPage> GetPageAsync(string languageCode, string locale, int pageSize,
        CatalogCursor? cursor, CancellationToken ct)
    {
        var query = PublishedSystems(languageCode);
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await Localize(page, languageCode, locale).ToListAsync(ct);
        // Only an empty page needs an extra query to distinguish an empty catalog from a hidden/missing parent.
        if (items.Count == 0 && !await db.Languages.AsNoTracking().AnyAsync(x => x.Code == languageCode && x.IsEnabled, ct))
            throw new NotFoundException("Language was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new WritingSystemPage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    public Task<WritingSystemDto?> GetByCodeAsync(string languageCode, string systemCode, string locale, CancellationToken ct) =>
        Localize(PublishedSystems(languageCode).Where(x => x.Code == systemCode), languageCode, locale)
            .SingleOrDefaultAsync(ct);

    public async Task<WritingSystemItemPage> GetItemsPageAsync(string languageCode, string systemCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = PublishedItems(languageCode, systemCode);
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        var items = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1)
            .Select(item => new WritingSystemItemDto(item.Id.ToString(), item.Code, item.Symbol, item.SortOrder,
                item.RowIndex, item.ColumnIndex, item.Version, item.UpdatedAt,
                db.WritingSystemItemReadings.AsNoTracking().Where(reading => reading.WritingSystemItemId == item.Id)
                    .OrderBy(reading => reading.SortOrder).ThenBy(reading => reading.Id)
                    .Select(reading => new WritingSystemItemReadingDto(reading.Id.ToString(), reading.Code,
                        reading.Transcription, reading.TranscriptionSystem, reading.AudioUrl, reading.SortOrder,
                        reading.Version, reading.UpdatedAt)).FirstOrDefault()))
            .ToListAsync(ct);
        if (items.Count == 0 && !await PublishedSystems(languageCode).AnyAsync(x => x.Code == systemCode, ct))
            throw new NotFoundException("Writing system was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new WritingSystemItemPage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    public async Task<WritingSystemItemReadingPage> GetReadingsPageAsync(string languageCode, string systemCode,
        string itemCode, int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var visibleItems = PublishedItems(languageCode, systemCode).Where(x => x.Code == itemCode);
        var query = from reading in db.WritingSystemItemReadings.AsNoTracking()
                    join item in visibleItems on reading.WritingSystemItemId equals item.Id
                    select reading;
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        var items = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1)
            .Select(reading => new WritingSystemItemReadingDto(reading.Id.ToString(), reading.Code, reading.Transcription,
                reading.TranscriptionSystem, reading.AudioUrl, reading.SortOrder, reading.Version, reading.UpdatedAt))
            .ToListAsync(ct);
        if (items.Count == 0 && !await visibleItems.AnyAsync(ct))
            throw new NotFoundException("Writing system item was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new WritingSystemItemReadingPage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    private IQueryable<WritingSystemDto> Localize(IQueryable<WritingSystem> systems, string languageCode, string locale) =>
        from system in systems
        from translation in db.WritingSystemTranslations.AsNoTracking()
            .Where(x => x.WritingSystemId == system.Id && (x.Locale == locale || x.Locale == "en"))
            .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
        orderby system.SortOrder, system.Id
        select new WritingSystemDto(system.Id.ToString(), languageCode, system.Code, system.NativeName,
            translation == null ? system.NativeName : translation.Name,
            translation == null ? null : translation.Description,
            translation == null ? null : translation.Locale,
            translation == null || translation.Locale != locale, system.SortOrder, system.Version,
            translation != null && translation.UpdatedAt > system.UpdatedAt ? translation.UpdatedAt : system.UpdatedAt);
}
