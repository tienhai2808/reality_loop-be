namespace Infrastructure.Persistence.Configurations;

internal static class GrammarPracticeConstraints
{
    internal const string OptionsShape = """
        CASE WHEN array_ndims(options) = 1 AND array_lower(options, 1) = 1
            THEN cardinality(options) BETWEEN 2 AND 4 AND array_position(options, NULL) IS NULL
            ELSE FALSE END
        """;
    internal const string OptionsText = """
        options[1] ~ '[^[:space:]]' AND options[2] ~ '[^[:space:]]'
        AND (cardinality(options) < 3 OR options[3] ~ '[^[:space:]]')
        AND (cardinality(options) < 4 OR options[4] ~ '[^[:space:]]')
        """;
    internal const string OptionsDistinct = """
        options[1] <> options[2]
        AND (cardinality(options) < 3 OR (options[1] <> options[3] AND options[2] <> options[3]))
        AND (cardinality(options) < 4 OR (options[1] <> options[4] AND options[2] <> options[4] AND options[3] <> options[4]))
        """;
    internal const string Prompt = """
        prompt ~ '[^[:space:]]' AND length(prompt) - length(replace(prompt, '{blank}', '')) = 7
        """;
    internal const string Locale = "translation_locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'";
}
