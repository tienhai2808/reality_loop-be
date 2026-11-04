using Application.Handwriting.Models;

namespace Application.Handwriting.Commands.CreateHandwritingAttempt;

public sealed record CreateHandwritingAttemptCommand(long ProfileId, string SystemCode, IReadOnlyList<HandwritingCharacterInput> Characters);
