using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Models;

public sealed record WritingSystemPracticeQuestionDto(string Id, string WritingSystemItemId, int Position,
    string Symbol, string TranscriptionSystem, IReadOnlyList<string> Options, int? SelectedOptionIndex,
    int? CorrectOptionIndex, bool? IsCorrect, DateTimeOffset? AnsweredAt)
{
    public static WritingSystemPracticeQuestionDto From(WritingSystemPracticeSessionItem item) => new(
        item.Id.ToString(), item.WritingSystemItemId.ToString(), item.Position, item.Symbol, item.TranscriptionSystem,
        item.Options, item.SelectedOptionIndex, item.AnsweredAt.HasValue ? item.CorrectOptionIndex : null,
        item.IsCorrect, item.AnsweredAt);
}
