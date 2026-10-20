using Infrastructure.AI.Providers;
using Infrastructure.AI.Security;
using Infrastructure.Persistence.BackgroundServices;
using System.Text;
using Application.Common.Interfaces;
using IdGen;
using Infrastructure.Authentication.Google;
using Infrastructure.Authentication.Facebook;
using Infrastructure.Authentication.Microsoft;
using Infrastructure.Authentication.Tokens;
using Infrastructure.IdGeneration;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'Database' is required.");
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString,
            npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history")));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var snowflake = configuration.GetSection(SnowflakeIdOptions.SectionName).Get<SnowflakeIdOptions>()
            ?? throw new InvalidOperationException("Snowflake ID configuration is required.");
        if (snowflake.GeneratorId is < 0 or > 1023)
            throw new InvalidOperationException("SnowflakeId:GeneratorId must be between 0 and 1023.");
        services.AddSingleton(new IdGenerator(snowflake.GeneratorId));
        services.AddSingleton<IIdGenerator, SnowflakeIdGenerator>();
        services.AddSingleton(TimeProvider.System);

        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations()
            .Validate(x => Encoding.UTF8.GetByteCount(x.SigningKey) >= 32 && !x.SigningKey.StartsWith("__"),
                "Authentication:Jwt:SigningKey must contain at least 32 bytes and must replace the placeholder.")
            .ValidateOnStart();
        services.AddOptions<MicrosoftOAuthOptions>().BindConfiguration(MicrosoftOAuthOptions.SectionName).ValidateDataAnnotations()
            .Validate(x => Guid.TryParseExact(x.ClientId, "D", out var clientId) && clientId != Guid.Empty &&
                !x.ClientSecret.StartsWith("__"), "Set the Microsoft application (client) ID and client secret before starting the API.")
            .Validate(x => x.TenantId is "common" or "organizations" or "consumers" ||
                (Guid.TryParseExact(x.TenantId, "D", out var tenant) && tenant != Guid.Empty),
                "Microsoft TenantId must be common, organizations, consumers, or a tenant GUID.")
            .Validate(x => Uri.TryCreate(x.RedirectUri, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "https" || (uri.Scheme == "http" && uri.IsLoopback)) &&
                string.IsNullOrEmpty(uri.Fragment) && string.IsNullOrEmpty(uri.UserInfo),
                "Microsoft RedirectUri must use HTTPS (HTTP is allowed for loopback development).")
            .ValidateOnStart();
        services.AddOptions<SessionOptions>().BindConfiguration(SessionOptions.SectionName).ValidateDataAnnotations()
            .Validate(x => x.RefreshTokenLifetimeDays <= x.AbsoluteLifetimeDays,
                "The refresh token lifetime cannot exceed the absolute session lifetime.").ValidateOnStart();
        services.AddOptions<GoogleOAuthOptions>().BindConfiguration(GoogleOAuthOptions.SectionName).ValidateDataAnnotations()
            .Validate(x => !x.ClientId.StartsWith("__") && !x.ClientSecret.StartsWith("__"),
                "Set the Google client ID and client secret before starting the API.")
            .Validate(x => Uri.TryCreate(x.RedirectUri, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "https" || (uri.Scheme == "http" && uri.IsLoopback)) &&
                string.IsNullOrEmpty(uri.Fragment), "Google RedirectUri must use HTTPS (HTTP is allowed for loopback development).")
            .ValidateOnStart();
        services.AddOptions<FacebookOAuthOptions>().BindConfiguration(FacebookOAuthOptions.SectionName).ValidateDataAnnotations()
            .Validate(x => !x.AppSecret.StartsWith("__"), "Set the Facebook app ID and app secret before starting the API.")
            .Validate(x => Uri.TryCreate(x.RedirectUri, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "https" || (uri.Scheme == "http" && uri.IsLoopback)) &&
                string.IsNullOrEmpty(uri.Fragment) && string.IsNullOrEmpty(uri.UserInfo),
                "Facebook RedirectUri must use HTTPS (HTTP is allowed for loopback development).")
            .ValidateOnStart();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IAuthenticationPolicy, AuthenticationPolicy>();
        services.AddDataProtection().SetApplicationName("RealityLoop");
        services.AddMemoryCache();
        services.AddHttpClient("GoogleOAuth", client => client.Timeout = TimeSpan.FromSeconds(15))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            .RemoveAllLoggers();
        services.AddSingleton<GoogleSigningKeys>();
        services.AddScoped<IGoogleOAuthClient, GoogleOAuthClient>();
        services.AddHttpClient("MicrosoftOAuth", client => client.Timeout = TimeSpan.FromSeconds(15))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            .RemoveAllLoggers();
        services.AddSingleton<MicrosoftOpenIdConfiguration>();
        services.AddScoped<IMicrosoftOAuthClient, MicrosoftOAuthClient>();
        services.AddHttpClient("FacebookOAuth", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.MaxResponseContentBufferSize = 64 * 1024;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false })
            .RemoveAllLoggers();
        services.AddScoped<IFacebookOAuthClient, FacebookOAuthClient>();
        services.AddOptions<AiKeyOptions>().BindConfiguration(AiKeyOptions.SectionName)
            .Validate(x => x.IsValid(), "AI key protection requires a valid active key ID, a 32-byte Base64 encryption key ring, and a separate 32-byte Base64 fingerprint key.")
            .ValidateOnStart();
        services.AddOptions<AiProviderOptions>().BindConfiguration(AiProviderOptions.SectionName)
            .Validate(x => new[] { x.OpenAiFallbackModels, x.AnthropicFallbackModels, x.GoogleFallbackModels }
                .All(models => models is not null && models.Length <= 1000 && models.All(m =>
                    !string.IsNullOrWhiteSpace(m) && m.Length <= 200 && !m.Any(char.IsWhiteSpace) && !m.Any(char.IsControl))),
                "AI fallback model IDs must be non-empty identifiers of at most 200 characters.").ValidateOnStart();
        services.AddSingleton<IAiKeyProtector, AiKeyProtector>();
        services.AddScoped<IAiConnectionRepository, AiConnectionRepository>();
        services.AddSingleton<AiHttpTransport>();
        services.AddScoped<IAiProviderClient, OpenAiProviderClient>();
        services.AddScoped<IAiProviderClient, AnthropicProviderClient>();
        services.AddScoped<IAiProviderClient, GoogleAiProviderClient>();
        services.AddHttpClient("AiProviders", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(20);
                client.MaxResponseContentBufferSize = 1024 * 1024;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AllowAutoRedirect = false, UseCookies = false, ConnectTimeout = TimeSpan.FromSeconds(5),
                PooledConnectionLifetime = TimeSpan.FromMinutes(5), MaxConnectionsPerServer = 32
            }).RemoveAllLoggers();
        services.AddHostedService<AiReceiptCleanupService>();
        return services;
    }
}
