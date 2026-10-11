using Application.Authentication.Models;
using Application.Common.Interfaces;

namespace Application.Authentication.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(ICurrentUser currentUser)
{
    public Task<UserDto> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(currentUser.User);
    }
}
