using Application.Common.Exceptions;
using Application.LearningProgress.Models;

namespace Application.LearningProgress.Validation;

internal static class LearningProgressValidation
{
    public static void Id(long value)
    {
        if (value <= 0) throw new RequestValidationException("Profile and item IDs must be positive 64-bit integers.");
    }

    public static void Version(long value)
    {
        if (value < 0) throw new RequestValidationException("Version must be non-negative; use 0 only for an item that has not been started.");
    }

    public static void Status(WritingSystemItemLearningState? value)
    {
        if (value.HasValue && !Enum.IsDefined(value.Value))
            throw new RequestValidationException("Unknown learning progress status.");
    }
}
