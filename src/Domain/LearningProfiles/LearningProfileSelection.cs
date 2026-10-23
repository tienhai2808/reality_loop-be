namespace Domain.LearningProfiles;

// UserId is the identity of the selection; it does not have a separate Snowflake ID.
public sealed class LearningProfileSelection
{
    private LearningProfileSelection()
    {
    }

    public long UserId { get; private set; }
    public long ActiveProfileId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static LearningProfileSelection Create(long userId, LearningProfile activeProfile, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ValidateOwnership(userId, activeProfile);

        return new LearningProfileSelection
        {
            UserId = userId,
            ActiveProfileId = activeProfile.Id,
            UpdatedAt = now.ToUniversalTime()
        };
    }

    public void SelectProfile(LearningProfile activeProfile, DateTimeOffset now)
    {
        ValidateOwnership(UserId, activeProfile);
        if (now < UpdatedAt)
            throw new ArgumentException("The update time cannot precede the previous selection update.", nameof(now));

        ActiveProfileId = activeProfile.Id;
        UpdatedAt = now.ToUniversalTime();
    }

    private static void ValidateOwnership(long userId, LearningProfile activeProfile)
    {
        ArgumentNullException.ThrowIfNull(activeProfile);
        if (activeProfile.UserId != userId)
            throw new ArgumentException("The selected learning profile must belong to the same user.", nameof(activeProfile));
    }
}
