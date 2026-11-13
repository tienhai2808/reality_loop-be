namespace Infrastructure.Persistence.Seeding.Audio;

internal sealed record FoundationAudioEntry(
    string FileName, string ObjectKey, string Sha256, long Bytes, string ContentType,
    string SourcePage, string Author, string License, string LicenseUrl, long[] ReadingIds, long[] ExampleIds)
{
    public string LanguageCode { get; init; } = "";
    public string ResourceName => $"{LanguageCode}.{FileName}";
}
