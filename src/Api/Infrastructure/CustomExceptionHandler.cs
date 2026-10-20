using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Domain.AI;

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
            AiCreationInProgressException => (409, "AI connection creation in progress"),
            ExternalServiceException => (502, "External service unavailable"),
            AiProviderException ex when ex.IsTimeout => (504, "AI provider timed out"),
            AiProviderException ex when ex.Code is AiFailureCode.InvalidApiKey or AiFailureCode.ModelUnavailable or AiFailureCode.QuotaExceeded
                => (422, "AI connection cannot be used"),
            AiProviderException => (502, "AI provider unavailable"),
            _ => (500, "Unexpected error")
        };
        if (status == 500) logger.LogError(exception, "Unhandled request error. TraceId: {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = status;
        context.Response.Headers.CacheControl = "no-store";
        if (exception is AiCreationInProgressException) context.Response.Headers.RetryAfter = "2";
        if (exception is AiProviderException { RetryAfterAt: { } retry })
            context.Response.Headers.RetryAfter = Math.Max(1, (int)Math.Ceiling((retry - DateTimeOffset.UtcNow).TotalSeconds)).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (status == 401) context.Response.Headers.WWWAuthenticate = "Bearer";
        var detail = status == 500 ? "An unexpected error occurred. Use the trace ID when contacting support."
            : exception.Message;
        var problem = new ProblemDetails
        {
            Status = status, Title = title, Detail = detail, Instance = context.Request.Path
        };
        if (exception is AiProviderException ai)
        {
            problem.Extensions["code"] = ai.Code.ToString();
            problem.Extensions["retryAfterAt"] = ai.RetryAfterAt;
        }
        if (exception is AiCreationInProgressException) problem.Extensions["code"] = "CreationInProgress";
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem
        });
    }
}
