using Api;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Handwriting.Tests;

internal static class TestApiHost
{
    public static WebApplication Build(string database, int permitLimit = 10000)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development", Args = ["--urls", "http://127.0.0.1:0"]
        });
        builder.Configuration.AddJsonFile(Path.GetFullPath("../../../../../src/Api/appsettings.json", AppContext.BaseDirectory))
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database, ["SnowflakeId:GeneratorId"] = "1021",
                ["Authentication:Google:ClientId"] = "test.apps.googleusercontent.com",
                ["Authentication:Google:ClientSecret"] = "test-only",
                ["Authentication:Microsoft:ClientId"] = "11111111-1111-1111-1111-111111111111",
                ["Authentication:Microsoft:ClientSecret"] = "test-only",
                ["Authentication:Facebook:AppId"] = "123456789", ["Authentication:Facebook:AppSecret"] = "test-only",
                ["Authentication:Jwt:SigningKey"] = "local-handwriting-test-only-signing-key-at-least-32-bytes",
                ["AI:KeyProtection:Keys:primary"] = Convert.ToBase64String(new byte[32]),
                ["AI:KeyProtection:FingerprintKey"] = Convert.ToBase64String(Enumerable.Repeat((byte)1, 32).ToArray()),
                ["Storage:R2:ServiceUrl"] = "https://11111111111111111111111111111111.r2.cloudflarestorage.com",
                ["Storage:R2:BucketName"] = "handwriting-tests", ["Storage:R2:AccessKeyId"] = "test-only",
                ["Storage:R2:SecretAccessKey"] = "test-only", ["RateLimiting:LearningProfiles:PermitLimit"] = "10000",
                ["RateLimiting:Handwriting:PermitLimit"] = permitLimit.ToString()
            });
        builder.Logging.ClearProviders().AddSimpleConsole().SetMinimumLevel(LogLevel.Warning)
            .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None).AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApi(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(Api.Controllers.HandwritingController).Assembly);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "HandwritingTests";
            options.DefaultChallengeScheme = "HandwritingTests";
        }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("HandwritingTests", _ => { });
        builder.Services.AddSingleton<TimeProvider, TestClock>();
        builder.Services.AddSingleton<SqlCommandCounter>();
        builder.Services.AddDbContext<ApplicationDbContext>((services, options) => options.AddInterceptors(services.GetRequiredService<SqlCommandCounter>()));
        return builder.Build().UseApi();
    }

    public static HttpClient Client(WebApplication app) => new()
    {
        BaseAddress = new Uri(app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single())
    };
}
