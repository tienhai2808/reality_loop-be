namespace Infrastructure.Storage.R2;

public sealed class R2Options
{
    public const string SectionName = "Storage:R2";

    public string ServiceUrl { get; init; } = "";
    public string BucketName { get; init; } = "";
    public string AccessKeyId { get; init; } = "";
    public string SecretAccessKey { get; init; } = "";
    public int TimeoutSeconds { get; init; } = 60;
    public int MaxErrorRetry { get; init; } = 2;
}
