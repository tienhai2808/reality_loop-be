using Domain.AI;

namespace Application.Common.Exceptions;

// Only safe, application-owned messages; never copy a raw provider body into this exception.
public sealed class AiProviderException(AiFailureCode code, string message,
    DateTimeOffset? retryAfterAt = null, bool isTimeout = false, bool modelListingForbidden = false) : Exception(message)
{
    public AiFailureCode Code { get; } = code;
    public DateTimeOffset? RetryAfterAt { get; } = retryAfterAt;
    public bool IsTimeout { get; } = isTimeout;
    public bool ModelListingForbidden { get; } = modelListingForbidden;
}
