using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.WritingSystemPractice.Models;
using Domain.WritingSystemPractice;

namespace Application.WritingSystemPractice.Services;

internal static class WritingSystemPracticeQuestionFactory
{
    public static WritingSystemPracticeQuestion[] Create(WritingSystemPracticeCatalog catalog, int count, IIdGenerator ids)
    {
        // Use one transcription system for the whole session. A target needs at least one genuinely wrong option.
        var groups = catalog.Items.SelectMany(x => x.Readings).GroupBy(x => x.TranscriptionSystem)
            .Select(group =>
            {
                var pool = group.Select(x => x.Transcription).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                var items = catalog.Items.Where(item => item.Readings.Any(x => x.TranscriptionSystem == group.Key) &&
                    item.Readings.Where(x => x.TranscriptionSystem == group.Key).Select(x => x.Transcription)
                        .Distinct(StringComparer.OrdinalIgnoreCase).Count() < pool.Length).ToArray();
                return new { Code = group.Key, Pool = pool, Items = items };
            }).OrderByDescending(x => x.Items.Length).ThenBy(x => x.Code, StringComparer.Ordinal).FirstOrDefault();
        if (groups is null || groups.Items.Length < count)
            throw new RequestValidationException($"The catalog supports at most {groups?.Items.Length ?? 0} unambiguous questions in one transcription system. Request fewer questions or choose another writing system.");

        Random.Shared.Shuffle(groups.Items);
        return groups.Items.Take(count).Select(candidate =>
        {
            var readings = candidate.Readings.Where(x => x.TranscriptionSystem == groups.Code)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToArray();
            var accepted = readings.Select(x => x.Transcription).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var distractors = groups.Pool.Where(x => !accepted.Contains(x)).ToArray();
            Random.Shared.Shuffle(distractors);
            var options = distractors.Take(WritingSystemPracticeQuestion.MaxOptions - 1)
                .Append(readings[0].Transcription).ToArray();
            Random.Shared.Shuffle(options);
            return WritingSystemPracticeQuestion.Create(ids.NewId(), candidate.Item, readings[0], candidate.Readings, options);
        }).ToArray();
    }
}
