using Api.Contracts.VocabularyLearning;
using Application.VocabularyLearning.Commands.AddVocabularyLearning;
using Application.VocabularyLearning.Commands.RecordVocabularyReview;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Queries.GetVocabularyLearningCard;
using Application.VocabularyLearning.Queries.GetVocabularyLearningPage;
using Application.VocabularyLearning.Queries.GetVocabularyLearningSummary;
using Application.VocabularyLearning.Queries.GetVocabularyReviewHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles/{profileId:long:min(1)}")]
[RequestSizeLimit(4096)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class VocabularyLearningController : ApiControllerBase
{
    [HttpPut("vocabulary-senses/{senseId:long:min(1)}/learning-progress")]
    [ProducesResponseType<VocabularyProgressDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<VocabularyProgressDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<VocabularyProgressDto>> Add(long profileId, long senseId,
        [FromServices] AddVocabularyLearningCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new(profileId, senseId), ct);
        return result.Created ? CreatedAtAction(nameof(Card), new { profileId, progressId = result.Progress.Id }, result.Progress) : Ok(result.Progress);
    }

    [HttpGet("vocabulary-learning")]
    public async Task<ActionResult<VocabularyLearningPage>> List(long profileId, [FromQuery] GetVocabularyLearningPageRequest request,
        [FromServices] GetVocabularyLearningPageQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, request.DueOnly, request.Status, request.PageSize, request.After, request.Locale), ct));

    [HttpGet("vocabulary-learning/summary")]
    public async Task<ActionResult<VocabularyLearningSummaryDto>> Summary(long profileId,
        [FromServices] GetVocabularyLearningSummaryQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId), ct));

    [HttpGet("vocabulary-learning/{progressId:long:min(1)}")]
    public async Task<ActionResult<VocabularyFlashcardDto>> Card(long profileId, long progressId,
        [FromQuery] GetVocabularyLearningCardRequest request, [FromServices] GetVocabularyLearningCardQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, progressId, request.Locale), ct));

    [HttpPost("vocabulary-learning/{progressId:long:min(1)}/reviews")]
    public async Task<ActionResult<VocabularyReviewDto>> Review(long profileId, long progressId, RecordVocabularyReviewRequest request,
        [FromServices] RecordVocabularyReviewCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, progressId, request.RequestId, request.Version, request.Rating), ct));

    [HttpGet("vocabulary-learning/{progressId:long:min(1)}/reviews")]
    public async Task<ActionResult<VocabularyReviewPage>> History(long profileId, long progressId,
        [FromQuery] GetVocabularyReviewHistoryRequest request, [FromServices] GetVocabularyReviewHistoryQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new(profileId, progressId, request.PageSize, request.Before), ct));
}
