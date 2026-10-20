using Domain.AI;

namespace Infrastructure.AI.Providers;

public sealed class AiProviderOptions
{
    public const string SectionName = "AI:Providers";
    public string[] OpenAiFallbackModels { get; init; } = [];
    public string[] AnthropicFallbackModels { get; init; } = [];
    public string[] GoogleFallbackModels { get; init; } = [];

    public string[] Fallback(AiProvider provider) => provider switch
    {
        AiProvider.OpenAI => OpenAiFallbackModels,
        AiProvider.Anthropic => AnthropicFallbackModels,
        AiProvider.Google => GoogleFallbackModels,
        _ => []
    };
}
