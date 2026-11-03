using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Contracts.WritingSystemPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AbandonWritingSystemPracticeSessionRequest
{
    [JsonRequired, Range(1, long.MaxValue)] public long Version { get; init; }
}
