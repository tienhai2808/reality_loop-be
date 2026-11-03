using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.WritingSystemPractice;

namespace Api.Contracts.WritingSystemPractice;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CreateWritingSystemPracticeSessionRequest
{
    [Range(1, WritingSystemPracticeSession.MaxQuestions)] public int QuestionCount { get; init; } = 10;
}
