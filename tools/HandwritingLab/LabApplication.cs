using System.Net;
using System.Text.Json.Serialization;
using HandwritingLab.Contracts;
using HandwritingLab.Models;
using HandwritingLab.Scoring;
using Microsoft.AspNetCore.Routing;

namespace HandwritingLab;

public static class LabApplication
{
    public const string ApiBase = "/api/v1/handwriting-lab";

    public static WebApplication Build(int port = 5387)
    {
        if (port is < 0 or > 65535) throw new ArgumentOutOfRangeException(nameof(port));
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(LabApplication).Assembly.GetName().Name,
            Args = [], EnvironmentName = "Production"
        });
        // This developer tool always binds loopback, independently of the main API's URLs/secrets/DB.
        builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
        builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 262144);
        builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
            options.SerializerOptions.MaxDepth = 12;
        });
        builder.Services.AddSingleton<TemplateCatalog>();
        builder.Services.AddSingleton<ScoringPolicy>();
        builder.Services.AddSingleton<HandwritingGrader>();
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; connect-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";
            var host = context.Request.Host.Host;
            var origin = context.Request.Headers.Origin.ToString();
            if ((host != "127.0.0.1" && host != "localhost") ||
                (context.Connection.RemoteIpAddress is { } address && !IPAddress.IsLoopback(address)) ||
                (origin.Length > 0 && origin != $"{context.Request.Scheme}://{context.Request.Host}"))
            {
                await Results.Problem(statusCode: 403, title: "Local tool only", detail: "Open the lab from its loopback URL.").ExecuteAsync(context);
                return;
            }
            try { await next(context); }
            catch (BadHttpRequestException ex) when (ex.StatusCode is 400 or 413 or 415)
            {
                await Results.Problem(statusCode: ex.StatusCode, title: "Invalid drawing", detail: ex.StatusCode switch
                {
                    413 => "Drawing payload exceeds 256 KiB.",
                    415 => "Use application/json for a drawing.",
                    _ => "Invalid JSON drawing payload."
                }).ExecuteAsync(context);
            }
            catch (ArgumentException ex)
            {
                await Results.Problem(statusCode: 400, title: "Invalid drawing", detail: ex.Message).ExecuteAsync(context);
            }
        });
        app.UseStatusCodePages(async status => await Results.Problem(statusCode: status.HttpContext.Response.StatusCode,
            title: "Lab request failed", detail: status.HttpContext.Response.StatusCode switch
            {
                413 => "Drawing payload exceeds 256 KiB.",
                415 => "Use application/json for a drawing.",
                404 => "This lab route was not found.",
                _ => "The lab request could not be completed."
            }).ExecuteAsync(status.HttpContext));
        app.MapGet("/", () => Asset("index.html", "text/html; charset=utf-8"));
        app.MapGet("/app.js", () => Asset("app.js", "text/javascript; charset=utf-8"));
        app.MapGet("/style.css", () => Asset("style.css", "text/css; charset=utf-8"));
        app.MapGet("/data-license", () => Resource("HandwritingLab.COPYING", "text/plain; charset=utf-8"));
        app.MapGet(ApiBase + "/templates", (TemplateCatalog catalog, ScoringPolicy policy) =>
            new CatalogResponse(policy, catalog.Dataset, catalog.Targets));
        app.MapPost(ApiBase + "/grade", (GradeRequest request, HandwritingGrader grader) => grader.Grade(request));
        return app;
    }

    private static IResult Asset(string name, string contentType) => Resource("HandwritingLab.Web." + name, contentType);

    private static IResult Resource(string name, string contentType)
    {
        using var stream = typeof(LabApplication).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException("Missing bundled lab resource.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return Results.Bytes(buffer.ToArray(), contentType);
    }
}
