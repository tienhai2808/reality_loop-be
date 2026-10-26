using Amazon.Runtime;
using Amazon.S3;

namespace Infrastructure.Storage.R2;

internal static class R2ClientFactory
{
    public static IAmazonS3 Create(R2Options options)
    {
        // DI owns and disposes this reusable client. R2 uses S3 signing with region auto.
        return new AmazonS3Client(
            new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey),
            new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                AuthenticationRegion = "auto",
                ForcePathStyle = true,
                UseHttp = false,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
                RetryMode = RequestRetryMode.Standard,
                MaxErrorRetry = options.MaxErrorRetry
            });
    }
}
