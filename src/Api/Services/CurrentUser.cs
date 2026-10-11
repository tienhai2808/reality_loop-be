using System.Globalization;
using Application.Authentication.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Api.Services;

internal sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private UserDto? _validatedUser;
    public long UserId => ReadId("sub");
    public long SessionId => ReadId("sid");
    public UserDto User => _validatedUser is { } user && user.Id == UserId.ToString(CultureInfo.InvariantCulture)
        ? user : throw new AuthenticationException("A valid authenticated session is required.");

    // Request-scoped snapshot populated only after JWT and primary DB validation.
    internal void SetValidatedUser(UserDto user) => _validatedUser = user;

    private long ReadId(string claim)
    {
        var principal = accessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true ||
            !long.TryParse(principal.FindFirst(claim)?.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new AuthenticationException("A valid access token is required.");
        return id;
    }
}
