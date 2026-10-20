using System.ComponentModel.DataAnnotations;
using Domain.AI;

namespace Api.Contracts.AI;

public sealed record CreateAiConnectionRequest
{
    [EnumDataType(typeof(AiProvider))] public AiProvider Provider { get; init; }
    [Required, StringLength(8192, MinimumLength = 8)] public string ApiKey { get; init; } = "";
    [Required, StringLength(200)] public string ModelId { get; init; } = "";
    [StringLength(150)] public string? DisplayName { get; init; }
    public override string ToString() => "CreateAiConnectionRequest (API key redacted)";
}
