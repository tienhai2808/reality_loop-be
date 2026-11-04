using Application.Common.Models;
using Application.Handwriting.Models;
using Domain.Handwriting;

namespace Application.Common.Interfaces;

public interface IHandwritingRepository
{
    Task<HandwritingTemplatePage> GetTemplatesAsync(long userId, long profileId, string systemCode,
        int pageSize, CatalogCursor? cursor, CancellationToken ct);
    Task<HandwritingTemplateDto?> GetTemplateAsync(long userId, long profileId, string systemCode, long templateId, CancellationToken ct);
    Task<HandwritingCatalog?> GetCatalogAsync(long userId, long profileId, string systemCode, IReadOnlyCollection<long> templateIds, CancellationToken ct);
    Task CreateAsync(long userId, HandwritingPracticeAttempt attempt, HandwritingCatalog catalog, CancellationToken ct);
    Task<HandwritingAttemptPage> GetAttemptsAsync(long userId, long profileId, string? systemCode, bool? passed,
        int pageSize, long? before, CancellationToken ct);
    Task<HandwritingAttemptDto?> GetAttemptAsync(long userId, long profileId, long attemptId, CancellationToken ct);
}
