using System.Globalization;
using Application.Common.Exceptions;
using Application.VocabularyLearning.Models;

namespace Application.VocabularyLearning.Validation;

internal static class VocabularyLearningValidation
{
    public static void Id(long id)
    {
        if (id <= 0) throw new RequestValidationException("Positive profile, sense and progress IDs are required.");
    }
    public static void PageSize(int size)
    {
        if (size is < 1 or > 100) throw new RequestValidationException("Page size must be between 1 and 100.");
    }
    public static long? Before(string? value)
    {
        if (value is null) return null;
        if (value.Length > 19 || !long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new RequestValidationException("Invalid review cursor.");
        return id;
    }
    public static VocabularyDueCursor? Cursor(string? value, DateTimeOffset now)
    {
        if (value is null) return null;
        var parts = value.Split(':');
        if (value.Length > 59 || parts.Length != 3 || !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var at) ||
            !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var due) ||
            !long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0 ||
            at <= 0 || due < 0 || at > now.UtcTicks || due > DateTimeOffset.MaxValue.UtcTicks || at % 10 != 0 || due % 10 != 0)
            throw new RequestValidationException("Invalid vocabulary queue cursor.");
        return new(new(at, TimeSpan.Zero), new(due, TimeSpan.Zero), id);
    }
    public static DateTimeOffset Timestamp(DateTimeOffset now) => new(now.UtcTicks / 10 * 10, TimeSpan.Zero);
}
