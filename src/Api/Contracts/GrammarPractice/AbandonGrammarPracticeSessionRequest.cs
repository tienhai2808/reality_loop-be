using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Contracts.GrammarPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AbandonGrammarPracticeSessionRequest
{
    [JsonRequired, Range(1, long.MaxValue)] public long Version { get; init; }
}
