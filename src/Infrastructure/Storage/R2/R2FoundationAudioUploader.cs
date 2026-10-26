using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.Persistence.Seeding.Audio;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.R2;

internal sealed class R2FoundationAudioUploader(IAmazonS3 client, IOptions<R2Options> options)
{
    public async Task UploadAsync(FoundationAudioEntry entry, CancellationToken ct)
    {
        using var stream = FoundationAudioCatalog.Open(entry.FileName);
        var request = new PutObjectRequest
        {
            BucketName = options.Value.BucketName,
            Key = entry.ObjectKey,
            InputStream = stream,
            ContentType = entry.ContentType,
            AutoCloseStream = false,
            UseChunkEncoding = false,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true,
            IfNoneMatch = "*"
        };
        request.Headers.CacheControl = "public, max-age=31536000, immutable";
        request.Metadata["sha256"] = entry.Sha256;
        request.Metadata["source"] = entry.SourcePage;
        request.Metadata["author"] = entry.Author;
        request.Metadata["license"] = entry.License;

        try
        {
            await client.PutObjectAsync(request, ct);
        }
        catch (AmazonS3Exception error) when (error.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            // A concurrent or previous initialization already created this immutable key.
            var existing = await client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = options.Value.BucketName,
                Key = entry.ObjectKey
            }, ct);
            if (existing.ContentLength != entry.Bytes || existing.Metadata["x-amz-meta-sha256"] != entry.Sha256)
                throw new InvalidOperationException($"R2 object '{entry.ObjectKey}' exists with different seed metadata; refusing to overwrite it.");
        }
    }
}
