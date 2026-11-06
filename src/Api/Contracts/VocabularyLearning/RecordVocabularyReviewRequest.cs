using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.VocabularyLearning;

namespace Api.Contracts.VocabularyLearning;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed record RecordVocabularyReviewRequest
{
    [JsonRequired] public Guid RequestId { get; init; }
    [JsonRequired, Range(1, long.MaxValue)] public long Version { get; init; }
    [JsonRequired, EnumDataType(typeof(VocabularyReviewRating)), JsonConverter(typeof(VocabularyReviewRatingJsonConverter))]
    public VocabularyReviewRating Rating { get; init; }
}
