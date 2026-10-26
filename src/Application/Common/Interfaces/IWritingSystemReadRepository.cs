using Application.Common.Models;
using Application.WritingSystems.Models;

namespace Application.Common.Interfaces;

public interface IWritingSystemReadRepository
{
    Task<WritingSystemPage> GetPageAsync(string languageCode, string locale, int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<WritingSystemDto?> GetByCodeAsync(string languageCode, string systemCode, string locale, CancellationToken ct);
    Task<WritingSystemItemPage> GetItemsPageAsync(string languageCode, string systemCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<WritingSystemItemReadingPage> GetReadingsPageAsync(string languageCode, string systemCode, string itemCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
}
