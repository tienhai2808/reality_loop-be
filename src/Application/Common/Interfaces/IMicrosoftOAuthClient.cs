using Application.Authentication.Models;

namespace Application.Common.Interfaces;

public interface IMicrosoftOAuthClient
{
    MicrosoftAuthorization CreateAuthorization();
    Task<OAuthIdentity> ExchangeCodeAsync(string code, string state, string protectedState, CancellationToken ct);
}
