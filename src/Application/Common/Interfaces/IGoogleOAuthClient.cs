using Application.Authentication.Models;

namespace Application.Common.Interfaces;

public interface IGoogleOAuthClient
{
    GoogleAuthorization CreateAuthorization();
    Task<GoogleIdentity> ExchangeCodeAsync(string code, string state, string protectedState, CancellationToken ct);
}
