using Domain.Handwriting;

namespace Application.Handwriting.Models;

public sealed record HandwritingAttemptDto(HandwritingAttemptSummaryDto Attempt,
    IReadOnlyList<HandwritingAttemptCharacterDto> Characters)
{
    public static HandwritingAttemptDto From(HandwritingPracticeAttempt attempt, HandwritingCatalog catalog) =>
        new(HandwritingAttemptSummaryDto.From(attempt), attempt.Characters.OrderBy(x => x.Position).Select(character =>
            new HandwritingAttemptCharacterDto(character.Id.ToString(), character.Position,
                HandwritingReferenceDto.From(catalog.Items.Single(x => x.Template.Id == character.TemplateId).Template),
                character.DrawingFormatVersion, character.Drawing.Strokes, character.FeedbackFormatVersion,
                HandwritingFeedbackDto.From(character.Feedback))).ToArray());
}
