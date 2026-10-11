using System.ComponentModel.DataAnnotations;
using Application.Authentication.Commands.GoogleLogin;
using Application.Authentication.Commands.FacebookLogin;
using Application.Authentication.Queries.GetFacebookAuthorizationUrl;
using Application.Authentication.Commands.MicrosoftLogin;
using Application.Authentication.Queries.GetMicrosoftAuthorizationUrl;
using Application.Authentication.Commands.RefreshToken;
using Application.Authentication.Commands.RevokeRefreshToken;
using Application.Authentication.Commands.Logout;
using Application.Authentication.Commands.LogoutSessions;
using Application.Authentication.Commands.LogoutAllSessions;
using Application.Authentication.Commands.LogoutOtherSessions;
using Application.Authentication.Queries.GetSessions;
using Application.Authentication.Models;
using Application.Authentication.Queries.GetCurrentUser;
using Application.Authentication.Queries.GetGoogleAuthorizationUrl;
using Application.Common.Exceptions;
using Api.Contracts.Authentication;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("auth")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("authentication")]
public sealed class AuthController : ApiControllerBase
{
    // Direct browser navigation starts the login and redirects to Google.
    [AllowAnonymous]
    [HttpGet("google")]
    public IActionResult Google([FromServices] GetGoogleAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Redirect(login.AuthorizationUrl);
    }

    // SPA clients may fetch the URL using credentials: 'include', then navigate to it.
    [AllowAnonymous]
    [HttpGet("google/authorize")]
    public IActionResult GoogleAuthorization([FromServices] GetGoogleAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Ok(new { login.AuthorizationUrl });
    }

    [AllowAnonymous]
    [HttpGet("google/callback")]
    public async Task<ActionResult<AuthenticationResult>> GoogleCallback(
        [FromServices] GoogleLoginCommandHandler handler, CancellationToken ct,
        [FromQuery, StringLength(4096)] string? code = null,
        [FromQuery, StringLength(128)] string? state = null,
        [FromQuery, StringLength(256)] string? error = null)
    {
        // Consume correlation even if the user declined consent.
        var protectedState = ConsumeState("google", state);
        if (!string.IsNullOrEmpty(error)) throw new AuthenticationException("Google login was cancelled or denied.");
        var result = await handler.Handle(new GoogleLoginCommand(code ?? "", state!, protectedState, GetDevice(null)), ct);
        return Ok(result);
    }

    // Use this when RedirectUri points to a frontend callback page.
    [AllowAnonymous]
    [HttpPost("google/callback")]
    public async Task<ActionResult<AuthenticationResult>> GoogleLogin(GoogleLoginRequest request,
        [FromServices] GoogleLoginCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GoogleLoginCommand(request.Code, request.State,
            ConsumeState("google", request.State), GetDevice(request.Device)), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("microsoft")]
    public IActionResult Microsoft([FromServices] GetMicrosoftAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Redirect(login.AuthorizationUrl);
    }

    [AllowAnonymous]
    [HttpGet("microsoft/authorize")]
    public IActionResult MicrosoftAuthorization([FromServices] GetMicrosoftAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Ok(new { login.AuthorizationUrl });
    }

