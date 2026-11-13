using Domain.Common;

namespace Domain.GrammarPractice;

public sealed class GrammarExerciseTranslation : Entity
{
    public const int InstructionMaxLength = 500;
    public const int ExplanationMaxLength = 4000;

    private GrammarExerciseTranslation()
    {
    }

    public long GrammarExerciseId { get; private init; }
    public string Locale { get; private init; } = null!;
    public string Instruction { get; private set; } = null!;
    public string Explanation { get; private set; } = null!;
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarExerciseTranslation Create(long id, long exerciseId, string locale, string instruction,
        string explanation, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exerciseId);
        var tag = LanguageTag.Normalize(locale, nameof(locale));
        var prompt = FoundationContentValidation.RequiredText(instruction, InstructionMaxLength, nameof(instruction));
        var feedback = FoundationContentValidation.RequiredText(explanation, ExplanationMaxLength, nameof(explanation));
        var timestamp = now.ToUniversalTime();
        return new GrammarExerciseTranslation
        {
            Id = id,
            GrammarExerciseId = exerciseId,
            Locale = tag,
            Instruction = prompt,
            Explanation = feedback,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(string instruction, string explanation, DateTimeOffset now)
    {
        var prompt = FoundationContentValidation.RequiredText(instruction, InstructionMaxLength, nameof(instruction));
        var feedback = FoundationContentValidation.RequiredText(explanation, ExplanationMaxLength, nameof(explanation));
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);
        Instruction = prompt;
        Explanation = feedback;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }
}
