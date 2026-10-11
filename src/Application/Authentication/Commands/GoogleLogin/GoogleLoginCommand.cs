using Application.Authentication.Models;

namespace Application.Authentication.Commands.GoogleLogin;

public sealed record GoogleLoginCommand(string Code, string State, string ProtectedState, LoginDevice Device);
