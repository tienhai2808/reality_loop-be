using System.ComponentModel.DataAnnotations;
using Api.Contracts.LearningProfiles;
using Application.LearningProfiles.Commands.CreateLearningProfile;
using Application.LearningProfiles.Commands.DeleteLearningProfile;
using Application.LearningProfiles.Commands.SetCurrentLearningProfile;
using Application.LearningProfiles.Commands.UpdateLearningProfilePreferences;
using Application.LearningProfiles.Models;
using Application.LearningProfiles.Queries.GetCurrentLearningProfile;
using Application.LearningProfiles.Queries.GetLearningProfile;
using Application.LearningProfiles.Queries.GetLearningProfileOptions;
using Application.LearningProfiles.Queries.GetLearningProfiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("learning-profiles")]
[RequestSizeLimit(8192)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[EnableRateLimiting("learning-profiles")]
public sealed class LearningProfilesController : ApiControllerBase
{
    [HttpGet("options")]
    public ActionResult<LearningProfileOptionsDto> Options([FromServices] GetLearningProfileOptionsQueryHandler handler) =>
        Ok(handler.Handle(new GetLearningProfileOptionsQuery()));

    [HttpGet]
    public async Task<ActionResult<LearningProfilePage>> List([FromQuery] GetLearningProfilesRequest request,
        [FromServices] GetLearningProfilesQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetLearningProfilesQuery(request.PageSize, request.After), ct));

    [HttpGet("{id:long:min(1)}")]
    public async Task<ActionResult<LearningProfileDto>> GetById(long id,
        [FromServices] GetLearningProfileQueryHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new GetLearningProfileQuery(id), ct));

    [HttpPost]
    [ProducesResponseType<LearningProfileDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<LearningProfileDto>> Create(CreateLearningProfileRequest request,
        [FromServices] CreateLearningProfileCommandHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateLearningProfileCommand(request.TargetLanguage,
            request.NativeLanguage, request.ExplanationLanguage, request.ProficiencyLevel, request.PrimaryGoal, request.GoalNote), ct);
        // Prefix-aware link generation uses the same global route convention as the other controllers.
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long:min(1)}/preferences")]
    public async Task<ActionResult<LearningProfileDto>> UpdatePreferences(long id, UpdateLearningProfilePreferencesRequest request,
        [FromServices] UpdateLearningProfilePreferencesCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new UpdateLearningProfilePreferencesCommand(id, request.Version, request.NativeLanguage,
            request.ExplanationLanguage, request.ProficiencyLevel, request.PrimaryGoal, request.GoalNote), ct));

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, [FromQuery, Range(1, long.MaxValue)] long version,
        [FromServices] DeleteLearningProfileCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new DeleteLearningProfileCommand(id, version), ct);
        return NoContent();
    }

    [HttpGet("current")]
    [ProducesResponseType<CurrentLearningProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<CurrentLearningProfileDto>> Current(
        [FromServices] GetCurrentLearningProfileQueryHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetCurrentLearningProfileQuery(), ct);
        return result is null ? NoContent() : Ok(result);
    }

    [HttpPut("current")]
    public async Task<ActionResult<CurrentLearningProfileDto>> SetCurrent(SetCurrentLearningProfileRequest request,
        [FromServices] SetCurrentLearningProfileCommandHandler handler, CancellationToken ct) =>
        Ok(await handler.Handle(new SetCurrentLearningProfileCommand(request.ProfileId), ct));
}
