using Application.Authentication.Models;

namespace Application.Authentication.Commands.FacebookLogin;

public sealed record FacebookLoginCommand(string Code, string State, string ProtectedState, LoginDevice Device);
