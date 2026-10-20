using Domain.AI;

namespace Application.AI.Models;

public sealed record AiModelPage(IReadOnlyList<AiModelDto> Items, bool IsFallback);
