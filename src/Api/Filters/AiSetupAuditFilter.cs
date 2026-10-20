using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

internal sealed class AiSetupAuditFilter(ILogger<AiSetupAuditFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (HttpMethods.IsGet(context.HttpContext.Request.Method)) { await next(); return; }
        var started = Stopwatch.GetTimestamp();
        var result = await next();
        logger.LogInformation("AI setup {Operation} user {UserId} connection {ConnectionId} outcome {Outcome} elapsed {ElapsedMs}ms trace {TraceId}",
            context.ActionDescriptor.DisplayName, context.HttpContext.User.FindFirst("sub")?.Value,
            context.RouteData.Values.GetValueOrDefault("id"), result.Exception is null ? "Completed" : "Failed",
            Stopwatch.GetElapsedTime(started).TotalMilliseconds, context.HttpContext.TraceIdentifier);
    }
}
