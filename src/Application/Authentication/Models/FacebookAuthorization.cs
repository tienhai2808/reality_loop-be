namespace Application.Authentication.Models;

public sealed record FacebookAuthorization(string AuthorizationUrl, string State, string ProtectedState);
