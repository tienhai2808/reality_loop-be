using System.Globalization;
using Application.Common.Exceptions;

namespace Application.LearningProfiles.Validation;

internal static class LearningProfileValidation
{
    public static void Id(long id)
    {
        if (id <= 0) throw new RequestValidationException("A positive learning profile ID is required.");
    }

    public static long ParseId(string value)
    {
        if (value is null || value.Length > 19 ||
            !long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new RequestValidationException("The learning profile ID must be a positive 64-bit integer string.");
        return id;
    }

    public static void Version(long version)
    {
        if (version <= 0) throw new RequestValidationException("A positive learning profile version is required.");
    }
}
