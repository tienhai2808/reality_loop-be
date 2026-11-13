using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.GrammarPractice;

namespace Api.Contracts.GrammarPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record AnswerGrammarPracticeQuestionRequest
{
    [JsonRequired, Range(0, GrammarExercise.MaxOptions - 1)] public int SelectedOptionIndex { get; init; }
    [JsonRequired, Range(1, long.MaxValue)] public long Version { get; init; }
}
