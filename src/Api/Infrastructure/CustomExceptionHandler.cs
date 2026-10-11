using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Infrastructure;

internal sealed class CustomExceptionHandler(IProblemDetailsService problems, ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            RequestValidationException => (400, "Invalid request"),
            AuthenticationException => (401, "Unauthorized"),
            ForbiddenException => (403, "Forbidden"),
            NotFoundException => (404, "Not found"),
            ConflictException => (409, "Conflict"),
            ExternalServiceException => (502, "External service unavailable"),
            _ => (500, "Unexpected error")
        };
        if (status == 500) logger.LogError(exception, "Unhandled request error. TraceId: {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = status;
        if (status == 401) context.Response.Headers.WWWAuthenticate = "Bearer";
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? "An unexpected error occurred. Use the trace ID when contacting support." : exception.Message,
                Instance = context.Request.Path
            }
        });
    }
}
