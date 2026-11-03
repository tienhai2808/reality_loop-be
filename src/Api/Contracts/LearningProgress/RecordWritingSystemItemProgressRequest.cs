using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.LearningProgress.Models;

namespace Api.Contracts.LearningProgress;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RecordWritingSystemItemProgressRequest
{
    [JsonRequired, EnumDataType(typeof(WritingSystemItemProgressAction))]
    public WritingSystemItemProgressAction Action { get; init; }
    [JsonRequired, Range(0, long.MaxValue)] public long Version { get; init; }
}
