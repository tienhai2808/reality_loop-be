using System.Globalization;
using Application.Common.Exceptions;
using Application.Common.Models;
using Domain.Common;

namespace Application.Common.Validation;

internal static class CatalogQueryValidation
{
    public static string Language(string value)
    {
        try { return LanguageTag.Normalize(value, nameof(value)); }
        catch (ArgumentException ex) { throw new RequestValidationException(ex.Message); }
    }

    public static string Locale(string? value) => value is null ? "en" : Language(value);

    public static string Code(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new RequestValidationException("A content code is required.");
        var code = value.Trim().ToLowerInvariant();
        if (code.Length > 64 || !char.IsAsciiLetterLower(code[0]) ||
            code.Any(c => !char.IsAsciiLetterLower(c) && !char.IsAsciiDigit(c) && c != '-'))
            throw new RequestValidationException("Content codes must start with a letter and contain at most 64 ASCII letters, digits or hyphens.");
        return code;
    }

    public static CatalogCursor? Page(int pageSize, string? after)
    {
        if (pageSize is < 1 or > 100) throw new RequestValidationException("Page size must be between 1 and 100.");
        if (after is null) return null;
        if (after.Length > 30) throw new RequestValidationException("Invalid catalog cursor.");
        var parts = after.Split(':');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var order) ||
            !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
            throw new RequestValidationException("Invalid catalog cursor.");
        return new CatalogCursor(order, id);
    }
}
