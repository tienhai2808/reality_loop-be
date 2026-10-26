using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Languages.Models;
using Domain.Languages;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class LanguageReadRepository(ApplicationDbContext db) : ILanguageReadRepository
{
    public async Task<LanguagePage> GetPageAsync(string locale, int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = db.Languages.AsNoTracking().Where(x => x.IsEnabled);
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        // Bound the parent page before looking up one translation per language.
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await Localize(page, locale).ToListAsync(ct);
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        var next = hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null;
        return new LanguagePage(items, next);
    }

    public Task<LanguageDto?> GetByCodeAsync(string languageCode, string locale, CancellationToken ct) =>
        Localize(db.Languages.AsNoTracking().Where(x => x.IsEnabled && x.Code == languageCode), locale)
            .SingleOrDefaultAsync(ct);

    private IQueryable<LanguageDto> Localize(IQueryable<Language> languages, string locale) =>
        from language in languages
        from translation in db.LanguageTranslations.AsNoTracking()
            .Where(x => x.LanguageId == language.Id && (x.Locale == locale || x.Locale == "en"))
            .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
        orderby language.SortOrder, language.Id
        select new LanguageDto(language.Id.ToString(), language.Code, language.NativeName,
            translation == null ? language.NativeName : translation.Name,
            translation == null ? null : translation.Description,
            translation == null ? null : translation.Locale,
            translation == null || translation.Locale != locale, language.SortOrder, language.Version,
            translation != null && translation.UpdatedAt > language.UpdatedAt ? translation.UpdatedAt : language.UpdatedAt);
}
