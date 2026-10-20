namespace Domain.AI;

public enum AiFailureCode
{
    InvalidApiKey = 1,
    QuotaExceeded = 2,
    RateLimited = 3,
    ProviderUnavailable = 4,
    ModelUnavailable = 5
}
