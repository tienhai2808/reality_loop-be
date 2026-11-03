using Api.Contracts.WritingSystemPractice;
using Application.WritingSystemPractice.Commands.AbandonWritingSystemPracticeSession;
using Application.WritingSystemPractice.Commands.AnswerWritingSystemPracticeQuestion;
using Application.WritingSystemPractice.Commands.CreateWritingSystemPracticeSession;
using Application.WritingSystemPractice.Models;
using Application.WritingSystemPractice.Queries.GetWritingSystemPracticeSession;
using Application.WritingSystemPractice.Queries.GetWritingSystemPracticeSessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles/{profileId:long:min(1)}")]
[RequestSizeLimit(4096)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class WritingSystemPracticeController : ApiControllerBase
{
    [HttpPost("writing-systems/{systemCode}/practice-sessions")]
    [ProducesResponseType<WritingSystemPracticeSessionDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<WritingSystemPracticeSessionDto>> Create(long profileId, string systemCode,
        CreateWritingSystemPracticeSessionRequest request, [FromServices] CreateWritingSystemPracticeSessionCommandHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new CreateWritingSystemPracticeSessionCommand(profileId, systemCode, request.QuestionCount), ct);
        return CreatedAtAction(nameof(GetById), new { profileId, sessionId = result.Session.Id }, result);
    }

    [HttpGet("practice-sessions")]
    public async Task<ActionResult<WritingSystemPracticeSessionPage>> List(long profileId,
        [FromQuery] GetWritingSystemPracticeSessionsRequest request, [FromServices] GetWritingSystemPracticeSessionsQueryHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new GetWritingSystemPracticeSessionsQuery(profileId, request.SystemCode,
            request.Status, request.PageSize, request.Before), ct));

    [HttpGet("practice-sessions/{sessionId:long:min(1)}")]
    public async Task<ActionResult<WritingSystemPracticeSessionDto>> GetById(long profileId, long sessionId,
        [FromServices] GetWritingSystemPracticeSessionQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemPracticeSessionQuery(profileId, sessionId), ct));

    [HttpPost("practice-sessions/{sessionId:long:min(1)}/items/{sessionItemId:long:min(1)}/answer")]
    public async Task<ActionResult<WritingSystemPracticeSessionDto>> Answer(long profileId, long sessionId, long sessionItemId,
        AnswerWritingSystemPracticeQuestionRequest request, [FromServices] AnswerWritingSystemPracticeQuestionCommandHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new AnswerWritingSystemPracticeQuestionCommand(profileId, sessionId,
            sessionItemId, request.SelectedOptionIndex, request.Version), ct));

    [HttpPost("practice-sessions/{sessionId:long:min(1)}/abandon")]
    public async Task<ActionResult<WritingSystemPracticeSessionDto>> Abandon(long profileId, long sessionId,
        AbandonWritingSystemPracticeSessionRequest request, [FromServices] AbandonWritingSystemPracticeSessionCommandHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new AbandonWritingSystemPracticeSessionCommand(profileId, sessionId,
            request.Version), ct));
}