    [AllowAnonymous]
    [HttpGet("microsoft/callback")]
    public async Task<ActionResult<AuthenticationResult>> MicrosoftCallback(
        [FromServices] MicrosoftLoginCommandHandler handler, CancellationToken ct,
        [FromQuery, StringLength(4096)] string? code = null,
        [FromQuery, StringLength(128)] string? state = null,
        [FromQuery, StringLength(256)] string? error = null)
    {
        var protectedState = ConsumeState("microsoft", state);
        if (!string.IsNullOrEmpty(error)) throw new AuthenticationException("Microsoft login was cancelled or denied.");
        var result = await handler.Handle(new MicrosoftLoginCommand(code ?? "", state!, protectedState, GetDevice(null)), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("microsoft/callback")]
    public async Task<ActionResult<AuthenticationResult>> MicrosoftLogin(MicrosoftLoginRequest request,
        [FromServices] MicrosoftLoginCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new MicrosoftLoginCommand(request.Code, request.State,
            ConsumeState("microsoft", request.State), GetDevice(request.Device)), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("facebook")]
    public IActionResult Facebook([FromServices] GetFacebookAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Redirect(login.AuthorizationUrl);
    }

    [AllowAnonymous]
    [HttpGet("facebook/authorize")]
    public IActionResult FacebookAuthorization([FromServices] GetFacebookAuthorizationUrlQueryHandler handler)
    {
        var login = StartLogin(handler);
        return Ok(new { login.AuthorizationUrl });
    }

    [AllowAnonymous]
    [HttpGet("facebook/callback")]
    public async Task<ActionResult<AuthenticationResult>> FacebookCallback(
        [FromServices] FacebookLoginCommandHandler handler, CancellationToken ct,
        [FromQuery, StringLength(4096)] string? code = null,
        [FromQuery, StringLength(128)] string? state = null,
        [FromQuery, StringLength(256)] string? error = null)
    {
        var protectedState = ConsumeState("facebook", state);
        if (!string.IsNullOrEmpty(error)) throw new AuthenticationException("Facebook login was cancelled or denied.");
        var result = await handler.Handle(new FacebookLoginCommand(code ?? "", state!, protectedState, GetDevice(null)), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("facebook/callback")]
    public async Task<ActionResult<AuthenticationResult>> FacebookLogin(FacebookLoginRequest request,
        [FromServices] FacebookLoginCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new FacebookLoginCommand(request.Code, request.State,
            ConsumeState("facebook", request.State), GetDevice(request.Device)), ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticationResult>> Refresh(RefreshTokenRequest request,
        [FromServices] RefreshTokenCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new RefreshTokenCommand(request.RefreshToken), ct));

    // Possession of a refresh token authorizes revoking its session, even when the access token expired.
    [AllowAnonymous]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RefreshTokenRequest request,
        [FromServices] RevokeRefreshTokenCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new RevokeRefreshTokenCommand(request.RefreshToken), ct);
        return NoContent();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] LogoutCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new LogoutCommand(), ct);
        return NoContent();
    }

    [HttpPost("logout/sessions")]
    public async Task<IActionResult> LogoutSessions(LogoutSessionsRequest request,
        [FromServices] LogoutSessionsCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new LogoutSessionsCommand(request.SessionIds), ct);
        return NoContent();
    }

    [HttpPost("logout/others")]
    public async Task<IActionResult> LogoutOthers([FromServices] LogoutOtherSessionsCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new LogoutOtherSessionsCommand(), ct);
        return NoContent();
    }

    [HttpPost("logout/all")]
    public async Task<IActionResult> LogoutAll([FromServices] LogoutAllSessionsCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new LogoutAllSessionsCommand(), ct);
        return NoContent();
    }

    [HttpGet("sessions")]
    public async Task<ActionResult<SessionPage>> Sessions([FromQuery] GetSessionsRequest request,
        [FromServices] GetSessionsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetSessionsQuery(request.PageSize, request.Before), ct));

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me([FromServices] GetCurrentUserQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetCurrentUserQuery(), ct));

    private GoogleAuthorization StartLogin(GetGoogleAuthorizationUrlQueryHandler handler)
    {
        var login = handler.Handle(new GetGoogleAuthorizationUrlQuery());
        WriteState("google", login.State, login.ProtectedState);
        return login;
    }

    private MicrosoftAuthorization StartLogin(GetMicrosoftAuthorizationUrlQueryHandler handler)
    {
        var login = handler.Handle(new GetMicrosoftAuthorizationUrlQuery());
        WriteState("microsoft", login.State, login.ProtectedState);
        return login;
    }

    private FacebookAuthorization StartLogin(GetFacebookAuthorizationUrlQueryHandler handler)
    {
        var login = handler.Handle(new GetFacebookAuthorizationUrlQuery());
        WriteState("facebook", login.State, login.ProtectedState);
        return login;
    }

    private void WriteState(string provider, string state, string protectedState) =>
        Response.Cookies.Append($"rl.{provider}." + state, protectedState, CookieOptions(provider));

    private string ConsumeState(string provider, string? state)
    {
        if (state is null || state.Length != 43 || state.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_'))
            throw new AuthenticationException($"Invalid {provider} login state.");
        var cookieName = $"rl.{provider}." + state;
        var value = Request.Cookies[cookieName];
        Response.Cookies.Delete(cookieName, CookieOptions(provider));
        return value ?? throw new AuthenticationException($"{provider} login cookie is missing. Start a new login in this browser.");
    }

    private CookieOptions CookieOptions(string provider) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
        Path = $"/api/v1/auth/{provider}",
        MaxAge = TimeSpan.FromMinutes(10),
        IsEssential = true
    };

    private LoginDevice GetDevice(LoginDeviceRequest? device)
    {
        var browser = device?.Browser ?? Request.Headers.UserAgent.ToString();
        if (browser.Length > 255) browser = browser[..255];
        return new LoginDevice(device?.DeviceType ?? SessionDeviceType.Unknown, device?.DeviceName,
            device?.Os, browser, HttpContext.Connection.RemoteIpAddress);
    }
}
