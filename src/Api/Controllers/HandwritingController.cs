using Api.Contracts.Handwriting;
using Application.Handwriting.Commands.CreateHandwritingAttempt;
using Application.Handwriting.Models;
using Application.Handwriting.Queries.GetHandwritingAttempt;
using Application.Handwriting.Queries.GetHandwritingAttempts;
using Application.Handwriting.Queries.GetHandwritingTemplate;
using Application.Handwriting.Queries.GetHandwritingTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles/{profileId:long:min(1)}")]
[RequestSizeLimit(262144)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class HandwritingController : ApiControllerBase
{
    [HttpGet("writing-systems/{systemCode}/handwriting-templates")]
    public async Task<ActionResult<HandwritingTemplatePage>> Templates(long profileId, string systemCode,
        [FromQuery] GetHandwritingTemplatesRequest request, [FromServices] GetHandwritingTemplatesQueryHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new(profileId, systemCode, request.PageSize, request.After), ct));

    [HttpGet("writing-systems/{systemCode}/handwriting-templates/{templateId:long:min(1)}")]
    public async Task<ActionResult<HandwritingTemplateDto>> Template(long profileId, string systemCode, long templateId,
        [FromServices] GetHandwritingTemplateQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, systemCode, templateId), ct));

    [HttpPost("writing-systems/{systemCode}/handwriting-attempts")]
    [EnableRateLimiting("handwriting")]
    [ProducesResponseType<HandwritingAttemptDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<HandwritingAttemptDto>> Create(long profileId, string systemCode,
        CreateHandwritingAttemptRequest request, [FromServices] CreateHandwritingAttemptCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new(profileId, systemCode, request.ToInput()), ct);
        return CreatedAtAction(nameof(GetById), new { profileId, attemptId = result.Attempt.Id }, result);
    }

    [HttpGet("handwriting-attempts")]
    public async Task<ActionResult<HandwritingAttemptPage>> List(long profileId, [FromQuery] GetHandwritingAttemptsRequest request,
        [FromServices] GetHandwritingAttemptsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, request.SystemCode, request.Passed, request.PageSize, request.Before), ct));

    [HttpGet("handwriting-attempts/{attemptId:long:min(1)}")]
    public async Task<ActionResult<HandwritingAttemptDto>> GetById(long profileId, long attemptId,
        [FromServices] GetHandwritingAttemptQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, attemptId), ct));
}
