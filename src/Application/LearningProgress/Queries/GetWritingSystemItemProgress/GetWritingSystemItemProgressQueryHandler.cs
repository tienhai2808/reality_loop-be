using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.LearningProgress.Models;
using Application.LearningProgress.Validation;

namespace Application.LearningProgress.Queries.GetWritingSystemItemProgress;

public sealed class GetWritingSystemItemProgressQueryHandler(ICurrentUser current, IWritingSystemItemProgressRepository repository)
{
    public async Task<WritingSystemItemProgressDto> Handle(GetWritingSystemItemProgressQuery query, CancellationToken ct)
    {
        LearningProgressValidation.Id(query.ProfileId);
        LearningProgressValidation.Id(query.ItemId);
        var code = CatalogQueryValidation.Code(query.SystemCode);
        return await repository.GetItemAsync(current.UserId, query.ProfileId, code, query.ItemId, ct)
            ?? throw new NotFoundException("Learning profile or published writing system item was not found.");
    }
}
