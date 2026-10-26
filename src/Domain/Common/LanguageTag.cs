namespace Domain.Common;

public static class LanguageTag
{
    public const int MaxLength = 35;

    public static string Normalize(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        var tag = value.Trim().ToLowerInvariant();
        if (tag.Length > MaxLength)
            throw new ArgumentException($"Language tags cannot exceed {MaxLength} characters.", parameterName);
        var subtags = tag.Split('-');
        if (subtags[0].Length is < 2 or > 8 || subtags[0].Any(c => c is < 'a' or > 'z') ||
            subtags.Skip(1).Any(part => part.Length is < 1 or > 8 ||
                part.Any(c => c is not (>= 'a' and <= 'z') and not (>= '0' and <= '9'))))
            throw new ArgumentException("Use a language tag with hyphen-separated ASCII subtags, such as en-us or zh-hant.", parameterName);
        return tag;
    }
}
