using System.Security.Cryptography;
using System.Text.Json;

namespace Infrastructure.Persistence.Seeding.Audio;

internal static class FoundationAudioCatalog
{
    private const string ResourcePrefix = "FoundationAudio.";

    public static FoundationAudioEntry[] Load()
    {
        using var stream = Open("manifest.json");
        var manifest = JsonSerializer.Deserialize<FoundationAudioManifest>(stream, JsonSerializerOptions.Web)
            ?? throw new InvalidOperationException("Foundation audio manifest is missing.");
        var entries = manifest.Entries;
        if (manifest.Bundle != "ja-kana-v1" || entries is not { Length: 46 } ||
            entries.Select(x => x.Reading).Distinct(StringComparer.Ordinal).Count() != entries.Length ||
            entries.Select(x => x.ItemCode).Distinct(StringComparer.Ordinal).Count() != entries.Length)
            throw new InvalidOperationException("Foundation audio manifest does not match the reviewed kana bundle.");

        var header = new byte[4];
        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Reading) || entry.Reading.Any(c => !char.IsAsciiLetterLower(c)) ||
                entry.FileName != $"{entry.Reading}.mp3" || entry.Sha256 is not { Length: 64 } ||
                entry.Sha256.Any(c => !char.IsAsciiHexDigitLower(c)) ||
                entry.ObjectKey != $"foundation/ja/kana/v1/{entry.Reading}-{entry.Sha256[..16]}.mp3" ||
                entry.ContentType != "audio/mpeg" || entry.Bytes is < 128 or > 1_048_576 ||
                entry.Author != "Hakatanoshio117117" || entry.License != "Public domain (PD-self)" ||
                !entry.SourcePage.StartsWith("https://commons.wikimedia.org/wiki/File:", StringComparison.Ordinal))
                throw new InvalidOperationException("Foundation audio entry failed manifest validation.");

            var items = FoundationSeedData.WritingSystemItemRows.Where(x => x.Code == entry.ItemCode).ToArray();
            if (items.Length != 2 || !items.Any(x => x.Symbol == entry.Hiragana) ||
                !items.Any(x => x.Symbol == entry.Katakana) ||
                FoundationSeedData.WritingSystemItemReadingRows.Count(x =>
                    items.Any(item => item.Id == x.WritingSystemItemId) && x.Transcription == entry.Reading) != 2)
                throw new InvalidOperationException($"Audio mapping for '{entry.Reading}' does not match the foundation seed.");

            using var audio = Open(entry.FileName);
            audio.ReadExactly(header);
            audio.Position = 0;
            var hasMp3Header = header.AsSpan(0, 3).SequenceEqual("ID3"u8) ||
                (header[0] == 0xff && (header[1] & 0xe0) == 0xe0);
            if (audio.Length != entry.Bytes || !hasMp3Header ||
                Convert.ToHexStringLower(SHA256.HashData(audio)) != entry.Sha256)
                throw new InvalidOperationException($"Foundation audio checksum failed for '{entry.FileName}'.");
        }
        return entries;
    }

    public static Stream Open(string fileName) =>
        typeof(FoundationAudioCatalog).Assembly.GetManifestResourceStream(ResourcePrefix + fileName)
        ?? throw new InvalidOperationException($"Embedded foundation audio resource '{fileName}' is missing.");
}
