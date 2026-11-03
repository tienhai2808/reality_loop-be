using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.WritingSystemPractice;

namespace Api.Contracts.WritingSystemPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AnswerWritingSystemPracticeQuestionRequest
{
    [JsonRequired, Range(0, WritingSystemPracticeQuestion.MaxOptions - 1)] public int SelectedOptionIndex { get; init; }
    [JsonRequired, Range(1, long.MaxValue)] public long Version { get; init; }
}
