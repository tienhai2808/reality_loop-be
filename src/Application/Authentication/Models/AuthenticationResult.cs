using System.Globalization;
using Domain.Users;

namespace Application.Authentication.Models;

public sealed record AuthenticationResult(string AccessToken, string RefreshToken, string TokenType,
    DateTimeOffset AccessTokenExpiresAt, DateTimeOffset RefreshTokenExpiresAt,
    string SessionId, UserDto User)
{
    public static AuthenticationResult Create(IssuedAccessToken access, IssuedRefreshToken refresh,
        DateTimeOffset refreshExpiresAt, Session session, User user) => new(access.Token, refresh.Token,
        "Bearer", access.ExpiresAt, refreshExpiresAt, session.Id.ToString(CultureInfo.InvariantCulture), UserDto.From(user));
}
