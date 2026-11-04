using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Handwriting.Models;
using Domain.Handwriting;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Handwriting.Tests;

internal static class PersistenceRaceChecks
{
    public static async Task RunAsync(IServiceProvider services, HandwritingTemplateDto template,
        HandwritingAttemptDto existing, JsonSerializerOptions json)
    {
        var templateId = long.Parse(template.Template.Reference.Id);
        var itemId = long.Parse(template.Template.Reference.WritingSystemItemId);
        var systemId = long.Parse(existing.Attempt.WritingSystemId);
        var mutations = new[]
        {
            ("ownership", "UPDATE learning_profiles SET user_id = 9200 WHERE id = 9001", "UPDATE learning_profiles SET user_id = 9000 WHERE id = 9001"),
            ("template", $"UPDATE handwriting_templates SET is_published = false, version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {templateId}",
                $"UPDATE handwriting_templates SET is_published = true, version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {templateId}"),
            ("item", $"UPDATE writing_system_items SET symbol = 'changed', version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {itemId}",
                $"UPDATE writing_system_items SET symbol = 'あ', version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {itemId}"),
            ("system", $"UPDATE writing_systems SET is_published = false, version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {systemId}",
                $"UPDATE writing_systems SET is_published = true, version = version + 1, updated_at = updated_at + interval '1 second' WHERE id = {systemId}"),
            ("language", "UPDATE languages SET is_enabled = false, version = version + 1, updated_at = updated_at + interval '1 second' WHERE code = 'ja'",
                "UPDATE languages SET is_enabled = true, version = version + 1, updated_at = updated_at + interval '1 second' WHERE code = 'ja'")
        };
        static HandwritingPracticeAttempt Attempt(long id, long childId, HandwritingCatalog catalog)
        {
            var reference = catalog.Items.Single();
            var feedback = HandwritingFeedback.Evaluated(reference.Template.Drawing.Strokes.Count,
                Enumerable.Range(0, reference.Template.Drawing.Strokes.Count).Select(i => new HandwritingStrokeFeedback(i, 100, 0, 1, [])));
            return HandwritingPracticeAttempt.Create(id, catalog.Profile, catalog.WritingSystem, catalog.Language, "geometry-v1",
                [new(childId, reference.Template, reference.Item, reference.Template.Drawing, feedback)], new TestClock().GetUtcNow());
        }
        var key = 9400;
        foreach (var (name, mutate, restore) in mutations)
        {
            await using var scope = services.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IHandwritingRepository>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var catalog = (await repository.GetCatalogAsync(9000, 9001, "hiragana", [templateId], default))!;
            var attempt = Attempt(key, key + 1, catalog);
            await db.Database.ExecuteSqlRawAsync(mutate);
            try
            {
                try { await repository.CreateAsync(9000, attempt, catalog, default); throw new InvalidOperationException($"Race accepted: {name}"); }
                catch (ConflictException) { Check.That(true, $"Write rechecks {name} after grading."); }
                Check.That(!await db.HandwritingPracticeAttempts.AnyAsync(x => x.Id == key) &&
                    !await db.HandwritingPracticeCharacters.AnyAsync(x => x.Id == key + 1), $"No orphan state after {name} race.");
                var history = await repository.GetAttemptAsync(9000, 9001, long.Parse(existing.Attempt.Id), default);
                if (name == "ownership") Check.That(history is null, "History ownership is checked at read time.");
                else Check.That(history is not null && JsonSerializer.Serialize(history, json) == JsonSerializer.Serialize(existing, json),
                    $"Stored history remains identical after {name} changes.");
            }
            finally { await db.Database.ExecuteSqlRawAsync(restore); }
            key += 10;
        }
        await using (var scope = services.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IHandwritingRepository>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var catalog = (await repository.GetCatalogAsync(9000, 9001, "hiragana", [templateId], default))!;
            var attempt = Attempt(9490, long.Parse(existing.Characters[0].Id), catalog);
            try { await repository.CreateAsync(9000, attempt, catalog, default); throw new InvalidOperationException("Duplicate child accepted."); }
            catch (ConflictException) { Check.That(true, "Child insert conflict is mapped to 409."); }
            Check.That(!await db.HandwritingPracticeAttempts.AnyAsync(x => x.Id == 9490), "Explicit transaction rolls back the conditional root insert when a child fails.");
        }
    }
}
