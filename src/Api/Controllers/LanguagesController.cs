using Api.Contracts.Languages;
using Application.Languages.Models;
using Application.Languages.Queries.GetLanguage;
using Application.Languages.Queries.GetLanguages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("languages")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-content")]
public sealed class LanguagesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<LanguagePage>> List([FromQuery] GetLanguagesRequest request,
        [FromServices] GetLanguagesQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetLanguagesQuery(request.Locale, request.PageSize, request.After), ct));

    [HttpGet("{languageCode}")]
    public async Task<ActionResult<LanguageDto>> GetByCode(string languageCode, [FromQuery] CatalogLocaleRequest request,
        [FromServices] GetLanguageQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetLanguageQuery(languageCode, request.Locale), ct));
}
