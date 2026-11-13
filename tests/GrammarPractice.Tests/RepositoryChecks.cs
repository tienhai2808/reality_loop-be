using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.GrammarPractice;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrammarPractice.Tests;

internal static class RepositoryChecks
{
    public static async Task RunAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IGrammarPracticeRepository>();
        var ids = scope.ServiceProvider.GetRequiredService<IIdGenerator>();
        var now = services.GetRequiredService<TestClock>().GetUtcNow();
        async Task<GrammarPracticeSession> NewSession(long? firstItemId = null)
        {
            var context = (await repository.GetContextAsync(7000, 7001, "practice-small", default))!;
            var candidates = await repository.SampleAsync(context, 5, default);
            var questions = candidates.Select((candidate, index) => GrammarPracticeQuestion.Create(
                index == 0 && firstItemId.HasValue ? firstItemId.Value : ids.NewId(), candidate.Exercise,
                candidate.Translation, candidate.GrammarPoint, context.Language, [0, 1])).ToArray();
            return GrammarPracticeSession.Create(ids.NewId(), context.Profile, context.Language, questions, now,
                context.SelectedPoint);
        }
        async Task AssertAbsent(GrammarPracticeSession session)
        {
            db.ChangeTracker.Clear();
            Check.That(!await db.GrammarPracticeSessions.AnyAsync(root => root.Id == session.Id) &&
                !await db.GrammarPracticeSessionItems.AnyAsync(item => item.SessionId == session.Id), "Failed creation leaves no root or child rows.");
        }

        var forbidden = await NewSession();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(8000, forbidden, default),
            "Create rechecks ownership at the write.");
        await AssertAbsent(forbidden);

        var staleExercise = await NewSession();
        var sourceId = staleExercise.Items.First().GrammarExerciseId;
        var exercise = await db.GrammarExercises.SingleAsync(exercise => exercise.Id == sourceId);
        exercise.Update(exercise.Prompt, exercise.Options, exercise.CorrectOptionIndex, true, exercise.SortOrder, now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, staleExercise, default),
            "Creation rejects a source exercise changed after sampling.");
        await AssertAbsent(staleExercise);

        var staleTranslation = await NewSession();
        sourceId = staleTranslation.Items.First().GrammarExerciseId;
        var translation = await db.GrammarExerciseTranslations.SingleAsync(translation => translation.GrammarExerciseId == sourceId);
        translation.Update(translation.Instruction, "A revised explanation.", now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, staleTranslation, default),
            "Creation rejects a translation changed after sampling.");
        await AssertAbsent(staleTranslation);

        var staleProfile = await NewSession();
        var profile = await db.LearningProfiles.SingleAsync(profile => profile.Id == 7001);
        profile.UpdatePreferences(profile.NativeLanguage, "en", profile.ProficiencyLevel, profile.PrimaryGoal, null, now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, staleProfile, default),
            "Creation rechecks the profile explanation language.");
        profile.UpdatePreferences(profile.NativeLanguage, "vi", profile.ProficiencyLevel, profile.PrimaryGoal, null, now);
        await db.SaveChangesAsync();
        await AssertAbsent(staleProfile);

        var unpublished = await NewSession();
        sourceId = unpublished.Items.First().GrammarExerciseId;
        exercise = await db.GrammarExercises.SingleAsync(exercise => exercise.Id == sourceId);
        exercise.Update(exercise.Prompt, exercise.Options, exercise.CorrectOptionIndex, false, exercise.SortOrder, now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, unpublished, default),
            "Creation rechecks exercise publication.");
        exercise.Update(exercise.Prompt, exercise.Options, exercise.CorrectOptionIndex, true, exercise.SortOrder, now);
        await db.SaveChangesAsync();
        await AssertAbsent(unpublished);

        var unpublishedPoint = await NewSession();
        var point = await db.GrammarPoints.SingleAsync(point => point.Id == 7100);
        point.Update(point.NativeName, point.Pattern, false, point.SortOrder, now);
        await db.SaveChangesAsync();
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, unpublishedPoint, default),
            "Creation rechecks grammar point publication after sampling.");
        point.Update(point.NativeName, point.Pattern, true, point.SortOrder, now);
        await db.SaveChangesAsync();
        await AssertAbsent(unpublishedPoint);

        var session = await NewSession();
        await repository.CreateAsync(7000, session, default);
        db.ChangeTracker.Clear();
        var broken = await NewSession(session.Items.First().Id);
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, broken, default),
            "A child identifier collision rejects the insert batch.");
        await AssertAbsent(broken);
        Check.That(await db.GrammarPracticeSessionItems.CountAsync(item => item.SessionId == session.Id) == 5,
            "Failed creation preserves the other session's children.");
        await Check.ThrowsAsync<ConflictException>(() => repository.CreateAsync(7000, session, default),
            "A duplicate root identifier is mapped to a conflict.");
        db.ChangeTracker.Clear();

        var snapshot = (await repository.FindOwnedAsync(7000, 7001, session.Id, default))!;
        var item = snapshot.Items.First();
        snapshot.Answer(item.Id, 0, now);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(8000, snapshot, item, 1, default),
            "Answer checks ownership again in the root CAS.");
        var stale = (await repository.FindOwnedAsync(7000, 7001, session.Id, default))!;
        await repository.SaveAnswerAsync(7000, snapshot, item, 1, default);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(7000, stale, stale.Items.First(), 1, default),
            "A stale no-op cannot claim a current snapshot.");
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(7000, snapshot, item, 1, default),
            "Root CAS rejects an already consumed version.");
        var abandon = (await repository.FindOwnedAsync(7000, 7001, session.Id, default))!;
        abandon.Abandon(now);
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAbandonAsync(8000, abandon, 2, default),
            "Abandon rechecks ownership at its conditional update.");

        var childRace = await NewSession();
        await repository.CreateAsync(7000, childRace, default);
        db.ChangeTracker.Clear();
        var childSnapshot = (await repository.FindOwnedAsync(7000, 7001, childRace.Id, default))!;
        var child = childSnapshot.Items.First();
        childSnapshot.Answer(child.Id, 0, now);
        // Simulate a child-only writer bypassing the aggregate to exercise the second CAS failure.
        await db.GrammarPracticeSessionItems.Where(item => item.Id == child.Id).ExecuteUpdateAsync(setters => setters
            .SetProperty(item => item.SelectedOptionIndex, (int?)1).SetProperty(item => item.AnsweredAt, (DateTimeOffset?)now)
            .SetProperty(item => item.UpdatedAt, now).SetProperty(item => item.Version, 2L));
        await Check.ThrowsAsync<ConflictException>(() => repository.SaveAnswerAsync(7000, childSnapshot, child, 1, default),
            "A failed child CAS rejects the entire answer transaction.");
        Check.That(await db.GrammarPracticeSessions.Where(root => root.Id == childRace.Id).Select(root => root.Version).SingleAsync() == 1,
            "Child CAS failure rolls back the successful root CAS.");
        Check.That(await db.GrammarPracticeSessionItems.Where(item => item.Id == child.Id).Select(item => item.SelectedOptionIndex).SingleAsync() == 1,
            "The failed writer leaves the existing answer intact.");
    }
}
