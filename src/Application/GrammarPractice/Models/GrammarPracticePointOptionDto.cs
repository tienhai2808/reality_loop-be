namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticePointOptionDto(string Id, string Code, string NativeName, string Name,
    string? TranslationLocale, bool IsTranslationFallback, int AvailableQuestionCount, int SortOrder);
