using System.Globalization;
using Application.Common.Exceptions;
using Domain.GrammarPractice;

namespace Application.GrammarPractice.Validation;

internal static class GrammarPracticeValidation
{
    public static void Id(long value)
    {
        if (value <= 0) throw new RequestValidationException("A positive profile, session or question ID is required.");
    }

    public static void Version(long value)
    {
        if (value <= 0) throw new RequestValidationException("A positive practice session version is required.");
    }

    public static void QuestionCount(int value)
    {
        if (value is < GrammarPracticeSession.MinQuestions or > GrammarPracticeSession.MaxQuestions)
            throw new RequestValidationException($"Question count must be between {GrammarPracticeSession.MinQuestions} and {GrammarPracticeSession.MaxQuestions}.");
    }

    public static long? Page(int pageSize, string? before)
    {
        if (pageSize is < 1 or > 100) throw new RequestValidationException("Page size must be between 1 and 100.");
        if (before is null) return null;
        if (before.Length > 19 || !long.TryParse(before, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new RequestValidationException("Invalid practice session cursor.");
        return id;
    }
}
