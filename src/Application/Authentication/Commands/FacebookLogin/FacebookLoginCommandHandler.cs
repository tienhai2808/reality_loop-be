using Application.Authentication.Models;
using Application.Authentication.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Commands.FacebookLogin;

public sealed class FacebookLoginCommandHandler(IFacebookOAuthClient facebook, OAuthLoginService login)
{
    public async Task<AuthenticationResult> Handle(FacebookLoginCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || command.Code.Length > 4096 ||
            string.IsNullOrWhiteSpace(command.State) || command.State.Length > 128 ||
            string.IsNullOrWhiteSpace(command.ProtectedState))
            throw new RequestValidationException("Facebook code and login state are required.");

        var identity = await facebook.ExchangeCodeAsync(command.Code, command.State, command.ProtectedState, ct);
        return await login.LoginAsync(OAuthProvider.Facebook, identity, command.Device, ct);
    }
}
