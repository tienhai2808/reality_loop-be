using System.Text.Json;
using Domain.Grammar;
using Domain.GrammarPractice;
using Domain.Languages;
using Domain.LearningProfiles;

namespace GrammarPractice.Tests;

internal static class DomainChecks
{
    internal static readonly DateTimeOffset Now = new(2026, 11, 11, 12, 0, 0, TimeSpan.Zero);
    private static readonly Language Language = Language.Create(20, "de", "Deutsch", true, 0, Now);
    private static readonly GrammarPoint Point = GrammarPoint.Create(30, 20, "formal-address", "Anrede", "Sie", true, 0, Now);
    private static LearningProfile Profile(string language = "de") => LearningProfile.Create(10, 1, language, "vi", "vi",
        ProficiencyLevel.Beginner, LearningGoal.Study, null, Now);
    private static GrammarExercise Exercise(long id = 40, string prompt = "Frau Müller, kommen {blank}?", string[]? options = null) =>
        GrammarExercise.Create(id, 30, "formal-address-" + id, prompt, options ?? ["Sie", "sie", "du", "ihr"], 0, true, 0, Now);
    private static GrammarExerciseTranslation Translation(long exerciseId = 40, long id = 50) =>
        GrammarExerciseTranslation.Create(id, exerciseId, "vi", "Chọn đại từ lịch sự.", "Dùng Sie viết hoa.", Now);
    private static GrammarPracticeQuestion[] Questions(int count = 5) => Enumerable.Range(0, count).Select(index =>
        GrammarPracticeQuestion.Create(100 + index, Exercise(40 + index), Translation(40 + index, 50 + index), Point, Language, [0, 1, 2, 3])).ToArray();

