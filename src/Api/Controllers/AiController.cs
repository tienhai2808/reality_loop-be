using System.ComponentModel.DataAnnotations;
using Api.Contracts.AI;
using Api.Filters;
using Application.AI.Commands.CreateAiConnection;
using Application.AI.Commands.DeleteAiConnection;
using Application.AI.Commands.UpdateAiConnection;
using Application.AI.Commands.VerifyAiConnection;
using Application.AI.Models;
using Application.AI.Queries.GetAiConnections;
using Application.AI.Queries.GetAiModels;
using Application.AI.Queries.GetAiProviders;
using Application.Common.Exceptions;
using Domain.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("ai")]
[RequestSizeLimit(65536)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("ai-ip")]
[ServiceFilter(typeof(AiSetupAuditFilter))]
public sealed class AiController : ApiControllerBase
{
    [HttpGet("providers")]
    public ActionResult<IReadOnlyList<AiProviderDto>> Providers([FromServices] GetAiProvidersQueryHandler handler) =>
        Ok(handler.Handle(new GetAiProvidersQuery()));

    [HttpPost("providers/{provider}/models")]
    public async Task<ActionResult<AiModelPage>> Models(AiProvider provider, AiModelsRequest request,
        [FromServices] GetAiModelsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetAiModelsQuery(provider, request.ApiKey), ct));

    [HttpGet("connections")]
    public async Task<ActionResult<AiConnectionPage>> Connections([FromQuery] GetAiConnectionsRequest request,
        [FromServices] GetAiConnectionsQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetAiConnectionsQuery(request.PageSize, request.After), ct));

    [HttpPost("connections")]
    public async Task<ActionResult<AiConnectionDto>> Create(CreateAiConnectionRequest request,
        [FromHeader(Name = "Idempotency-Key"), Required] string? requestId,
        [FromServices] CreateAiConnectionCommandHandler handler, CancellationToken ct)
    {
        if (!Guid.TryParseExact(requestId, "D", out var idempotencyKey) || idempotencyKey == Guid.Empty)
            throw new RequestValidationException("Idempotency-Key must be a non-empty UUID in D format.");
        var result = await handler.Handle(new CreateAiConnectionCommand(idempotencyKey, request.Provider,
            request.ApiKey, request.ModelId, request.DisplayName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("connections/{id:long:min(1)}")]
    public async Task<ActionResult<AiConnectionDto>> Update(long id, UpdateAiConnectionRequest request,
        [FromServices] UpdateAiConnectionCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new UpdateAiConnectionCommand(id, request.Version, request.DisplayName,
            request.ModelId, request.ApiKey, request.IsEnabled), ct));

    [HttpPost("connections/{id:long:min(1)}/verify")]
    public async Task<ActionResult<AiConnectionDto>> Verify(long id, VerifyAiConnectionRequest request,
        [FromServices] VerifyAiConnectionCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new VerifyAiConnectionCommand(id, request.Version), ct));

    [HttpDelete("connections/{id:long:min(1)}")]
    public async Task<IActionResult> Delete(long id, [FromQuery, Range(1, long.MaxValue)] long version,
        [FromServices] DeleteAiConnectionCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new DeleteAiConnectionCommand(id, version), ct);
        return NoContent();
    }
}
