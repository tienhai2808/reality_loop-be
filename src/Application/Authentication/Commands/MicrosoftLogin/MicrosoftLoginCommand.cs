using Application.Authentication.Models;

namespace Application.Authentication.Commands.MicrosoftLogin;

public sealed record MicrosoftLoginCommand(string Code, string State, string ProtectedState, LoginDevice Device);
