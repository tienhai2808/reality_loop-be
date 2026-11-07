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
        // Check the complete offline bundle before making changes to either external system.
        var entries = FoundationAudioCatalog.Load();
        var publicRoot = _options.PublicBaseUrl.TrimEnd('/');
        var entriesByCode = entries.ToDictionary(x => x.ItemCode);
        var urls = entries.ToDictionary(x => x.ItemCode, x => $"{publicRoot}/{x.ObjectKey}");
        await _db.Database.MigrateAsync(ct);

        _logger.LogInformation("Uploading/verifying {Count} foundation recordings in R2.", entries.Length);
        await Parallel.ForEachAsync(entries, new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = ct
        }, async (entry, token) => await _uploader.UploadAsync(entry, token));

        // Never hold database locks across network uploads. Publish URLs only after all uploads succeed.
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        await _db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(72584619033201)", ct);
        // Only attach recordings present in the reviewed bundle. Contracted sounds need their own recordings.
        var recordedItems = FoundationSeedData.WritingSystemItemRows
            .Where(x => entriesByCode.ContainsKey(x.Code)).Select(x => x.Id).ToHashSet();
        var expected = FoundationSeedData.WritingSystemItemReadingRows
            .Where(x => recordedItems.Contains(x.WritingSystemItemId)).ToDictionary(x => x.Id);
        var ids = expected.Keys.ToArray();
        var readings = await _db.WritingSystemItemReadings.FromSqlInterpolated($"""
            SELECT * FROM writing_system_item_readings WHERE id = ANY ({ids}) ORDER BY id FOR UPDATE
            """).AsNoTracking().ToListAsync(ct);
        if (readings.Count != expected.Count)
            throw new InvalidOperationException("Foundation readings are missing. Restore/review the foundation seed before initializing audio.");

        var items = FoundationSeedData.WritingSystemItemRows.ToDictionary(x => x.Id);
        var patches = new List<object>();
        var preserved = 0;
        var now = _clock.GetUtcNow();
        foreach (var reading in readings)
        {
            var seed = expected[reading.Id];
            if (reading.WritingSystemItemId != seed.WritingSystemItemId || reading.Code != seed.Code ||
                reading.Transcription != seed.Transcription || reading.TranscriptionSystem != seed.TranscriptionSystem)
                throw new InvalidOperationException($"Reading {reading.Id} differs from the reviewed foundation mapping; refusing to attach incorrect audio.");

            var code = items[seed.WritingSystemItemId].Code;
            var url = urls[code];
            if (reading.AudioUrl == url) continue;
            var key = entriesByCode[code].ObjectKey;
            if (reading.AudioUrl is not null && !reading.AudioUrl.EndsWith('/' + key, StringComparison.Ordinal))
            {
                preserved++;
                continue;
            }
            var previousVersion = reading.Version;
            reading.Update(reading.Transcription, reading.TranscriptionSystem, url, reading.SortOrder,
                now < reading.UpdatedAt ? reading.UpdatedAt : now);
            patches.Add(new
            {
                id = reading.Id,
                previous_version = previousVersion,
                version = reading.Version,
                audio_url = reading.AudioUrl,
                updated_at = reading.UpdatedAt
            });
        }

        if (patches.Count > 0)
        {
            var json = JsonSerializer.Serialize(patches);
            var updated = await _db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE writing_system_item_readings AS reading
                SET audio_url = seed.audio_url, version = seed.version, updated_at = seed.updated_at
                FROM jsonb_to_recordset(CAST({json} AS jsonb)) AS seed(
                    id bigint, previous_version bigint, version bigint, audio_url text, updated_at timestamptz)
                WHERE reading.id = seed.id AND reading.version = seed.previous_version
                """, ct);
            if (updated != patches.Count)
                throw new DbUpdateConcurrencyException("Foundation audio update conflicted with another writer.");
        }
        // A syllable recording also applies to an example containing exactly that syllable.
        var examplesById = PronunciationSeedData.ExampleRows.ToDictionary(x => x.Id);
        var recordingsBySymbol = entries.SelectMany(entry => new[]
        {
            (Symbol: entry.Hiragana, Entry: entry), (Symbol: entry.Katakana, Entry: entry)
        }).ToDictionary(x => x.Symbol, x => x.Entry);
        var exampleIds = examplesById.Values.Where(example => recordingsBySymbol.ContainsKey(example.Text))
            .Select(example => example.Id).ToArray();
        var examples = await _db.PronunciationExamples.FromSqlInterpolated($"""
            SELECT * FROM pronunciation_examples WHERE id = ANY ({exampleIds}) ORDER BY id FOR UPDATE
            """).ToListAsync(ct);
        foreach (var example in examples)
        {
            var seed = examplesById[example.Id];
            if (example.Text != seed.Text || example.Transcription != seed.Transcription ||
                example.TranscriptionSystem != "hepburn") continue;
            var entry = recordingsBySymbol[example.Text];
            if (example.Transcription != entry.Reading) continue;
            var url = urls[entry.ItemCode];
            if (example.AudioUrl == url) continue;
            if (example.AudioUrl is not null && !example.AudioUrl.EndsWith('/' + entry.ObjectKey, StringComparison.Ordinal))
                continue;
            example.Update(example.Text, example.Transcription, example.TranscriptionSystem, url, example.SortOrder,
                now < example.UpdatedAt ? example.UpdatedAt : now);
        }
        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        _logger.LogInformation("Foundation audio ready: {Recordings} recordings; {Updated} readings updated; {Preserved} custom audio URLs preserved.",
            entries.Length, patches.Count, preserved);
    }
}
