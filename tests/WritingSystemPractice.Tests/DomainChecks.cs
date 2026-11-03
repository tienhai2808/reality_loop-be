using System.Reflection;
using Domain.Languages;
using Domain.LearningProfiles;
using Domain.WritingSystemPractice;
using Domain.WritingSystems;

namespace WritingSystemPractice.Tests;

internal static class DomainChecks
{
    public static void Run()
    {
        var now = new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.FromHours(7));
        var language = Language.Create(1, "ja", "日本語", true, 0, now);
        var system = WritingSystem.Create(2, 1, "hiragana", "ひらがな", true, 0, now);
        var profile = LearningProfile.Create(3, 4, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        var a = WritingSystemItem.Create(5, 2, "a", "あ", 0, null, null, now);
        var i = WritingSystemItem.Create(6, 2, "i", "い", 1, null, null, now);
        var ar = WritingSystemItemReading.Create(7, 5, "hepburn", "a", "hepburn", null, 0, now);
        var ir = WritingSystemItemReading.Create(8, 6, "hepburn", "i", "hepburn", null, 0, now);
        var options = new[] { " i ", "A", "u", "e" };
        var q1 = WritingSystemPracticeQuestion.Create(10, a, ar, [ar], options);
        var q2 = WritingSystemPracticeQuestion.Create(11, i, ir, [ir], ["a", "i"]);
        WritingSystemPracticeSession Session() => WritingSystemPracticeSession.Create(20, profile, system, language, [q1, q2], now);
        var session = Session();
        Check.That(q1.Options.SequenceEqual(new[] { "i", "A", "u", "e" }) && q1.CorrectOptionIndex == 1, "Normalize options and derive correct index.");
        options[1] = "mutated";
        Check.That(q1.Options[1] == "A", "Question input must be copied.");
        var exposed = session.Items.First().Options;
        exposed[1] = "mutated";
        Check.That(session.Items.First().Options[1] == "A", "Entity option arrays must not be externally mutable.");
        Check.That(session.Version == 1 && session.Status == WritingSystemPracticeSessionStatus.InProgress && session.EndedAt is null, "Initial session state.");
        Check.That(session.QuestionCount == 2 && session.Items.Select(x => x.Position).SequenceEqual(new[] { 0, 1 }), "Stable zero-based order.");
        Check.That(session.Items.All(x => x.SessionId == 20 && x.Version == 1 && x.IsCorrect is null && x.AnsweredAt is null), "Initial item state.");
        Check.That(session.CreatedAt.Offset == TimeSpan.Zero && session.Items.All(x => x.CreatedAt == session.CreatedAt), "UTC and shared creation time.");
        a.Update("changed", 0, null, null, now.AddSeconds(1));
        ar.Update("changed", "hepburn", null, 0, now.AddSeconds(1));
        Check.That(session.Items.First().Symbol == "あ" && session.Items.First().Options[1] == "A", "Question snapshots survive catalog edits.");

        session.Answer(10, 1, now.AddSeconds(1));
        var first = session.Items.Single(x => x.Id == 10);
        Check.That(first.IsCorrect == true && first.Version == 2 && first.AnsweredAt == now.AddSeconds(1), "Correct answer recorded once.");
        Check.That(session.Version == 2 && session.Status == WritingSystemPracticeSessionStatus.InProgress, "Partial session stays active.");
        session.Answer(10, 1, now.AddSeconds(2));
        Check.That(session.Version == 2 && session.UpdatedAt == now.AddSeconds(1) && first.Version == 2, "Same-option retry is a no-op.");
        Check.Throws<InvalidOperationException>(() => session.Answer(10, 0, now.AddSeconds(2)), "Cannot change an answer.");
        Check.Throws<ArgumentOutOfRangeException>(() => session.Answer(11, -1, now.AddSeconds(2)), "Negative option rejected.");
        Check.Throws<ArgumentOutOfRangeException>(() => session.Answer(11, 2, now.AddSeconds(2)), "Out-of-range option rejected.");
        Check.Throws<ArgumentException>(() => session.Answer(999, 0, now.AddSeconds(2)), "Foreign question rejected.");
        Check.Throws<ArgumentException>(() => session.Answer(11, 0, now), "Backdated answer rejected.");
        Check.That(session.Version == 2 && session.Items.Last().SelectedOptionIndex is null, "Rejected answers leave no partial mutation.");
        session.Answer(11, 0, now.AddSeconds(3));
        Check.That(session.Status == WritingSystemPracticeSessionStatus.Completed && session.EndedAt == now.AddSeconds(3) && session.Version == 3, "Final answer completes session.");
        Check.That(session.Items.Count(x => x.IsCorrect == true) == 1 && session.Items.Last().IsCorrect == false, "Score is derived from answers.");
        session.Answer(11, 0, now.AddSeconds(4));
        Check.That(session.Version == 3 && session.EndedAt == now.AddSeconds(3), "Last-answer retry after completion is a no-op.");
        Check.Throws<InvalidOperationException>(() => session.Abandon(now.AddSeconds(4)), "Completed session cannot be abandoned.");
        Check.That(profile.Version == 1, "Practice does not change learning-profile version.");

        var abandoned = Session();
        abandoned.Answer(10, 0, now.AddSeconds(1));
        abandoned.Abandon(now.AddSeconds(2));
        Check.That(abandoned.Status == WritingSystemPracticeSessionStatus.Abandoned && abandoned.Items.First().IsCorrect == false, "Abandon preserves prior answers.");
        abandoned.Abandon(now.AddSeconds(3));
        abandoned.Answer(10, 0, now.AddSeconds(3));
        Check.That(abandoned.Version == 3 && abandoned.EndedAt == now.AddSeconds(2), "Abandon and recorded-answer retries are no-ops.");
        Check.Throws<InvalidOperationException>(() => abandoned.Answer(11, 1, now.AddSeconds(3)), "Abandoned session cannot accept a new answer.");
        Check.Throws<ArgumentException>(() => abandoned.Abandon(now), "Backdated terminal no-op rejected.");

        Check.Throws<ArgumentOutOfRangeException>(() => WritingSystemPracticeSession.Create(0, profile, system, language, [q1], now), "Positive Snowflake session ID required.");
        Check.Throws<ArgumentOutOfRangeException>(() => WritingSystemPracticeQuestion.Create(0, i, ir, [ir], ["i", "u"]), "Positive Snowflake question ID required.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, language, [], now), "Empty session rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, language, Enumerable.Repeat(q1, 21).ToArray(), now), "Oversized session rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, language, [q1, q1], now), "Duplicate question IDs/items rejected.");
        var duplicateId = WritingSystemPracticeQuestion.Create(10, i, ir, [ir], ["i", "u"]);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, language, [q1, duplicateId], now), "Duplicate ID with different items rejected.");
        var duplicateItem = WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i", "u"]);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, language, [q2, duplicateItem], now), "Duplicate target with different IDs rejected.");
        var otherSystem = WritingSystem.Create(99, 1, "katakana", "カタカナ", true, 1, now);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, otherSystem, language, [q2], now), "Cross-system question rejected.");
        var foreignProfile = LearningProfile.Create(30, 4, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, now);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, foreignProfile, system, language, [q2], now), "Wrong target language rejected.");
        var otherLanguage = Language.Create(31, "ja", "日本語", true, 0, now);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeSession.Create(20, profile, system, otherLanguage, [q2], now), "Wrong catalog ancestry rejected.");
        system.Update(system.NativeName, false, 0, now.AddSeconds(1));
        Check.Throws<ArgumentException>(() => Session(), "Unpublished system rejected.");
        system.Update(system.NativeName, true, 0, now.AddSeconds(2));
        language.Update(language.NativeName, false, 0, now.AddSeconds(1));
        Check.Throws<ArgumentException>(() => Session(), "Disabled language rejected.");
        language.Update(language.NativeName, true, 0, now.AddSeconds(2));

        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i"]), "Too few options rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i", "a", "u", "e", "o"]), "Too many options rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], [" i ", "I"]), "Normalized case-insensitive duplicates rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i", "\t"]), "Blank option rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i", new string('x', 201)]), "Oversized option rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["i", "x\0"]), "NUL option rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir], ["a", "u"]), "Missing correct answer rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [], ["i", "u"]), "Correct reading must be included.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir, ar], ["i", "u"]), "Foreign reading rejected.");
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ar, [ir], ["i", "u"]), "Correct reading belongs to target.");
        var alternate = WritingSystemItemReading.Create(40, 6, "alternate", "ee", "hepburn", null, 1, now);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, ir, [ir, alternate], ["i", "ee"]), "Alternate valid reading cannot be distractor.");
        var accent = WritingSystemItemReading.Create(41, 6, "accent", "é", "hepburn", null, 2, now);
        Check.Throws<ArgumentException>(() => WritingSystemPracticeQuestion.Create(12, i, accent, [accent], ["é", "e\u0301"]), "NFC-equivalent duplicate options rejected.");
        var normalized = WritingSystemPracticeQuestion.Create(12, i, accent, [accent], ["e\u0301", "i"]);
        Check.That(normalized.Options[0] == "é", "NFC option normalization.");

        var overflow = Session();
        typeof(WritingSystemPracticeSession).GetProperty(nameof(overflow.Version))!.SetValue(overflow, long.MaxValue);
        Check.Throws<OverflowException>(() => overflow.Answer(10, 0, now.AddSeconds(1)), "Session version overflow rejected.");
        Check.That(overflow.Items.First().SelectedOptionIndex is null && overflow.UpdatedAt == now, "Overflow cannot partially answer a question.");
        var itemOverflow = Session();
        typeof(WritingSystemPracticeSessionItem).GetProperty(nameof(WritingSystemPracticeSessionItem.Version))!.SetValue(itemOverflow.Items.First(), long.MaxValue);
        Check.Throws<OverflowException>(() => itemOverflow.Answer(10, 0, now.AddSeconds(1)), "Item version overflow rejected.");
        Check.That(itemOverflow.Version == 1 && itemOverflow.Items.First().SelectedOptionIndex is null, "Item overflow cannot partially change session.");
        var incomplete = Session();
        var field = typeof(WritingSystemPracticeSession).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!;
        ((List<WritingSystemPracticeSessionItem>)field.GetValue(incomplete)!).RemoveAt(1);
        Check.Throws<InvalidOperationException>(() => incomplete.Answer(10, 0, now.AddSeconds(1)), "Partially loaded aggregate cannot finish prematurely.");
    }
}
