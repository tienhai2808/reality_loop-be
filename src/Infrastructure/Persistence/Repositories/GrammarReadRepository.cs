using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Grammar.Models;
using Domain.Grammar;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class GrammarReadRepository(ApplicationDbContext db) : IGrammarReadRepository
{
    private IQueryable<GrammarPoint> PublishedPoints(string languageCode) =>
        from point in db.GrammarPoints.AsNoTracking()
        join language in db.Languages.AsNoTracking() on point.LanguageId equals language.Id
        where language.Code == languageCode && language.IsEnabled && point.IsPublished
        select point;

    public async Task<GrammarPointPage> GetPageAsync(string languageCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = PublishedPoints(languageCode);
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        // Bound the page before translation lookup and project summary fields only.
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await (
            from point in page
            from translation in db.GrammarPointTranslations.AsNoTracking()
                .Where(x => x.GrammarPointId == point.Id && (x.Locale == locale || x.Locale == "en"))
                .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
            orderby point.SortOrder, point.Id
            select new GrammarPointSummaryDto(point.Id.ToString(), languageCode, point.Code, point.NativeName, point.Pattern,
                translation == null ? point.NativeName : translation.Name,
                translation == null ? null : translation.Locale,
                translation == null || translation.Locale != locale, point.SortOrder, point.Version,
                translation != null && translation.UpdatedAt > point.UpdatedAt ? translation.UpdatedAt : point.UpdatedAt))
            .ToListAsync(ct);
        // Only an empty page needs an extra query to distinguish no content from a hidden/missing language.
        if (items.Count == 0 && !await db.Languages.AsNoTracking().AnyAsync(x => x.Code == languageCode && x.IsEnabled, ct))
            throw new NotFoundException("Language was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new GrammarPointPage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    public Task<GrammarPointDto?> GetByCodeAsync(string languageCode, string pointCode, string locale, CancellationToken ct) =>
        (from point in PublishedPoints(languageCode).Where(x => x.Code == pointCode)
         from translation in db.GrammarPointTranslations.AsNoTracking()
             .Where(x => x.GrammarPointId == point.Id && (x.Locale == locale || x.Locale == "en"))
             .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
         select new GrammarPointDto(point.Id.ToString(), languageCode, point.Code, point.NativeName, point.Pattern,
             translation == null ? point.NativeName : translation.Name,
             translation == null ? null : translation.Explanation,
             translation == null ? null : translation.UsageNote,
             translation == null ? null : translation.Locale,
             translation == null || translation.Locale != locale, point.SortOrder, point.Version,
             translation != null && translation.UpdatedAt > point.UpdatedAt ? translation.UpdatedAt : point.UpdatedAt))
        .SingleOrDefaultAsync(ct);
}
