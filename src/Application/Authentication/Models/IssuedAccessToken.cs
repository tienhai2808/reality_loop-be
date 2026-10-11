namespace Application.Authentication.Models;

public sealed record IssuedAccessToken(string Token, DateTimeOffset ExpiresAt);
