using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Pronunciation.Models;
using Domain.Pronunciation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class PronunciationReadRepository(ApplicationDbContext db) : IPronunciationReadRepository
{
    private IQueryable<PronunciationRule> PublishedRules(string languageCode) =>
        from rule in db.PronunciationRules.AsNoTracking()
        join language in db.Languages.AsNoTracking() on rule.LanguageId equals language.Id
        where language.Code == languageCode && language.IsEnabled && rule.IsPublished
        select rule;

    public async Task<PronunciationRulePage> GetPageAsync(string languageCode, string locale,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var query = PublishedRules(languageCode);
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        // Limit rules before looking up their translation; list does not load explanations or examples.
        var page = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1);
        var items = await (
            from rule in page
            from translation in db.PronunciationRuleTranslations.AsNoTracking()
                .Where(x => x.PronunciationRuleId == rule.Id && (x.Locale == locale || x.Locale == "en"))
                .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
            orderby rule.SortOrder, rule.Id
            select new PronunciationRuleSummaryDto(rule.Id.ToString(), languageCode, rule.Code, rule.NativeName,
                translation == null ? rule.NativeName : translation.Name,
                translation == null ? null : translation.Locale,
                translation == null || translation.Locale != locale, rule.SortOrder, rule.Version,
                translation != null && translation.UpdatedAt > rule.UpdatedAt ? translation.UpdatedAt : rule.UpdatedAt))
            .ToListAsync(ct);
        // Only an empty page needs an extra query to distinguish no content from a hidden/missing language.
        if (items.Count == 0 && !await db.Languages.AsNoTracking().AnyAsync(x => x.Code == languageCode && x.IsEnabled, ct))
            throw new NotFoundException("Language was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new PronunciationRulePage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }

    public Task<PronunciationRuleDto?> GetByCodeAsync(string languageCode, string ruleCode, string locale, CancellationToken ct) =>
        (from rule in PublishedRules(languageCode).Where(x => x.Code == ruleCode)
         from translation in db.PronunciationRuleTranslations.AsNoTracking()
             .Where(x => x.PronunciationRuleId == rule.Id && (x.Locale == locale || x.Locale == "en"))
             .OrderBy(x => x.Locale == locale ? 0 : 1).Take(1).DefaultIfEmpty()
         select new PronunciationRuleDto(rule.Id.ToString(), languageCode, rule.Code, rule.NativeName,
             translation == null ? rule.NativeName : translation.Name,
             translation == null ? null : translation.Explanation,
             translation == null ? null : translation.UsageNote,
             translation == null ? null : translation.Locale,
             translation == null || translation.Locale != locale, rule.SortOrder, rule.Version,
             translation != null && translation.UpdatedAt > rule.UpdatedAt ? translation.UpdatedAt : rule.UpdatedAt))
        .SingleOrDefaultAsync(ct);

    public async Task<PronunciationExamplePage> GetExamplesPageAsync(string languageCode, string ruleCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct)
    {
        var visibleRules = PublishedRules(languageCode).Where(x => x.Code == ruleCode);
        var query = from example in db.PronunciationExamples.AsNoTracking()
                    join rule in visibleRules on example.PronunciationRuleId equals rule.Id
                    select example;
        if (cursor is not null)
            query = query.Where(x => x.SortOrder > cursor.SortOrder || (x.SortOrder == cursor.SortOrder && x.Id > cursor.Id));
        var items = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Take(pageSize + 1)
            .Select(example => new PronunciationExampleDto(example.Id.ToString(), example.Code, example.Text,
                example.Transcription, example.TranscriptionSystem, example.AudioUrl, example.SortOrder,
                example.Version, example.UpdatedAt))
            .ToListAsync(ct);
        if (items.Count == 0 && !await visibleRules.AnyAsync(ct))
            throw new NotFoundException("Pronunciation rule was not found.");
        var hasNext = items.Count > pageSize;
        if (hasNext) items.RemoveAt(items.Count - 1);
        return new PronunciationExamplePage(items, hasNext ? CatalogCursor.Encode(items[^1].SortOrder, items[^1].Id) : null);
    }
}
