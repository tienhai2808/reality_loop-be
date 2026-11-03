using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WritingSystemPractice.Models;
using Application.WritingSystemPractice.Validation;

namespace Application.WritingSystemPractice.Queries.GetWritingSystemPracticeSession;

public sealed class GetWritingSystemPracticeSessionQueryHandler(ICurrentUser current, IWritingSystemPracticeRepository repository)
{
    public async Task<WritingSystemPracticeSessionDto> Handle(GetWritingSystemPracticeSessionQuery query, CancellationToken ct)
    {
        WritingSystemPracticeValidation.Id(query.ProfileId);
        WritingSystemPracticeValidation.Id(query.SessionId);
        var session = await repository.FindOwnedAsync(current.UserId, query.ProfileId, query.SessionId, ct)
            ?? throw new NotFoundException("Practice session was not found.");
        return WritingSystemPracticeSessionDto.From(session);
    }
}
