namespace Application.Common.Interfaces;

public interface IAuthenticationPolicy
{
    TimeSpan RefreshTokenLifetime { get; }
    TimeSpan SessionLifetime { get; }
}
