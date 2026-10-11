using System.Globalization;
using Domain.Users;

namespace Application.Authentication.Models;

public sealed record UserDto(string Id, string? Email, string FirstName, string LastName, string? AvatarUrl,
    string Status, DateTimeOffset CreatedAt)
{
    public static UserDto From(User user) => new(user.Id.ToString(CultureInfo.InvariantCulture), user.Email,
        user.FirstName, user.LastName, user.AvatarUrl, user.Status.ToString(), user.CreatedAt);
}
