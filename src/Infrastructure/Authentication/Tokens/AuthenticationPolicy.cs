using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication.Tokens;

internal sealed class AuthenticationPolicy(IOptions<SessionOptions> options) : IAuthenticationPolicy
{
    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(options.Value.RefreshTokenLifetimeDays);
    public TimeSpan SessionLifetime => TimeSpan.FromDays(options.Value.AbsoluteLifetimeDays);
}
