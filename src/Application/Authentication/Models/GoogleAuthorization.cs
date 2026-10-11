namespace Application.Authentication.Models;

public sealed record GoogleAuthorization(string AuthorizationUrl, string State, string ProtectedState);
