using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeding.Audio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grammar.Tests;

internal static class SeedAudioChecks
{
    public static async Task RunAsync(string database)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Database"] = database,
            ["Storage:R2:ServiceUrl"] = "https://00000000000000000000000000000000.r2.cloudflarestorage.com",
            ["Storage:R2:BucketName"] = "seed-tests",
            ["Storage:R2:AccessKeyId"] = "test-only",
            ["Storage:R2:SecretAccessKey"] = "test-only",
            ["FoundationAudioSeed:PublicBaseUrl"] = "https://pub-seed-tests.r2.dev"
        }).Build();
        using var storage = new MemoryS3Client();
        var services = new ServiceCollection().AddLogging().AddSingleton<IConfiguration>(configuration);
        services.AddFoundationAudioInitialization(configuration);
        services.AddSingleton<IAmazonS3>(storage);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var initializer = scope.ServiceProvider.GetRequiredService<FoundationAudioInitializer>();
        async Task<string> Snapshot() => JsonSerializer.Serialize(new
        {
            Readings = await db.WritingSystemItemReadings.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new { x.Id, x.AudioUrl, x.Version }).ToArrayAsync(),
            Examples = await db.PronunciationExamples.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new { x.Id, x.AudioUrl, x.Version }).ToArrayAsync()
        });
        await initializer.InitializeAsync(CancellationToken.None);
        Check.That(storage.ObjectCount == 71, "Initializer uploads all 71 verified embedded recordings.");
        Check.That(await db.WritingSystemItemReadings.CountAsync(x => x.AudioUrl != null) == 142,
            "Only the 142 kana with real recordings receive audio.");
        Check.That(await db.WritingSystemItemReadings.CountAsync(x => x.AudioUrl == "https://example.invalid/custom-kana.mp3") == 1,
            "Initializer preserves customized audio on an existing reading.");
        Check.That(await db.PronunciationExamples.CountAsync(x => x.AudioUrl != null) == 5,
            "Exact one-kana examples share the correct recordings.");
        Check.That(!await db.VocabularyEntries.AnyAsync(x => x.AudioUrl != null), "Kana audio is never attached to vocabulary words.");
        var first = await Snapshot();
        await initializer.InitializeAsync(CancellationToken.None);
        Check.That(await Snapshot() == first && storage.MetadataReads == 71,
            "Repeated initialization verifies existing objects and leaves every DB version unchanged.");
        storage.FailUploads = true;
        try
        {
            await initializer.InitializeAsync(CancellationToken.None);
            throw new InvalidOperationException("An upload failure must abort initialization.");
        }
        catch (IOException) { Check.That(true, "Storage failure propagates."); }
        Check.That(await Snapshot() == first, "Failed upload does not publish partial URLs or advance versions.");
    }

    private sealed class MemoryS3Client() : AmazonS3Client("test-only", "test-only",
        new AmazonS3Config { ServiceURL = "https://storage.invalid", AuthenticationRegion = "auto" })
    {
        private readonly ConcurrentDictionary<string, (long Bytes, string Hash)> _objects = new();
        private int _metadataReads;
        public int ObjectCount => _objects.Count;
        public int MetadataReads => _metadataReads;
        public bool FailUploads { get; set; }

        public override async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken cancellationToken = default)
        {
            if (FailUploads) throw new IOException("Simulated storage failure.");
            using var bytes = new MemoryStream();
            await request.InputStream.CopyToAsync(bytes, cancellationToken);
            var hash = Convert.ToHexStringLower(SHA256.HashData(bytes.ToArray()));
            if (request.IfNoneMatch != "*" || hash != request.Metadata["sha256"])
                throw new InvalidOperationException("Upload must preserve immutable bytes and checksum.");
            if (!_objects.TryAdd(request.Key, (bytes.Length, hash)))
                throw new AmazonS3Exception("Object already exists.") { StatusCode = HttpStatusCode.PreconditionFailed };
            return new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
        }

        public override Task<GetObjectMetadataResponse> GetObjectMetadataAsync(GetObjectMetadataRequest request,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _metadataReads);
            var stored = _objects[request.Key];
            var response = new GetObjectMetadataResponse { ContentLength = stored.Bytes, HttpStatusCode = HttpStatusCode.OK };
            response.Metadata["x-amz-meta-sha256"] = stored.Hash;
            return Task.FromResult(response);
        }
    }
}
