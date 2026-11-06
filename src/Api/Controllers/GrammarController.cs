using Api.Contracts.Grammar;
using Application.Grammar.Models;
using Application.Grammar.Queries.GetGrammarPoint;
using Application.Grammar.Queries.GetGrammarPoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("languages/{languageCode}/grammar-points")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-content")]
public sealed class GrammarController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GrammarPointPage>> List(string languageCode,
        [FromQuery] GetGrammarPointsRequest request,
        [FromServices] GetGrammarPointsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetGrammarPointsQuery(languageCode, request.Locale,
            request.PageSize, request.After), ct));

    [HttpGet("{pointCode}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<GrammarPointDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GrammarPointDto>> GetByCode(string languageCode, string pointCode,
        [FromQuery] GetGrammarPointRequest request,
        [FromServices] GetGrammarPointQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetGrammarPointQuery(languageCode, pointCode, request.Locale), ct));
}
