using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Api.Common.Routing;
using Api.Common.Security;
using Api.Infrastructure;
using Api.Services;
using Application.Common.Interfaces;
using Infrastructure.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options => options.Conventions.Add(new ApiRoutePrefixConvention()))
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddOpenApi();
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(provider => provider.GetRequiredService<CurrentUser>());
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);

        services.AddScoped<SessionJwtBearerEvents>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, configured) =>
            {
                var jwt = configured.Value;
                bearer.MapInboundClaims = false;
                bearer.IncludeErrorDetails = false;
                bearer.EventsType = typeof(SessionJwtBearerEvents);
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub"
                };
            });
        services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser().Build());

        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        if (origins.Length == 0 || origins.Any(origin => !Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "https" && uri.Scheme != "http") || origin.Contains('*') ||
            !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment) || uri.AbsolutePath != "/" || origin.EndsWith('/')))
            throw new InvalidOperationException("Cors:AllowedOrigins must contain explicit origins without trailing slashes or wildcards.");
        services.AddCors(options => options.AddPolicy("Frontend", policy => policy.WithOrigins(origins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders("Authorization", "Content-Type")
            .AllowCredentials().SetPreflightMaxAge(TimeSpan.FromMinutes(10))));

        var permitLimit = configuration.GetValue("RateLimiting:Authentication:PermitLimit", 30);
        var windowSeconds = configuration.GetValue("RateLimiting:Authentication:WindowSeconds", 60);
        if (permitLimit is < 1 or > 10000 || windowSeconds is < 1 or > 3600)
            throw new InvalidOperationException("Authentication rate limit settings are invalid.");
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit, Window = TimeSpan.FromSeconds(windowSeconds), QueueLimit = 0, AutoReplenishment = true
                }));
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.Headers.RetryAfter = windowSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
                await Results.Problem(statusCode: 429, title: "Too many requests",
                    detail: "Please retry after the rate limit window resets.").ExecuteAsync(context.HttpContext);
            };
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
            foreach (var proxy in configuration.GetSection("ReverseProxy:KnownProxies").Get<string[]>() ?? [])
                options.KnownProxies.Add(IPAddress.Parse(proxy));
        });
        return services;
    }
}
