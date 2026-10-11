using Application.Authentication.Models;
using Application.Authentication.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Users;

namespace Application.Authentication.Commands.MicrosoftLogin;

public sealed class MicrosoftLoginCommandHandler(IMicrosoftOAuthClient microsoft, OAuthLoginService login)
{
    public async Task<AuthenticationResult> Handle(MicrosoftLoginCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || command.Code.Length > 4096 ||
            string.IsNullOrWhiteSpace(command.State) || command.State.Length > 128 ||
            string.IsNullOrWhiteSpace(command.ProtectedState))
            throw new RequestValidationException("Microsoft code and login state are required.");

        var identity = await microsoft.ExchangeCodeAsync(command.Code, command.State, command.ProtectedState, ct);
        return await login.LoginAsync(OAuthProvider.Microsoft, identity, command.Device, ct);
    }
}
