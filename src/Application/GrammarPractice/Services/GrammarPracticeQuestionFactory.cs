using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.GrammarPractice.Models;
using Domain.GrammarPractice;

namespace Application.GrammarPractice.Services;

internal static class GrammarPracticeQuestionFactory
{
    public static GrammarPracticeQuestion[] Create(GrammarPracticeContext context,
        IReadOnlyList<GrammarPracticeCandidate> candidates, int count, IIdGenerator ids)
    {
        if (candidates.Count < count)
            throw new RequestValidationException($"Only {candidates.Count} published questions with a supported translation are available in this scope. Request fewer questions or choose another scope.");

        return candidates.Take(count).Select(candidate =>
        {
            var order = Enumerable.Range(0, candidate.Exercise.Options.Length).ToArray();
            Random.Shared.Shuffle(order);
            return GrammarPracticeQuestion.Create(ids.NewId(), candidate.Exercise, candidate.Translation,
                candidate.GrammarPoint, context.Language, order);
        }).ToArray();
    }
}
