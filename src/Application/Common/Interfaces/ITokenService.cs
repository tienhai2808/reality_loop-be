using Domain.Users;
using Application.Authentication.Models;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    IssuedAccessToken CreateAccessToken(User user, Session session, DateTimeOffset now);
    IssuedRefreshToken CreateRefreshToken();
    string HashRefreshToken(string token);
}

