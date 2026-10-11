namespace Application.Authentication.Models;

public sealed record OAuthIdentity(string AccountId, string? Email, string FirstName, string LastName, string? AvatarUrl);
