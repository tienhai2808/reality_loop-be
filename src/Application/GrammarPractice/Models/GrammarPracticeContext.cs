using Domain.Grammar;
using Domain.Languages;
using Domain.LearningProfiles;

namespace Application.GrammarPractice.Models;

public sealed record GrammarPracticeContext(LearningProfile Profile, Language Language, GrammarPoint? SelectedPoint);
