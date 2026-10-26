using System.Text;

namespace Domain.Common;

internal static class FoundationContentValidation
{
    public static string RequiredText(string value, int maxLength, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        var text = value.Trim().Normalize(NormalizationForm.FormC);
        if (text.Length > maxLength || text.Contains('\0'))
            throw new ArgumentException($"Text must contain at most {maxLength} characters and no null characters.", parameterName);
        return text;
    }

    public static string? OptionalText(string? value, int maxLength, string parameterName) =>
        string.IsNullOrWhiteSpace(value) ? null : RequiredText(value, maxLength, parameterName);

    public static string Code(string value, string parameterName)
    {
        var code = RequiredText(value, 64, parameterName).ToLowerInvariant();
        if (!char.IsAsciiLetterLower(code[0]) || code.Any(c => !char.IsAsciiLetterLower(c) && !char.IsAsciiDigit(c) && c != '-'))
            throw new ArgumentException("Codes must start with a lowercase ASCII letter and contain only letters, digits or hyphens.", parameterName);
        return code;
    }

    public static string? AudioUrl(string? value)
    {
        var url = OptionalText(value, 1024, nameof(value));
        if (url is null) return null;
        if (url.Any(char.IsWhiteSpace) || url.Any(char.IsControl) || url.Contains('\\') ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrEmpty(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException("Audio URLs must be absolute HTTPS URLs without credentials or fragments.", nameof(value));
        return url;
    }

    public static (long Version, DateTimeOffset UpdatedAt) Next(long version, DateTimeOffset updatedAt, DateTimeOffset now)
    {
        if (now < updatedAt)
            throw new ArgumentException("The update time cannot precede the previous update.", nameof(now));
        return (checked(version + 1), now.ToUniversalTime());
    }
}
