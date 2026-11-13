using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.GrammarPractice;

namespace Api.Contracts.GrammarPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CreateGrammarPracticeSessionRequest
{
    [StringLength(64, MinimumLength = 1)] public string? GrammarPointCode { get; init; }
    [Range(GrammarPracticeSession.MinQuestions, GrammarPracticeSession.MaxQuestions)]
    public int QuestionCount { get; init; } = 10;
}
