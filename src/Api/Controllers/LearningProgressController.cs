using Api.Contracts.LearningProgress;
using Application.LearningProgress.Commands.RecordWritingSystemItemProgress;
using Application.LearningProgress.Models;
using Application.LearningProgress.Queries.GetWritingSystemItemProgress;
using Application.LearningProgress.Queries.GetWritingSystemItemProgressPage;
using Application.LearningProgress.Queries.GetWritingSystemProgressSummary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles/{profileId:long:min(1)}")]
[RequestSizeLimit(4096)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class LearningProgressController : ApiControllerBase
{
    [HttpGet("writing-system-progress")]
    public async Task<ActionResult<LearningProgressSummaryDto>> Summary(long profileId,
        [FromServices] GetWritingSystemProgressSummaryQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemProgressSummaryQuery(profileId), ct));

    [HttpGet("writing-systems/{systemCode}/progress")]
    public async Task<ActionResult<WritingSystemItemProgressPage>> List(long profileId, string systemCode,
        [FromQuery] GetWritingSystemItemProgressPageRequest request,
        [FromServices] GetWritingSystemItemProgressPageQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemItemProgressPageQuery(profileId, systemCode,
            request.Status, request.PageSize, request.After), ct));

    [HttpGet("writing-systems/{systemCode}/items/{itemId:long:min(1)}/progress")]
    public async Task<ActionResult<WritingSystemItemProgressDto>> GetByItem(long profileId, string systemCode, long itemId,
        [FromServices] GetWritingSystemItemProgressQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemItemProgressQuery(profileId, systemCode, itemId), ct));

    [HttpPost("writing-systems/{systemCode}/items/{itemId:long:min(1)}/progress")]
    public async Task<ActionResult<WritingSystemItemProgressDto>> Record(long profileId, string systemCode, long itemId,
        RecordWritingSystemItemProgressRequest request,
        [FromServices] RecordWritingSystemItemProgressCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new RecordWritingSystemItemProgressCommand(profileId, systemCode, itemId,
            request.Action, request.Version), ct));
}
