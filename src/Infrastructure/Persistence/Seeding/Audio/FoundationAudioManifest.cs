namespace Infrastructure.Persistence.Seeding.Audio;

internal sealed record FoundationAudioManifest(
    string Bundle, string LanguageCode, int ExpectedRecordings, FoundationAudioEntry[] Entries);
