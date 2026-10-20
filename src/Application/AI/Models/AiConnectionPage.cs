using Domain.AI;

namespace Application.AI.Models;

public sealed record AiConnectionPage(IReadOnlyList<AiConnectionDto> Items, string? NextCursor);
