using System.Text.Json;
using Infrastructure.Storage.R2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Seeding.Audio;

public sealed class FoundationAudioInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly R2FoundationAudioUploader _uploader;
    private readonly FoundationAudioSeedOptions _options;
    private readonly TimeProvider _clock;
    private readonly ILogger<FoundationAudioInitializer> _logger;

    internal FoundationAudioInitializer(ApplicationDbContext db, R2FoundationAudioUploader uploader,
        IOptions<FoundationAudioSeedOptions> options, TimeProvider clock, ILogger<FoundationAudioInitializer> logger)
    {
        _db = db;
        _uploader = uploader;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct)
    {
        var entries = FoundationAudioCatalog.Load();
        var publicRoot = _options.PublicBaseUrl.TrimEnd('/');
        var recordedReadings = entries.SelectMany(entry => entry.ReadingIds.Select(id => (Id: id, Entry: entry)))
            .ToDictionary(target => target.Id, target => target.Entry);
        var recordedExamples = entries.SelectMany(entry => entry.ExampleIds.Select(id => (Id: id, Entry: entry)))
            .ToDictionary(target => target.Id, target => target.Entry);
        var expectedReadings = CatalogSeedData.WritingSystemItemReadingRows.ToDictionary(row => row.Id);
        var expectedExamples = CatalogSeedData.PronunciationExampleRows.ToDictionary(row => row.Id);
        await _db.Database.MigrateAsync(ct);
        _logger.LogInformation("Uploading/verifying {Count} foundation recordings in R2.", entries.Length);
        await Parallel.ForEachAsync(entries, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
            async (entry, token) => await _uploader.UploadAsync(entry, token));

        // Uploads finish before taking DB locks. All languages publish their URLs in one transaction.
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        await _db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(72584619033201)", ct);
        var readingIds = recordedReadings.Keys.ToArray();
        var readings = await _db.WritingSystemItemReadings.FromSqlInterpolated($"""
            SELECT * FROM writing_system_item_readings WHERE id = ANY ({readingIds}) ORDER BY id FOR UPDATE
            """).AsNoTracking().ToListAsync(ct);
        var exampleIds = recordedExamples.Keys.ToArray();
        var examples = await _db.PronunciationExamples.FromSqlInterpolated($"""
            SELECT * FROM pronunciation_examples WHERE id = ANY ({exampleIds}) ORDER BY id FOR UPDATE
            """).AsNoTracking().ToListAsync(ct);
        if (readings.Count != readingIds.Length || examples.Count != exampleIds.Length)
            throw new InvalidOperationException("Foundation audio targets are missing. Restore/review the seed before initializing audio.");
        var readingPatches = new List<AudioPatch>();
        var examplePatches = new List<AudioPatch>();
        var preserved = 0;
        var now = _clock.GetUtcNow();
        foreach (var reading in readings)
        {
            var seed = expectedReadings[reading.Id];
            if (reading.WritingSystemItemId != seed.WritingSystemItemId || reading.Code != seed.Code ||
                reading.Transcription != seed.Transcription || reading.TranscriptionSystem != seed.TranscriptionSystem)
                throw new InvalidOperationException($"Reading {reading.Id} differs from the reviewed foundation audio mapping.");
            var entry = recordedReadings[reading.Id];
            var url = $"{publicRoot}/{entry.ObjectKey}";
            if (reading.AudioUrl == url) continue;
            if (reading.AudioUrl is not null && !reading.AudioUrl.EndsWith('/' + entry.ObjectKey, StringComparison.Ordinal))
            {
                preserved++;
                continue;
            }
            var previousVersion = reading.Version;
            reading.Update(reading.Transcription, reading.TranscriptionSystem, url, reading.SortOrder,
                now < reading.UpdatedAt ? reading.UpdatedAt : now);
            readingPatches.Add(new(reading.Id, previousVersion, reading.Version, url, reading.UpdatedAt));
        }
        foreach (var example in examples)
        {
            var seed = expectedExamples[example.Id];
            if (example.PronunciationRuleId != seed.PronunciationRuleId || example.Code != seed.Code ||
                example.Text != seed.Text || example.Transcription != seed.Transcription || example.TranscriptionSystem != seed.TranscriptionSystem)
                throw new InvalidOperationException($"Example {example.Id} differs from the reviewed foundation audio mapping.");
            var entry = recordedExamples[example.Id];
            var url = $"{publicRoot}/{entry.ObjectKey}";
            if (example.AudioUrl == url) continue;
            if (example.AudioUrl is not null && !example.AudioUrl.EndsWith('/' + entry.ObjectKey, StringComparison.Ordinal))
            {
                preserved++;
                continue;
            }
            var previousVersion = example.Version;
            example.Update(example.Text, example.Transcription, example.TranscriptionSystem, url, example.SortOrder,
                now < example.UpdatedAt ? example.UpdatedAt : now);
            examplePatches.Add(new(example.Id, previousVersion, example.Version, url, example.UpdatedAt));
        }
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        if (readingPatches.Count > 0)
        {
            var json = JsonSerializer.Serialize(readingPatches, jsonOptions);
            var updated = await _db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE writing_system_item_readings AS reading
                SET audio_url = seed.audio_url, version = seed.version, updated_at = seed.updated_at
                FROM jsonb_to_recordset(CAST({json} AS jsonb)) AS seed(
                    id bigint, previous_version bigint, version bigint, audio_url text, updated_at timestamptz)
                WHERE reading.id = seed.id AND reading.version = seed.previous_version
                """, ct);
            if (updated != readingPatches.Count) throw new DbUpdateConcurrencyException("Foundation reading audio update conflicted.");
        }
        if (examplePatches.Count > 0)
        {
            var json = JsonSerializer.Serialize(examplePatches, jsonOptions);
            var updated = await _db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE pronunciation_examples AS example
                SET audio_url = seed.audio_url, version = seed.version, updated_at = seed.updated_at
                FROM jsonb_to_recordset(CAST({json} AS jsonb)) AS seed(
                    id bigint, previous_version bigint, version bigint, audio_url text, updated_at timestamptz)
                WHERE example.id = seed.id AND example.version = seed.previous_version
                """, ct);
            if (updated != examplePatches.Count) throw new DbUpdateConcurrencyException("Foundation example audio update conflicted.");
        }
        await transaction.CommitAsync(ct);
        _logger.LogInformation("Foundation audio ready: {Recordings} recordings; {Readings} readings and {Examples} examples updated; {Preserved} custom URLs preserved.",
            entries.Length, readingPatches.Count, examplePatches.Count, preserved);
    }

    private sealed record AudioPatch(long Id, long PreviousVersion, long Version, string AudioUrl, DateTimeOffset UpdatedAt);
}
