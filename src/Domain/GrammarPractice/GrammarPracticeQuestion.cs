using Domain.Grammar;
using Domain.Languages;

namespace Domain.GrammarPractice;

// Validated immutable input to the session aggregate, including the selected option permutation.
public sealed class GrammarPracticeQuestion
{
    private readonly string[] _options;
    private GrammarPracticeQuestion(long id, GrammarExercise exercise, GrammarExerciseTranslation translation,
        GrammarPoint point, Language language, int[] order)
    {
        Id = id;
        GrammarExerciseId = exercise.Id;
        ExerciseVersion = exercise.Version;
        GrammarPointId = point.Id;
        LanguageId = language.Id;
        Locale = translation.Locale;
        TranslationVersion = translation.Version;
        Prompt = exercise.Prompt;
        Instruction = translation.Instruction;
        Explanation = translation.Explanation;
        var options = exercise.Options;
        _options = order.Select(index => options[index]).ToArray();
        CorrectOptionIndex = Array.IndexOf(order, exercise.CorrectOptionIndex);
        SourceUpdatedAt = new[] { exercise.UpdatedAt, translation.UpdatedAt, point.UpdatedAt, language.UpdatedAt }.Max();
    }

    public long Id { get; }
    public long GrammarExerciseId { get; }
    public long GrammarPointId { get; }
    public long LanguageId { get; }
    public long ExerciseVersion { get; }
    public long TranslationVersion { get; }
    public string Locale { get; }
    public string Prompt { get; }
    public string Instruction { get; }
    public string Explanation { get; }
    public IReadOnlyList<string> Options => Array.AsReadOnly(_options);
    public int CorrectOptionIndex { get; }
    public DateTimeOffset SourceUpdatedAt { get; }

    public static GrammarPracticeQuestion Create(long id, GrammarExercise exercise, GrammarExerciseTranslation translation,
        GrammarPoint point, Language language, IReadOnlyList<int> optionOrder)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(exercise);
        ArgumentNullException.ThrowIfNull(translation);
        ArgumentNullException.ThrowIfNull(point);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(optionOrder);
        if (exercise.GrammarPointId != point.Id || point.LanguageId != language.Id || translation.GrammarExerciseId != exercise.Id)
            throw new ArgumentException("Exercise, translation and grammar point must belong to the same language.");
        if (!exercise.IsPublished || !point.IsPublished || !language.IsEnabled)
            throw new ArgumentException("Practice requires published content and an enabled language.");
        var count = exercise.Options.Length;
        var order = optionOrder.ToArray();
        if (order.Length != count || order.Distinct().Count() != count || order.Any(index => index < 0 || index >= count))
            throw new ArgumentException("Option order must be a permutation of the exercise's option indices.", nameof(optionOrder));
        return new(id, exercise, translation, point, language, order);
    }
}
