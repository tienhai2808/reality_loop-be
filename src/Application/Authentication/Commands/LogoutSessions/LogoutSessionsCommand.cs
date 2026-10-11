namespace Application.Authentication.Commands.LogoutSessions;

public sealed record LogoutSessionsCommand(IReadOnlyList<string> SessionIds);
