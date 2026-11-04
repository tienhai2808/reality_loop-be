using System.Text.Json.Serialization;
using HandwritingLab.Models;

namespace HandwritingLab.Contracts;

public sealed record GradeRequest
{
    [JsonRequired] public string TargetId { get; init; } = null!;
    [JsonRequired] public IReadOnlyList<IReadOnlyList<IReadOnlyList<InkPoint>>> Characters { get; init; } = null!;
}
