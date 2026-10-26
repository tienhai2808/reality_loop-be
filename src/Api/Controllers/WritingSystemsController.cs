using Api.Contracts.WritingSystems;
using Application.WritingSystems.Models;
using Application.WritingSystems.Queries.GetWritingSystem;
using Application.WritingSystems.Queries.GetWritingSystemItems;
using Application.WritingSystems.Queries.GetWritingSystemItemReadings;
using Application.WritingSystems.Queries.GetWritingSystems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("languages/{languageCode}/writing-systems")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-content")]
public sealed class WritingSystemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WritingSystemPage>> List(string languageCode, [FromQuery] GetWritingSystemsRequest request,
        [FromServices] GetWritingSystemsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemsQuery(languageCode, request.Locale, request.PageSize, request.After), ct));

    [HttpGet("{systemCode}")]
    public async Task<ActionResult<WritingSystemDto>> GetByCode(string languageCode, string systemCode,
        [FromQuery] CatalogLocaleRequest request, [FromServices] GetWritingSystemQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemQuery(languageCode, systemCode, request.Locale), ct));

    [HttpGet("{systemCode}/items")]
    public async Task<ActionResult<WritingSystemItemPage>> Items(string languageCode, string systemCode,
        [FromQuery] GetWritingSystemItemsRequest request, [FromServices] GetWritingSystemItemsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemItemsQuery(languageCode, systemCode, request.PageSize, request.After), ct));

    [HttpGet("{systemCode}/items/{itemCode}/readings")]
    public async Task<ActionResult<WritingSystemItemReadingPage>> Readings(string languageCode, string systemCode,
        string itemCode, [FromQuery] GetWritingSystemItemReadingsRequest request,
        [FromServices] GetWritingSystemItemReadingsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetWritingSystemItemReadingsQuery(languageCode, systemCode, itemCode, request.PageSize, request.After), ct));
}
