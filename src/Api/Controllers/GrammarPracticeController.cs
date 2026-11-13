using Api.Contracts.GrammarPractice;
using Application.GrammarPractice.Commands.AbandonGrammarPracticeSession;
using Application.GrammarPractice.Commands.AnswerGrammarPracticeQuestion;
using Application.GrammarPractice.Commands.CreateGrammarPracticeSession;
using Application.GrammarPractice.Models;
using Application.GrammarPractice.Queries.GetGrammarPracticeOptions;
using Application.GrammarPractice.Queries.GetGrammarPracticeSession;
using Application.GrammarPractice.Queries.GetGrammarPracticeSessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles/{profileId:long:min(1)}/grammar-practice")]
[RequestSizeLimit(4096)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class GrammarPracticeController : ApiControllerBase
{
    [HttpGet("options")]
    public async Task<ActionResult<GrammarPracticeOptionsDto>> Options(long profileId,
        [FromServices] GetGrammarPracticeOptionsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetGrammarPracticeOptionsQuery(profileId), ct));

    [HttpPost("sessions")]
    [ProducesResponseType<GrammarPracticeSessionDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<GrammarPracticeSessionDto>> Create(long profileId,
        CreateGrammarPracticeSessionRequest request, [FromServices] CreateGrammarPracticeSessionCommandHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new CreateGrammarPracticeSessionCommand(profileId, request.GrammarPointCode,
            request.QuestionCount), ct);
        return CreatedAtAction(nameof(GetById), new { profileId, sessionId = result.Session.Id }, result);
    }

    [HttpGet("sessions")]
    public async Task<ActionResult<GrammarPracticeSessionPage>> List(long profileId,
        [FromQuery] GetGrammarPracticeSessionsRequest request, [FromServices] GetGrammarPracticeSessionsQueryHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new GetGrammarPracticeSessionsQuery(profileId,
            request.GrammarPointCode, request.Status, request.PageSize, request.Before), ct));

    [HttpGet("sessions/{sessionId:long:min(1)}")]
    public async Task<ActionResult<GrammarPracticeSessionDto>> GetById(long profileId, long sessionId,
        [FromServices] GetGrammarPracticeSessionQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetGrammarPracticeSessionQuery(profileId, sessionId), ct));

    [HttpPost("sessions/{sessionId:long:min(1)}/items/{sessionItemId:long:min(1)}/answer")]
    public async Task<ActionResult<GrammarPracticeSessionDto>> Answer(long profileId, long sessionId, long sessionItemId,
        AnswerGrammarPracticeQuestionRequest request, [FromServices] AnswerGrammarPracticeQuestionCommandHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new AnswerGrammarPracticeQuestionCommand(profileId, sessionId,
            sessionItemId, request.SelectedOptionIndex, request.Version), ct));

    [HttpPost("sessions/{sessionId:long:min(1)}/abandon")]
    public async Task<ActionResult<GrammarPracticeSessionDto>> Abandon(long profileId, long sessionId,
        AbandonGrammarPracticeSessionRequest request, [FromServices] AbandonGrammarPracticeSessionCommandHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new AbandonGrammarPracticeSessionCommand(profileId, sessionId,
            request.Version), ct));
}
