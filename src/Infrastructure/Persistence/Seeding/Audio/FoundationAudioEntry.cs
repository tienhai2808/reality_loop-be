namespace Infrastructure.Persistence.Seeding.Audio;

internal sealed record FoundationAudioEntry(
    string Reading, string ItemCode, string Hiragana, string Katakana, string FileName,
    string ObjectKey, string Sha256, long Bytes, string ContentType, string SourcePage,
    string Author, string License);
