using Application.Common.Exceptions;
using Domain.AI;

namespace Application.AI.Services;

public static class AiSetupValidation
{
    public static string Key(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new RequestValidationException("An AI API key is required.");
        var key = value.Trim();
        if (key.Length is < 8 or > 8192 || key.Any(c => c < '!' || c > '~'))
            throw new RequestValidationException("The AI API key has an invalid format.");
        return key;
    }

    public static string Model(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new RequestValidationException("A model is required.");
        var model = value.Trim();
        if (model.Length > AiConnection.ModelIdMaxLength || model.Any(char.IsWhiteSpace) || model.Any(char.IsControl))
            throw new RequestValidationException("The AI model ID has an invalid format.");
        return model;
    }

    public static string Name(string? value, AiProvider provider)
    {
        var name = string.IsNullOrWhiteSpace(value) ? provider.ToString() : value.Trim();
        if (name.Length > AiConnection.DisplayNameMaxLength || name.Any(char.IsControl))
            throw new RequestValidationException("The AI connection name is invalid.");
        return name;
    }

    public static void Version(long version)
    {
        if (version <= 0) throw new RequestValidationException("A positive connection version is required.");
    }
}
