using System.Text.Json.Serialization;
using Domain.VocabularyLearning;

namespace Api.Contracts.VocabularyLearning;

public sealed class VocabularyReviewRatingJsonConverter() : JsonStringEnumConverter<VocabularyReviewRating>(allowIntegerValues: false);
