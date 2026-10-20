namespace Api;

public static class WebApplicationExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        if (app.Configuration.GetSection("ReverseProxy:KnownProxies").Get<string[]>() is { Length: > 0 })
            app.UseForwardedHeaders();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("Frontend");
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/v1/auth") || context.Request.Path.StartsWithSegments("/api/v1/ai"))
            {
                context.Response.Headers.CacheControl = "no-store";
                context.Response.Headers.Pragma = "no-cache";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
            }
            await next(context);
        });
        // Current limiters partition by IP; reject excess traffic before JWT validation or DB access.
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<Api.Common.RateLimiting.AiSetupRateLimitMiddleware>();
        if (app.Environment.IsDevelopment()) app.MapOpenApi("/api/v1/openapi/{documentName}.json").AllowAnonymous();
        app.MapControllers();
        return app;
    }
}
