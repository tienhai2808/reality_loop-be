namespace Application.Authentication.Models;

public sealed record MicrosoftAuthorization(string AuthorizationUrl, string State, string ProtectedState);
