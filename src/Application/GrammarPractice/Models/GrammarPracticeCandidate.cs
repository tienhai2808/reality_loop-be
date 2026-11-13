using Domain.Grammar;
using Domain.GrammarPractice;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeCandidate(GrammarExercise Exercise, GrammarExerciseTranslation Translation,
    GrammarPoint GrammarPoint);
