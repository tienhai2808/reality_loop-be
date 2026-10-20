using Domain.AI;

namespace Application.AI.Models;

public sealed record AiConnectionDto(string Id, AiProvider Provider, string DisplayName, string ApiKeyLast4,
    string ModelId, int SortOrder, bool IsEnabled, AiConnectionStatus Status, DateTimeOffset? RetryAfterAt,
    AiFailureCode? LastFailureCode, long Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static AiConnectionDto From(AiConnection x) => new(x.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
        x.Provider, x.DisplayName, x.ApiKeyLast4, x.ModelId, x.SortOrder, x.IsEnabled, x.Status,
        x.RetryAfterAt, x.LastFailureCode, x.Version, x.CreatedAt, x.UpdatedAt);
}