    public static void Run()
    {
        var input = new[] { " Sie ", "sie", "du", "ihr" };
        var exercise = Exercise(options: input);
        Check.That(exercise.Options[0] == "Sie" && exercise.Options[1] == "sie", "Grammar preserves case distinctions.");
        input[0] = "changed";
        var returned = exercise.Options;
        returned[0] = "changed";
        Check.That(exercise.Options[0] == "Sie", "Exercise owns input and output option arrays.");
        foreach (var prompt in new[] { "", " \t", "plain sentence", "{blank} and {blank}", new string('x', 1001), "a\0{blank}" })
            Check.Throws<ArgumentException>(() => Exercise(prompt: prompt), "Invalid prompt rejected.");
        foreach (var options in new[] { new[] { "one" }, new[] { "a", "b", "c", "d", "e" }, new[] { "a", " a " },
            new[] { "Café", "Cafe\u0301" }, new[] { "", "a" }, new[] { "a\0", "b" }, new[] { new string('a', 251), "b" } })
            Check.Throws<ArgumentException>(() => Exercise(options: options), "Invalid options rejected after normalization.");
        Check.Throws<ArgumentOutOfRangeException>(() => GrammarExercise.Create(1, 30, "test", "{blank}", ["a", "b"], 2, true, 0, Now), "Correct index bounded.");
        Check.Throws<ArgumentOutOfRangeException>(() => GrammarExercise.Create(1, 0, "test", "{blank}", ["a", "b"], 0, true, 0, Now), "Source ID positive.");
        Check.Throws<ArgumentException>(() => GrammarExerciseTranslation.Create(1, 40, "vi_vi", "Instruction", "Explanation", Now), "Locale validated.");
        Check.Throws<ArgumentException>(() => GrammarExerciseTranslation.Create(1, 40, "vi", " ", "Explanation", Now), "Instruction required.");
        Check.Throws<ArgumentException>(() => GrammarExerciseTranslation.Create(1, 40, "vi", "Instruction", new string('a', 4001), Now), "Explanation bounded.");
        var translation = Translation();
        var snapshot = GrammarPracticeQuestion.Create(100, exercise, translation, Point, Language, [2, 0, 3, 1]);
        Check.That(snapshot.CorrectOptionIndex == 1 && snapshot.Options[1] == "Sie", "Permutation preserves the correct answer.");
        foreach (var order in new[] { new[] { 0, 1 }, new[] { 0, 1, 2, 2 }, new[] { 0, 1, 2, 4 }, new[] { -1, 1, 2, 3 } })
            Check.Throws<ArgumentException>(() => GrammarPracticeQuestion.Create(100, exercise, translation, Point, Language, order), "Invalid permutation rejected.");
        Check.Throws<ArgumentException>(() => GrammarPracticeQuestion.Create(100, exercise, Translation(99), Point, Language, [0, 1, 2, 3]), "Translation ownership enforced.");
        var otherLanguage = Language.Create(21, "ja", "日本語", true, 0, Now);
        Check.Throws<ArgumentException>(() => GrammarPracticeQuestion.Create(100, exercise, translation, Point, otherLanguage, [0, 1, 2, 3]), "Cross-language question rejected.");
        var before = JsonSerializer.Serialize(exercise);
        Check.Throws<ArgumentException>(() => exercise.Update("no blank", ["a", "b"], 0, false, 0, Now.AddMinutes(1)), "Invalid update rejected.");
        Check.That(before == JsonSerializer.Serialize(exercise), "Invalid update leaves every field intact.");
        exercise.Update("New {blank}.", ["a", "b"], 1, false, 1, Now.AddMinutes(1));
        translation.Update("New instruction", "New explanation", Now.AddMinutes(1));
        Check.That(snapshot.Prompt == "Frau Müller, kommen {blank}?" && snapshot.Options[1] == "Sie" &&
            snapshot.Explanation == "Dùng Sie viết hoa." && snapshot.ExerciseVersion == 1 && snapshot.TranslationVersion == 1,
            "Source edits cannot alter captured question content or provenance.");
        Check.Throws<ArgumentException>(() => GrammarPracticeQuestion.Create(101, exercise, translation, Point, Language, [0, 1]), "Unpublished exercise rejected.");
        var questions = Questions();
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(200, Profile("ja"), Language, questions, Now), "Profile language enforced.");
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(200, Profile(), Language, Questions(4), Now), "Minimum five questions.");
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(200, Profile(), Language, Questions(11), Now), "Maximum ten questions.");
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(100, Profile(), Language, questions, Now), "Root/item IDs distinct.");
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(200, Profile(), Language, [questions[0], questions[0], questions[2], questions[3], questions[4]], Now), "No duplicate exercise snapshots.");
        Check.Throws<ArgumentException>(() => GrammarPracticeSession.Create(200, Profile(), Language, questions, Now.AddSeconds(-1)), "Session cannot predate sources.");
        var session = GrammarPracticeSession.Create(200, Profile(), Language, questions, Now, Point);
        Check.That(session.Version == 1 && session.Items.Count == 5 && session.ExplanationLanguage == "vi", "Initial aggregate state.");
        before = JsonSerializer.Serialize(session);
        Check.Throws<ArgumentOutOfRangeException>(() => session.Answer(100, 4, Now), "Invalid selected index rejected.");
        Check.That(before == JsonSerializer.Serialize(session), "Rejected answer cannot mutate the aggregate.");
        session.Answer(100, 0, Now.AddMinutes(1));
        before = JsonSerializer.Serialize(session);
        session.Answer(100, 0, Now.AddMinutes(2));
        Check.That(before == JsonSerializer.Serialize(session), "Identical answer retry is idempotent.");
        Check.Throws<InvalidOperationException>(() => session.Answer(100, 1, Now.AddMinutes(2)), "Recorded answer cannot change.");
        foreach (var question in questions.Skip(1)) session.Answer(question.Id, 0, Now.AddMinutes(2));
        Check.That(session.Status == GrammarPracticeSessionStatus.Completed && session.EndedAt == Now.AddMinutes(2) &&
            session.Version == 6 && session.Items.All(item => item.IsCorrect == true && item.Version == 2), "All answers complete the session exactly once.");
        Check.Throws<InvalidOperationException>(() => session.Abandon(Now.AddMinutes(3)), "Completed session cannot be abandoned.");
        var abandoned = GrammarPracticeSession.Create(201, Profile(), Language, Questions(10), Now);
        abandoned.Abandon(Now.AddMinutes(1));
        before = JsonSerializer.Serialize(abandoned);
        abandoned.Abandon(Now.AddMinutes(2));
        Check.That(before == JsonSerializer.Serialize(abandoned), "Repeated abandon is idempotent.");
        Check.Throws<InvalidOperationException>(() => abandoned.Answer(100, 0, Now.AddMinutes(3)), "Abandoned session rejects new answers.");
        var overflow = GrammarPracticeSession.Create(202, Profile(), Language, Questions(), Now);
        typeof(GrammarPracticeSession).GetProperty("Version")!.SetValue(overflow, long.MaxValue);
        before = JsonSerializer.Serialize(overflow);
        Check.Throws<OverflowException>(() => overflow.Answer(100, 0, Now), "Overflow rejected before mutation.");
        Check.That(before == JsonSerializer.Serialize(overflow), "Overflow preserves root and item states.");
    }
}
