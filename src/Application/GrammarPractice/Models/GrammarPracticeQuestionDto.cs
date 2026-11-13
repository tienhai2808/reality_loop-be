using Domain.GrammarPractice;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeQuestionDto(string Id, string GrammarExerciseId, string GrammarPointId, int Position,
    string Prompt, string Instruction, string TranslationLocale, bool IsTranslationFallback, IReadOnlyList<string> Options,
    int? SelectedOptionIndex, int? CorrectOptionIndex, bool? IsCorrect, string? Explanation, DateTimeOffset? AnsweredAt)
{
    public static GrammarPracticeQuestionDto From(GrammarPracticeSessionItem item, string explanationLanguage) => new(
        item.Id.ToString(), item.GrammarExerciseId.ToString(), item.GrammarPointId.ToString(), item.Position,
        item.Prompt, item.Instruction, item.TranslationLocale, item.TranslationLocale != explanationLanguage,
        item.Options, item.SelectedOptionIndex, item.AnsweredAt.HasValue ? item.CorrectOptionIndex : null,
        item.IsCorrect, item.AnsweredAt.HasValue ? item.Explanation : null, item.AnsweredAt);
}
