using Domain.Common;

namespace Domain.AI;

public sealed class AiConnection : Entity
{
    public const int DisplayNameMaxLength = 150;
    public const int ModelIdMaxLength = 200;
    public const int ApiKeyLast4Length = 4;

    private AiConnection()
    {
    }

    public long UserId { get; private set; }
    public AiProvider Provider { get; private set; }
    public string DisplayName { get; private set; } = null!;

    // Encryption/decryption belongs to Infrastructure. This entity accepts ciphertext only.
    public string EncryptedApiKey { get; private set; } = null!;
    public string ApiKeyLast4 { get; private set; } = null!;
    public string ModelId { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsEnabled { get; private set; }
    public AiConnectionStatus Status { get; private set; }
    public DateTimeOffset? RetryAfterAt { get; private set; }
    public AiFailureCode? LastFailureCode { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // The application must validate credentials/model with the provider before creating a ready connection.
    public static AiConnection Create(long id, long userId, AiProvider provider, string? displayName,
        string encryptedApiKey, string apiKeyLast4, string modelId, int sortOrder, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        if (!Enum.IsDefined(provider)) throw new ArgumentOutOfRangeException(nameof(provider));
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedApiKey);
        ValidateKeyHint(apiKeyLast4);
        var name = NormalizeDisplayName(displayName, provider);
        var model = NormalizeModelId(modelId);
        var timestamp = now.ToUniversalTime();

        return new AiConnection
        {
            Id = id,
            UserId = userId,
            Provider = provider,
            DisplayName = name,
            EncryptedApiKey = encryptedApiKey,
            ApiKeyLast4 = apiKeyLast4,
            ModelId = model,
            SortOrder = sortOrder,
            IsEnabled = true,
            Status = AiConnectionStatus.Ready,
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void UpdateSettings(string? displayName, string modelId, int sortOrder, bool isEnabled, DateTimeOffset now)
    {
        var name = NormalizeDisplayName(displayName, Provider);
        var model = NormalizeModelId(modelId);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        if (DisplayName == name && ModelId == model && SortOrder == sortOrder && IsEnabled == isEnabled) return;

        RecordChange(now);
        DisplayName = name;
        ModelId = model;
        SortOrder = sortOrder;
        IsEnabled = isEnabled;
        // Editing settings does not prove that an unhealthy connection has recovered.
    }

    public void ReplaceApiKey(string encryptedApiKey, string apiKeyLast4, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedApiKey);
        ValidateKeyHint(apiKeyLast4);
        if (EncryptedApiKey == encryptedApiKey && ApiKeyLast4 == apiKeyLast4) return;

        RecordChange(now);
        EncryptedApiKey = encryptedApiKey;
        ApiKeyLast4 = apiKeyLast4;
        // MarkReady is explicit: the application first verifies the replacement credentials.
    }

    public bool CanUse(DateTimeOffset now) => IsEnabled &&
        (Status == AiConnectionStatus.Ready ||
         (Status == AiConnectionStatus.CoolingDown && RetryAfterAt <= now));

    public void MarkReady(DateTimeOffset now)
    {
        if (Status == AiConnectionStatus.Ready) return;

        RecordChange(now);
        Status = AiConnectionStatus.Ready;
        RetryAfterAt = null;
        LastFailureCode = null;
    }

    // Explicit verification must perform a versioned write even if health did not change.
    public void RecordVerification(AiFailureCode? failureCode, DateTimeOffset? retryAfterAt, DateTimeOffset now)
    {
        var previousVersion = Version;
        if (failureCode is null) MarkReady(now);
        else if (retryAfterAt is { } retry) MarkCoolingDown(failureCode.Value, retry, now);
        else MarkNeedsAttention(failureCode.Value, now);
        if (Version == previousVersion) RecordChange(now);
    }

    public void MarkCoolingDown(AiFailureCode failureCode, DateTimeOffset retryAfterAt, DateTimeOffset now)
    {
        if (failureCode is not (AiFailureCode.QuotaExceeded or AiFailureCode.RateLimited or AiFailureCode.ProviderUnavailable))
            throw new ArgumentOutOfRangeException(nameof(failureCode), "This failure requires attention rather than a cooldown.");
        if (retryAfterAt <= now)
            throw new ArgumentException("The retry time must be in the future.", nameof(retryAfterAt));

        RecordChange(now);
        Status = AiConnectionStatus.CoolingDown;
        RetryAfterAt = retryAfterAt.ToUniversalTime();
        LastFailureCode = failureCode;
    }

    public void MarkNeedsAttention(AiFailureCode failureCode, DateTimeOffset now)
    {
        if (!Enum.IsDefined(failureCode)) throw new ArgumentOutOfRangeException(nameof(failureCode));
        if (Status == AiConnectionStatus.NeedsAttention && LastFailureCode == failureCode) return;

        RecordChange(now);
        Status = AiConnectionStatus.NeedsAttention;
        RetryAfterAt = null;
        LastFailureCode = failureCode;
    }

    private void RecordChange(DateTimeOffset now)
    {
        if (now < UpdatedAt) throw new ArgumentException("The update time cannot precede the previous update.", nameof(now));
        var nextVersion = checked(Version + 1);
        UpdatedAt = now.ToUniversalTime();
        Version = nextVersion;
    }

    private static string NormalizeDisplayName(string? value, AiProvider provider)
    {
        var name = string.IsNullOrWhiteSpace(value) ? provider.ToString() : value.Trim();
        if (name.Length > DisplayNameMaxLength)
            throw new ArgumentException($"The display name cannot exceed {DisplayNameMaxLength} characters.", nameof(value));
        return name;
    }

    private static string NormalizeModelId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var model = value.Trim();
        if (model.Length > ModelIdMaxLength)
            throw new ArgumentException($"The model ID cannot exceed {ModelIdMaxLength} characters.", nameof(value));
        return model;
    }

    private static void ValidateKeyHint(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length != ApiKeyLast4Length || value.Any(c => !char.IsAscii(c) || char.IsControl(c) || char.IsWhiteSpace(c)))
            throw new ArgumentException("The key hint must contain exactly four printable non-whitespace ASCII characters.", nameof(value));
    }
}
