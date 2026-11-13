using System.Security.Cryptography;
using System.Text.Json;

namespace Infrastructure.Persistence.Seeding.Audio;

internal static class FoundationAudioCatalog
{
    private const string ResourcePrefix = "FoundationAudio.";

    public static FoundationAudioEntry[] Load()
    {
        var assembly = typeof(FoundationAudioCatalog).Assembly;
        var manifests = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal) && name.EndsWith(".manifest.json", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal);
        var entries = new List<FoundationAudioEntry>();
        var languages = CatalogSeedData.LanguageRows.ToDictionary(row => row.Code);
        var systems = CatalogSeedData.WritingSystemRows.ToDictionary(row => row.Id);
        var items = CatalogSeedData.WritingSystemItemRows.ToDictionary(row => row.Id);
        var readings = CatalogSeedData.WritingSystemItemReadingRows.ToDictionary(row => row.Id);
        var examples = CatalogSeedData.PronunciationExampleRows.ToDictionary(row => row.Id);
        var rules = CatalogSeedData.PronunciationRuleRows.ToDictionary(row => row.Id);
        var recordedReadings = new HashSet<long>();
        var recordedExamples = new HashSet<long>();
        var objectKeys = new HashSet<string>(StringComparer.Ordinal);
        var resources = new HashSet<string>(StringComparer.Ordinal);
        var header = new byte[4];
        foreach (var resource in manifests)
        {
            using var manifestStream = assembly.GetManifestResourceStream(resource)!;
            var manifest = JsonSerializer.Deserialize<FoundationAudioManifest>(manifestStream, JsonSerializerOptions.Web)
                ?? throw new InvalidOperationException("Foundation audio manifest is missing.");
            if (!languages.TryGetValue(manifest.LanguageCode, out var language) ||
                resource != $"{ResourcePrefix}{manifest.LanguageCode}.manifest.json" ||
                string.IsNullOrWhiteSpace(manifest.Bundle) || manifest.ExpectedRecordings <= 0 ||
                manifest.Entries is null || manifest.Entries.Length != manifest.ExpectedRecordings)
                throw new InvalidOperationException($"Invalid foundation audio manifest '{resource}'.");
            foreach (var source in manifest.Entries)
            {
                var entry = source with { LanguageCode = manifest.LanguageCode };
                if (string.IsNullOrWhiteSpace(entry.FileName) || entry.FileName != Path.GetFileName(entry.FileName) ||
                    !entry.FileName.EndsWith(".mp3", StringComparison.Ordinal) ||
                    entry.FileName.Any(c => !char.IsAsciiLetterLower(c) && !char.IsAsciiDigit(c) && c is not '-' and not '.') ||
                    entry.Sha256 is not { Length: 64 } || entry.Sha256.Any(c => !char.IsAsciiHexDigitLower(c)) ||
                    string.IsNullOrEmpty(entry.ObjectKey) || !entry.ObjectKey.StartsWith($"foundation/{manifest.LanguageCode}/", StringComparison.Ordinal) ||
                    !entry.ObjectKey.EndsWith($"-{entry.Sha256[..16]}.mp3", StringComparison.Ordinal) ||
                    entry.ObjectKey.Contains("..", StringComparison.Ordinal) || entry.ObjectKey.Any(c =>
                        !char.IsAsciiLetterLower(c) && !char.IsAsciiDigit(c) && c is not '/' and not '-' and not '.') ||
                    entry.ContentType != "audio/mpeg" || entry.Bytes is < 128 or > 1_048_576 ||
                    string.IsNullOrWhiteSpace(entry.Author) || string.IsNullOrWhiteSpace(entry.License) ||
                    !IsHttps(entry.SourcePage) || !IsHttps(entry.LicenseUrl) ||
                    entry.ReadingIds is not { Length: > 0 } || entry.ExampleIds is null ||
                    !objectKeys.Add(entry.ObjectKey) || !resources.Add(entry.ResourceName))
                    throw new InvalidOperationException($"Invalid foundation audio entry '{entry.ResourceName}'.");
                foreach (var id in entry.ReadingIds)
                    if (!readings.TryGetValue(id, out var reading) || !recordedReadings.Add(id) ||
                        systems[items[reading.WritingSystemItemId].WritingSystemId].LanguageId != language.Id)
                        throw new InvalidOperationException($"Audio reading target {id} is invalid, duplicated or belongs to another language.");
                foreach (var id in entry.ExampleIds)
                    if (!examples.TryGetValue(id, out var example) || !recordedExamples.Add(id) ||
                        rules[example.PronunciationRuleId].LanguageId != language.Id ||
                        !entry.ReadingIds.Select(readingId => readings[readingId]).Any(reading =>
                            items[reading.WritingSystemItemId].Symbol == example.Text &&
                            reading.Transcription == example.Transcription && reading.TranscriptionSystem == example.TranscriptionSystem))
                        throw new InvalidOperationException($"Audio example target {id} does not match the recorded reading.");
                using var audio = Open(entry.ResourceName);
                audio.ReadExactly(header);
                audio.Position = 0;
                var hasMp3Header = header.AsSpan(0, 3).SequenceEqual("ID3"u8) || (header[0] == 0xff && (header[1] & 0xe0) == 0xe0);
                if (audio.Length != entry.Bytes || !hasMp3Header || Convert.ToHexStringLower(SHA256.HashData(audio)) != entry.Sha256)
                    throw new InvalidOperationException($"Foundation audio checksum failed for '{entry.ResourceName}'.");
                entries.Add(entry);
            }
        }
        if (entries.Count == 0) throw new InvalidOperationException("No foundation audio manifests are embedded.");
        return entries.ToArray();
    }

    public static Stream Open(string resourceName) =>
        typeof(FoundationAudioCatalog).Assembly.GetManifestResourceStream(ResourcePrefix + resourceName)
        ?? throw new InvalidOperationException($"Embedded foundation audio resource '{resourceName}' is missing.");

    private static bool IsHttps(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps && uri.UserInfo.Length == 0 && !value.Any(char.IsWhiteSpace);
}
