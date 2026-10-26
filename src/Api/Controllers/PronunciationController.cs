using Api.Contracts.Pronunciation;
using Application.Pronunciation.Models;
using Application.Pronunciation.Queries.GetPronunciationExamples;
using Application.Pronunciation.Queries.GetPronunciationRule;
using Application.Pronunciation.Queries.GetPronunciationRules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("languages/{languageCode}/pronunciation-rules")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-content")]
public sealed class PronunciationController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PronunciationRulePage>> List(string languageCode,
        [FromQuery] GetPronunciationRulesRequest request,
        [FromServices] GetPronunciationRulesQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetPronunciationRulesQuery(languageCode, request.Locale,
            request.PageSize, request.After), ct));

    [HttpGet("{ruleCode}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PronunciationRuleDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PronunciationRuleDto>> GetByCode(string languageCode, string ruleCode,
        [FromQuery] CatalogLocaleRequest request,
        [FromServices] GetPronunciationRuleQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetPronunciationRuleQuery(languageCode, ruleCode, request.Locale), ct));

    [HttpGet("{ruleCode}/examples")]
    public async Task<ActionResult<PronunciationExamplePage>> Examples(string languageCode, string ruleCode,
        [FromQuery] GetPronunciationExamplesRequest request,
        [FromServices] GetPronunciationExamplesQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetPronunciationExamplesQuery(languageCode, ruleCode,
            request.PageSize, request.After), ct));
}
