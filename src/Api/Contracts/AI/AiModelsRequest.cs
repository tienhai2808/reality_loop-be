using System.ComponentModel.DataAnnotations;
using Domain.AI;

namespace Api.Contracts.AI;

public sealed record AiModelsRequest
{
    [Required, StringLength(8192, MinimumLength = 8)] public string ApiKey { get; init; } = "";
    public override string ToString() => "AiModelsRequest (API key redacted)";
}
