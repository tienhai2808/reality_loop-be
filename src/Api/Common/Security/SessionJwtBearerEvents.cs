using System.Globalization;
using Application.Common.Interfaces;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Common.Security;

internal sealed class SessionJwtBearerEvents(ISessionRepository sessions, CurrentUser currentUser, TimeProvider clock) : JwtBearerEvents
{
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var subject = context.Principal?.FindFirst("sub")?.Value;
        var sid = context.Principal?.FindFirst("sid")?.Value;
        if (!long.TryParse(subject, NumberStyles.None, CultureInfo.InvariantCulture, out var userId) || userId <= 0 ||
            !long.TryParse(sid, NumberStyles.None, CultureInfo.InvariantCulture, out var sessionId) || sessionId <= 0)
        {
            context.Fail("Invalid session claims.");
            return;
        }
        var user = await sessions.GetAuthenticatedUserAsync(sessionId, userId, clock.GetUtcNow(), context.HttpContext.RequestAborted);
        if (user is null) context.Fail("Session is not active.");
        else currentUser.SetValidatedUser(user);
    }

    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.Headers.WWWAuthenticate = "Bearer";
        await Results.Problem(statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized", detail: "A valid access token and active session are required.")
            .ExecuteAsync(context.HttpContext);
    }

    public override Task Forbidden(ForbiddenContext context) =>
        Results.Problem(statusCode: StatusCodes.Status403Forbidden, title: "Forbidden",
            detail: "You do not have permission to perform this action.").ExecuteAsync(context.HttpContext);
}
