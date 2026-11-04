using Api.Contracts.Vocabulary;
using Application.Vocabulary.Models;
using Application.Vocabulary.Queries.GetVocabularyEntries;
using Application.Vocabulary.Queries.GetVocabularyEntry;
using Application.Vocabulary.Queries.GetVocabularySenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("languages/{languageCode}/vocabulary")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-content")]
public sealed class VocabularyController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<VocabularyEntryPage>> List(string languageCode,
        [FromQuery] GetVocabularyEntriesRequest request, [FromServices] GetVocabularyEntriesQueryHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new GetVocabularyEntriesQuery(languageCode, request.Locale,
            request.PartOfSpeech, request.PageSize, request.After), ct));

    [HttpGet("{entryCode}")]
    [ProducesResponseType<VocabularyEntryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VocabularyEntryDto>> GetByCode(string languageCode, string entryCode,
        [FromServices] GetVocabularyEntryQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetVocabularyEntryQuery(languageCode, entryCode), ct));

    [HttpGet("{entryCode}/senses")]
    public async Task<ActionResult<VocabularySensePage>> Senses(string languageCode, string entryCode,
        [FromQuery] GetVocabularySensesRequest request, [FromServices] GetVocabularySensesQueryHandler handler,
        CancellationToken ct) => Ok(await handler.Handle(new GetVocabularySensesQuery(languageCode, entryCode,
            request.Locale, request.PageSize, request.After), ct));
}
