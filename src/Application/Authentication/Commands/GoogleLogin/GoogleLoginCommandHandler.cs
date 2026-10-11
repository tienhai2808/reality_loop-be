using Application.Authentication.Models;
using Application.Authentication.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Commands.GoogleLogin;

public sealed class GoogleLoginCommandHandler(IGoogleOAuthClient google, OAuthLoginService login)
{
    public async Task<AuthenticationResult> Handle(GoogleLoginCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || command.Code.Length > 4096 ||
            string.IsNullOrWhiteSpace(command.State) || command.State.Length > 128 ||
            string.IsNullOrWhiteSpace(command.ProtectedState))
            throw new RequestValidationException("Google code and login state are required.");

        // Never hold database locks while waiting for Google.
        var identity = await google.ExchangeCodeAsync(command.Code, command.State, command.ProtectedState, ct);
        return await login.LoginAsync(OAuthProvider.Google,
            new OAuthIdentity(identity.Subject, identity.Email, identity.FirstName, identity.LastName, identity.AvatarUrl),
            command.Device, ct);
    }
}
