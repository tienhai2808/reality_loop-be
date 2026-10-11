using Application.Authentication.Commands.GoogleLogin;
using Application.Authentication.Commands.FacebookLogin;
using Application.Authentication.Queries.GetFacebookAuthorizationUrl;
using Application.Authentication.Commands.MicrosoftLogin;
using Application.Authentication.Queries.GetMicrosoftAuthorizationUrl;
using Application.Authentication.Commands.RefreshToken;
using Application.Authentication.Commands.Logout;
using Application.Authentication.Commands.LogoutSessions;
using Application.Authentication.Commands.LogoutAllSessions;
using Application.Authentication.Commands.LogoutOtherSessions;
using Application.Authentication.Queries.GetSessions;
using Application.Authentication.Services;
using Application.Authentication.Commands.RevokeRefreshToken;
using Application.Authentication.Queries.GetCurrentUser;
using Application.Authentication.Queries.GetGoogleAuthorizationUrl;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetGoogleAuthorizationUrlQueryHandler>();
        services.AddScoped<GoogleLoginCommandHandler>();
        services.AddScoped<GetMicrosoftAuthorizationUrlQueryHandler>();
        services.AddScoped<MicrosoftLoginCommandHandler>();
        services.AddScoped<GetFacebookAuthorizationUrlQueryHandler>();
        services.AddScoped<FacebookLoginCommandHandler>();
        services.AddScoped<OAuthLoginService>();
        services.AddScoped<RefreshTokenCommandHandler>();
        services.AddScoped<SessionLogoutService>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<LogoutSessionsCommandHandler>();
        services.AddScoped<LogoutAllSessionsCommandHandler>();
        services.AddScoped<LogoutOtherSessionsCommandHandler>();
        services.AddScoped<GetSessionsQueryHandler>();
        services.AddScoped<RevokeRefreshTokenCommandHandler>();
        services.AddScoped<GetCurrentUserQueryHandler>();
        return services;
    }
}
