namespace Application.Authentication.Models;

public sealed record SessionPage(IReadOnlyList<SessionDto> Items, string? NextCursor);
