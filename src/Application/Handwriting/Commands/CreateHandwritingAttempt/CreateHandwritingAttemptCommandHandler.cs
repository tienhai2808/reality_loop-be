using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Handwriting.Models;
using Application.Handwriting.Scoring;
using Application.Handwriting.Validation;
using Domain.Handwriting;

namespace Application.Handwriting.Commands.CreateHandwritingAttempt;

public sealed class CreateHandwritingAttemptCommandHandler(ICurrentUser current, IHandwritingRepository repository,
    HandwritingGrader grader, HandwritingScoringPolicy policy, IIdGenerator ids, TimeProvider clock)
{
    public async Task<HandwritingAttemptDto> Handle(CreateHandwritingAttemptCommand command, CancellationToken ct)
    {
        HandwritingValidation.Id(command.ProfileId);
        var code = CatalogQueryValidation.Code(command.SystemCode);
        // Validate and bound all untrusted geometry before any database access or scoring work.
        var drawings = HandwritingValidation.Drawings(command.Characters);
        var requestedIds = drawings.Select(x => x.TemplateId).Distinct().ToArray();
        var catalog = await repository.GetCatalogAsync(current.UserId, command.ProfileId, code, requestedIds, ct)
            ?? throw new NotFoundException("Learning profile or published writing system was not found.");
        if (catalog.Items.Count != requestedIds.Length)
            throw new NotFoundException("One or more published handwriting templates were not found in this writing system.");
        var references = catalog.Items.ToDictionary(x => x.Template.Id);
        var answers = drawings.Select(drawing =>
        {
            var reference = references[drawing.TemplateId];
            return new HandwritingPracticeAnswer(ids.NewId(), reference.Template, reference.Item, drawing.Drawing,
                grader.Grade(reference.Template.Drawing, drawing.Drawing, ct));
        }).ToArray();
        var attempt = HandwritingPracticeAttempt.Create(ids.NewId(), catalog.Profile, catalog.WritingSystem,
            catalog.Language, policy.Version, answers, clock.GetUtcNow());
        await repository.CreateAsync(current.UserId, attempt, catalog, ct);
        // The saved graph already contains every result: no read-back query.
        return HandwritingAttemptDto.From(attempt, catalog);
    }
}
