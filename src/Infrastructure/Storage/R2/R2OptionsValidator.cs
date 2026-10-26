using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.R2;

internal sealed partial class R2OptionsValidator : IValidateOptions<R2Options>
{
    public ValidateOptionsResult Validate(string? name, R2Options options)
    {
        if (name is not null && name != Options.DefaultName) return ValidateOptionsResult.Skip;

        var failures = new List<string>();
        if (!Uri.TryCreate(options.ServiceUrl, UriKind.Absolute, out var endpoint) ||
            endpoint.Scheme != Uri.UriSchemeHttps || !endpoint.IsDefaultPort ||
            !EndpointHostPattern().IsMatch(endpoint.Host) || endpoint.AbsolutePath != "/" ||
            endpoint.Query.Length != 0 || endpoint.Fragment.Length != 0 || endpoint.UserInfo.Length != 0)
            failures.Add("Storage:R2:ServiceUrl must be the HTTPS R2 S3 API endpoint copied from Cloudflare, without a bucket, query or credentials.");
        if (options.BucketName is null || options.BucketName.Length is < 3 or > 63 ||
            !BucketNamePattern().IsMatch(options.BucketName))
            failures.Add("Storage:R2:BucketName must contain 3-63 lowercase letters, digits or hyphens, starting and ending with a letter or digit.");
        if (!IsConfiguredSecret(options.AccessKeyId))
            failures.Add("Storage:R2:AccessKeyId is required. Configure R2 S3 credentials through user-secrets or the deployment secret store.");
        if (!IsConfiguredSecret(options.SecretAccessKey))
            failures.Add("Storage:R2:SecretAccessKey is required. Configure R2 S3 credentials through user-secrets or the deployment secret store.");
        if (options.TimeoutSeconds is < 1 or > 300)
            failures.Add("Storage:R2:TimeoutSeconds must be between 1 and 300.");
        if (options.MaxErrorRetry is < 0 or > 5)
            failures.Add("Storage:R2:MaxErrorRetry must be between 0 and 5.");

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsConfiguredSecret(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value == value.Trim() &&
        !value.StartsWith("__", StringComparison.Ordinal) && !value.Any(char.IsWhiteSpace);

    [GeneratedRegex(@"\A[a-f0-9]{32}(?:\.[a-z][a-z0-9-]*)?\.r2\.cloudflarestorage\.com\z", RegexOptions.CultureInvariant)]
    private static partial Regex EndpointHostPattern();

    [GeneratedRegex(@"\A[a-z0-9][a-z0-9-]*[a-z0-9]\z", RegexOptions.CultureInvariant)]
    private static partial Regex BucketNamePattern();
}
