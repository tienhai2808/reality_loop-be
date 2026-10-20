using System.Threading.RateLimiting;

namespace Api.Common.RateLimiting;

internal sealed class AiSetupRateLimitMiddleware(RequestDelegate next, AiSetupLimiter limiter)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/v1/ai") || context.User.Identity?.IsAuthenticated != true)
        { await next(context); return; }
        using var rate = await limiter.Requests.AcquireAsync(context, 1, context.RequestAborted);
        if (!rate.IsAcquired) { await Reject(context, rate); return; }
        if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPatch(context.Request.Method))
        {
            using var concurrent = await limiter.Concurrency.AcquireAsync(context, 1, context.RequestAborted);
            if (!concurrent.IsAcquired) { await Reject(context, concurrent); return; }
            await next(context);
        }
        else await next(context);
    }
    private static Task Reject(HttpContext context, RateLimitLease lease)
    {
        var seconds = lease.TryGetMetadata(MetadataName.RetryAfter, out var delay) ? Math.Max(1, (int)Math.Ceiling(delay.TotalSeconds)) : 1;
        context.Response.Headers.RetryAfter = seconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return Results.Problem(statusCode: 429, title: "Too many AI setup requests",
            detail: "Wait before retrying AI setup.").ExecuteAsync(context);
    }
}
