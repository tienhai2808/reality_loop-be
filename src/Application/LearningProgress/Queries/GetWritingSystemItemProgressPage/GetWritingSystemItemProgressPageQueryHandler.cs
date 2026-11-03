using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.LearningProgress.Models;
using Application.LearningProgress.Validation;

namespace Application.LearningProgress.Queries.GetWritingSystemItemProgressPage;

public sealed class GetWritingSystemItemProgressPageQueryHandler(ICurrentUser current, IWritingSystemItemProgressRepository repository)
{
    public Task<WritingSystemItemProgressPage> Handle(GetWritingSystemItemProgressPageQuery query, CancellationToken ct)
    {
        LearningProgressValidation.Id(query.ProfileId);
        LearningProgressValidation.Status(query.Status);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        var cursor = CatalogQueryValidation.Page(query.PageSize, query.After);
        return repository.GetPageAsync(current.UserId, query.ProfileId, code, query.Status, query.PageSize, cursor, ct);
    }
}
