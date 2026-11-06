using Domain.VocabularyLearning;

namespace Application.VocabularyLearning.Models;

public sealed record VocabularyDueCursor(DateTimeOffset AsOf, DateTimeOffset DueAt, long Id)
{
    public string Encode() => string.Join(":", AsOf.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture),
        DueAt.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture), Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
}
