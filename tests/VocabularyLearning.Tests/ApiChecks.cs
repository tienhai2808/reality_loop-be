using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.VocabularyLearning.Models;
using Application.VocabularyLearning.Services;
using Domain.LearningProfiles;
using Domain.Users;
using Domain.Vocabulary;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VocabularyLearning.Tests;

internal static class ApiChecks
{
    public static async Task RunAsync(string database)
    {
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
        await using var app = TestApiHost.Build(database);
        var counter = app.Services.GetRequiredService<SqlCommandCounter>();
        var clock = (TestClock)app.Services.GetRequiredService<TimeProvider>();
        const string root = "/api/v1/learning-profiles/9001";
        const string queue = root + "/vocabulary-learning";
        long[] senseIds;
        long entryId;
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.AddRange(User.Create(9000, "a@vocab-learning.invalid", "A", "Test", null, clock.GetUtcNow()),
                User.Create(9100, "b@vocab-learning.invalid", "B", "Test", null, clock.GetUtcNow()),
                User.Create(9200, "c@vocab-learning.invalid", "C", "Test", null, clock.GetUtcNow()));
            db.LearningProfiles.AddRange(
                LearningProfile.Create(9001, 9000, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, clock.GetUtcNow()),
                LearningProfile.Create(9002, 9000, "en", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, clock.GetUtcNow()),
                LearningProfile.Create(9101, 9100, "ja", "vi", "vi", ProficiencyLevel.Beginner, LearningGoal.Travel, null, clock.GetUtcNow()));
            entryId = (await db.VocabularyEntries.SingleAsync(x => x.Code == "gohan")).Id;
            senseIds = await db.VocabularySenses.Where(x => x.VocabularyEntryId == entryId).OrderBy(x => x.Id).Select(x => x.Id).ToArrayAsync();
            db.VocabularySenses.AddRange(VocabularySense.Create(9005, entryId, "no-translation", PartOfSpeech.Noun, 10, clock.GetUtcNow()),
                VocabularySense.Create(9006, entryId, "race", PartOfSpeech.Noun, 11, clock.GetUtcNow()),
                VocabularySense.Create(9007, entryId, "add-race", PartOfSpeech.Noun, 12, clock.GetUtcNow()));
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var client = TestApiHost.Client(app);
        async Task<HttpResponseMessage> Send(string method, string path, object? body = null, int status = 200,
            int queries = 1, string user = "a", string? key = null)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            if (user != "none") request.Headers.Authorization = new("Bearer", "vocabulary-learning-test-" + user);
            key ??= Guid.NewGuid().ToString("N");
            request.Headers.Add("X-Test-Query", key);
            if (body is string raw) request.Content = new StringContent(raw, Encoding.UTF8, "application/json");
            else if (body is not null) request.Content = JsonContent.Create(body);
            var response = await client.SendAsync(request);
            if ((int)response.StatusCode != status) throw new InvalidOperationException($"{method} {path}: expected {status}, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            Check.That(true, "HTTP status.");
            Check.That(counter.Count(key) == queries, $"Expected {queries} queries, got {counter.Count(key)}: {counter.Sql(key)}");
            return response;
        }
        async Task<T> Read<T>(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<T>(json))!;
        string Add(long sense) => root + "/vocabulary-senses/" + sense + "/learning-progress";
        object Rating(Guid id, long version, string rating) => new { requestId = id, version, rating };
        foreach (var path in new[] { queue, queue + "/summary", queue + "/1", queue + "/1/reviews" })
            await Send("GET", path, status: 401, queries: 0, user: "none");
        await Send("PUT", Add(senseIds[0]), status: 401, queries: 0, user: "none");
        await Send("POST", queue + "/1/reviews", Rating(Guid.NewGuid(), 1, "Good"), 401, 0, "none");
        var initialSummary = await Read<VocabularyLearningSummaryDto>(await Send("GET", queue + "/summary"));
        Check.That(initialSummary.Total == 0 && initialSummary.Due == 0 && initialSummary.NextFutureReviewAt is null, "Empty owned summary is retained by left join.");
        await Send("GET", queue, queries: 2);
        var addedResponse = await Send("PUT", Add(senseIds[0]), status: 201, queries: 2);
        var first = await Read<VocabularyProgressDto>(addedResponse);
        var cardPath = queue + "/" + first.Id;
        var reviewPath = cardPath + "/reviews";
        Check.That(first.Status == Domain.VocabularyLearning.VocabularyLearningStatus.New && first.Version == 1 &&
            first.NextReviewAt == clock.GetUtcNow() && addedResponse.Headers.Location?.ToString().EndsWith(cardPath) == true, "Add creates a due New card and correct Location.");
        var repeated = await Read<VocabularyProgressDto>(await Send("PUT", Add(senseIds[0])));
        Check.That(JsonSerializer.Serialize(first, json) == JsonSerializer.Serialize(repeated, json), "Repeated PUT never resets progress.");
        var second = await Read<VocabularyProgressDto>(await Send("PUT", Add(senseIds[1]), status: 201, queries: 2));
        Check.That(first.Id != second.Id && first.VocabularySenseId != second.VocabularySenseId, "Different meanings of the same word are separate cards.");
        var card = await Read<VocabularyFlashcardDto>(await Send("GET", cardPath));
        Check.That(card.Meaning is not null && card.TranslationLocale == "vi" && !card.IsTranslationFallback, "Profile explanation language is the default answer locale.");
        var fallback = await Read<VocabularyFlashcardDto>(await Send("GET", cardPath + "?locale=fr-ca"));
        Check.That(fallback.Meaning is not null && fallback.TranslationLocale == "en" && fallback.IsTranslationFallback, "Exact locale then English fallback.");
        var page1 = await Read<VocabularyLearningPage>(await Send("GET", queue + "?pageSize=1", key: "queue-sql"));
        clock.Advance(TimeSpan.FromHours(1));
        var page2 = await Read<VocabularyLearningPage>(await Send("GET", queue + "?pageSize=1&after=" + page1.NextCursor));
        Check.That(page1.Items.Single().Progress.Id != page2.Items.Single().Progress.Id && page2.NextCursor is null && page1.AsOf == page2.AsOf, "Composite cursor handles tied due dates and pins due cutoff across pages.");
        Check.That(counter.Sql("queue-sql").Contains("LIMIT", StringComparison.OrdinalIgnoreCase) && !counter.Sql("queue-sql").Contains("OFFSET", StringComparison.OrdinalIgnoreCase), "Page is bounded in SQL.");
        await Send("GET", reviewPath, queries: 2);
        await Send("PUT", Add(senseIds[0]), status: 404, user: "b");
        await Send("GET", cardPath, status: 404, user: "b");
        await Send("GET", reviewPath, status: 404, queries: 2, user: "b");
        await Send("GET", queue, status: 404, queries: 2, user: "b");
        await Send("GET", queue + "/summary", status: 404, user: "b");
        await Send("POST", reviewPath, Rating(Guid.NewGuid(), 1, "Good"), 404, user: "b");
        await Send("PUT", Add(senseIds[0]).Replace("9001", "9002"), status: 404);
        await Send("PUT", Add(999999), status: 404);
        var requestId = Guid.NewGuid();
        var receipt = await Read<VocabularyReviewDto>(await Send("POST", reviewPath, Rating(requestId, 1, "Good"), queries: 2));
        Check.That(receipt.ProgressVersionAfter == 2 && receipt.IntervalMinutes == 1440 && receipt.NextReviewAt == clock.GetUtcNow().AddDays(1), "Server schedules Good and records exact before/after versions.");
        var replay = await Read<VocabularyReviewDto>(await Send("POST", reviewPath, Rating(requestId, 1, "Good")));
        Check.That(JsonSerializer.Serialize(receipt, json) == JsonSerializer.Serialize(replay, json), "Same request returns byte-equivalent DTO, not a newly computed schedule.");
        await Send("POST", reviewPath, Rating(requestId, 1, "Easy"), 409);
        await Send("POST", reviewPath, Rating(requestId, 2, "Good"), 409);
        await Send("POST", reviewPath, Rating(Guid.NewGuid(), 1, "Good"), 409);
        var summary = await Read<VocabularyLearningSummaryDto>(await Send("GET", queue + "/summary", key: "summary-sql"));
        Check.That(summary.Total == 2 && summary.New == 1 && summary.Reviewing == 1 && summary.Due == 1 && summary.NextFutureReviewAt == receipt.NextReviewAt, "Summary reflects active cards and upcoming schedule in one SQL aggregate.");
        var due = await Read<VocabularyLearningPage>(await Send("GET", queue));
        Check.That(due.Items.Count == 1 && due.Items[0].Progress.Id == second.Id, "Reviewed future card leaves the due queue.");
        Check.That((await Read<VocabularyLearningPage>(await Send("GET", queue + "?dueOnly=false"))).Items.Count == 2, "All-added view includes future schedules.");
        var again = await Read<VocabularyReviewDto>(await Send("POST", reviewPath, Rating(Guid.NewGuid(), 2, "Again"), queries: 2));
        Check.That(again.AgainCount == 1 && again.ConsecutiveRecallCount == 0 && again.IntervalMinutes == 1, "Again resets recall streak and schedules relearning.");
        var oldReplay = await Read<VocabularyReviewDto>(await Send("POST", reviewPath, Rating(requestId, 1, "Good")));
        Check.That(oldReplay.Id == receipt.Id && oldReplay.ProgressVersionAfter == 2, "Replay remains the original event after later reviews.");
        clock.Advance(TimeSpan.FromMinutes(2));
        Check.That((await Read<VocabularyLearningPage>(await Send("GET", queue))).Items.Count == 2, "Due queue becomes available as server clock advances.");
        var history = await Read<VocabularyReviewPage>(await Send("GET", reviewPath + "?pageSize=1"));
        var older = await Read<VocabularyReviewPage>(await Send("GET", reviewPath + "?pageSize=1&before=" + history.NextCursor));
        Check.That(history.Items[0].ReviewNumber == 2 && older.Items[0].ReviewNumber == 1 && older.NextCursor is null, "History keyset order is descending review number.");
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "dueOnly=maybe", "status=Unknown", "status=99", "after=bad", "after=1:1:0", "locale=en_US" })
            await Send("GET", queue + "?" + query, status: 400, queries: 0);
        foreach (var query in new[] { "pageSize=0", "pageSize=101", "before=0", "before=-1", "before=bad" })
            await Send("GET", reviewPath + "?" + query, status: 400, queries: 0);
        foreach (var body in new[] { "{}", "null", "{\"requestId\":\"00000000-0000-0000-0000-000000000000\",\"version\":1,\"rating\":\"Good\"}",
                     "{\"requestId\":\"bad\",\"version\":1,\"rating\":\"Good\"}",
                     $$"""{"requestId":"{{Guid.NewGuid()}}","version":0,"rating":"Good"}""",
                     $$"""{"requestId":"{{Guid.NewGuid()}}","version":1,"rating":1}""",
                     $$"""{"requestId":"{{Guid.NewGuid()}}","version":"1","rating":"Good"}""",
                     $$"""{"requestId":"{{Guid.NewGuid()}}","version":1,"rating":"Invalid"}""",
                     $$"""{"requestId":"{{Guid.NewGuid()}}","version":1,"rating":"Good","intervalMinutes":1}""" })
            await Send("POST", reviewPath, body, 400, 0);
        await Send("POST", reviewPath, new string(' ', 4097) + "{}", 413, 0);
        // All simultaneous retries must return exactly one persisted receipt.
        var raceCard = await Read<VocabularyProgressDto>(await Send("PUT", Add(9005), status: 201, queries: 2));
        var noMeaning = await Read<VocabularyFlashcardDto>(await Send("GET", queue + "/" + raceCard.Id));
        Check.That(noMeaning.Meaning is null && noMeaning.TranslationLocale is null && noMeaning.IsTranslationFallback, "Missing answers are null, never fabricated.");
        async Task<(int Status, string Json)> ConcurrentPost(string id, object body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, queue + "/" + id + "/reviews");
            request.Headers.Authorization = new("Bearer", "vocabulary-learning-test-a");
            request.Content = JsonContent.Create(body);
            using var response = await client.SendAsync(request);
            return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        var sameKey = Guid.NewGuid();
        var retries = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => ConcurrentPost(raceCard.Id, Rating(sameKey, 1, "Good"))));
        Check.That(retries.All(x => x.Status == 200) && retries.Select(x => x.Json).Distinct().Count() == 1, "Concurrent identical submissions resolve to one receipt.");
        var differentCard = await Read<VocabularyProgressDto>(await Send("PUT", Add(9006), status: 201, queries: 2));
        var races = await Task.WhenAll(ConcurrentPost(differentCard.Id, Rating(Guid.NewGuid(), 1, "Good")),
            ConcurrentPost(differentCard.Id, Rating(Guid.NewGuid(), 1, "Easy")));
        Check.That(races.Select(x => x.Status).Order().SequenceEqual(new[] { 200, 409 }), "Different concurrent ratings on the same version have exactly one winner.");
        var adds = await Task.WhenAll(Enumerable.Range(0, 8).Select(async _ =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, Add(9007));
            request.Headers.Authorization = new("Bearer", "vocabulary-learning-test-a");
            using var response = await client.SendAsync(request);
            return (Status: (int)response.StatusCode, Card: await Read<VocabularyProgressDto>(response));
        }));
        Check.That(adds.Count(x => x.Status == 201) == 1 && adds.Count(x => x.Status == 200) == 7 &&
            adds.Select(x => x.Card.Id).Distinct().Count() == 1 && adds.All(x => x.Card.Version == 1),
            "Concurrent PUT adds one card and returns its unchanged state to every caller.");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var repository = scope.ServiceProvider.GetRequiredService<IVocabularyLearningRepository>();
            var scheduler = scope.ServiceProvider.GetRequiredService<VocabularyReviewScheduler>();
            var raceId = long.Parse(raceCard.Id);
            Check.That(await db.VocabularyReviews.CountAsync(x => x.ProgressId == raceId) == 1 &&
                (await db.VocabularyLearningProgress.SingleAsync(x => x.Id == raceId)).Version == 2, "Replay race does not double-count history or progress.");
            var context = (await repository.GetReviewContextAsync(9000, 9001, raceId, Guid.NewGuid(), default))!;
            var mutation = context.Progress.RecordReview(long.Parse(receipt.Id), Guid.NewGuid(), Domain.VocabularyLearning.VocabularyReviewRating.Good,
                scheduler.Schedule(context.Progress, Domain.VocabularyLearning.VocabularyReviewRating.Good), clock.GetUtcNow());
            try { await repository.SaveReviewAsync(9000, context.Progress, mutation, default); throw new InvalidOperationException("Duplicate log ID accepted."); }
            catch (ConflictException) { Check.That(true, "History insert failure is a conflict."); }
            Check.That((await db.VocabularyLearningProgress.AsNoTracking().SingleAsync(x => x.Id == raceId)).Version == 2, "Single-statement transaction rolls back CAS when history insert fails.");
            var pendingContext = (await repository.GetReviewContextAsync(9000, 9001, raceId, Guid.NewGuid(), default))!;
            var pending = pendingContext.Progress.RecordReview(99999, Guid.NewGuid(), Domain.VocabularyLearning.VocabularyReviewRating.Good,
                scheduler.Schedule(pendingContext.Progress, Domain.VocabularyLearning.VocabularyReviewRating.Good), clock.GetUtcNow());
            await db.VocabularyEntries.Where(x => x.Id == entryId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, false));
            try
            {
                await Send("GET", cardPath, status: 404);
                await Send("POST", reviewPath, Rating(Guid.NewGuid(), 3, "Good"), 404);
                await Send("PUT", Add(senseIds[0]), status: 404);
                await Send("GET", reviewPath);
                var hiddenReplay = await Read<VocabularyReviewDto>(await Send("POST", reviewPath, Rating(requestId, 1, "Good")));
                Check.That(hiddenReplay.Id == receipt.Id, "A committed receipt replays even after content unpublishes.");
                Check.That((await Read<VocabularyLearningSummaryDto>(await Send("GET", queue + "/summary"))).Total == 0, "Hidden content is excluded from active summary.");
                // Read-then-unpublish race must not insert new history or advance the root.
                try { await repository.SaveReviewAsync(9000, pendingContext.Progress, pending, default); throw new InvalidOperationException("Hidden source write accepted."); }
                catch (ConflictException) { Check.That(true, "Publication is checked again at write."); }
            }
            finally { await db.VocabularyEntries.Where(x => x.Id == entryId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsPublished, true)); }
            await db.Database.ExecuteSqlRawAsync("UPDATE learning_profiles SET user_id = 9200 WHERE id = 9001");
            try
            {
                try { await repository.SaveReviewAsync(9000, pendingContext.Progress, pending, default); throw new InvalidOperationException("Ownership race accepted."); }
                catch (ConflictException) { Check.That(true, "Ownership is checked again at the atomic write."); }
                Check.That(!await db.VocabularyReviews.AnyAsync(x => x.Id == 99999) &&
                    (await db.VocabularyLearningProgress.AsNoTracking().SingleAsync(x => x.Id == raceId)).Version == 2,
                    "Failed publication/ownership races leave no root mutation or history row.");
            }
            finally { await db.Database.ExecuteSqlRawAsync("UPDATE learning_profiles SET user_id = 9000 WHERE id = 9001"); }
            Check.That((await db.LearningProfiles.SingleAsync(x => x.Id == 9001)).Version == 1, "Reviews never mutate profile preferences.");
        }
        await app.StopAsync();
    }
}
