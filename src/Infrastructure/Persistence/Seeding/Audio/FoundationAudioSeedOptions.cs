namespace Infrastructure.Persistence.Seeding.Audio;

public sealed class FoundationAudioSeedOptions
{
    public const string SectionName = "FoundationAudioSeed";

    public string PublicBaseUrl { get; init; } = "";

    internal bool IsValid() => !string.IsNullOrWhiteSpace(PublicBaseUrl) && PublicBaseUrl.Length <= 512 &&
        PublicBaseUrl == PublicBaseUrl.Trim() &&
        Uri.TryCreate(PublicBaseUrl, UriKind.Absolute, out var url) && url.Scheme == Uri.UriSchemeHttps &&
        url.IsDefaultPort && !string.IsNullOrEmpty(url.Host) && url.AbsolutePath == "/" &&
        url.UserInfo.Length == 0 && url.Query.Length == 0 && url.Fragment.Length == 0 &&
        !PublicBaseUrl.Any(char.IsWhiteSpace);
}
