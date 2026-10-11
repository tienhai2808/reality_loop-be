using Application.Authentication.Models;

namespace Application.Common.Interfaces;

public interface ICurrentUser
{
    long UserId { get; }
    long SessionId { get; }
    UserDto User { get; }
}
