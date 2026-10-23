using Domain.Common;

namespace Domain.LearningProfiles;

public sealed class LearningProfile : Entity
{
    public const int LanguageMaxLength = 35;
    public const int GoalNoteMaxLength = 500;

    private LearningProfile()
    {
    }

    public long UserId { get; private set; }
    public string TargetLanguage { get; private init; } = null!;
    public string NativeLanguage { get; private set; } = null!;
    public string ExplanationLanguage { get; private set; } = null!;
    public ProficiencyLevel ProficiencyLevel { get; private set; }
    public LearningGoal PrimaryGoal { get; private set; }
    public string? GoalNote { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // The caller supplies an ID from the existing Snowflake generator.
    public static LearningProfile Create(long id, long userId, string targetLanguage,
        string nativeLanguage, string explanationLanguage, ProficiencyLevel proficiencyLevel,
        LearningGoal primaryGoal, string? goalNote, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ValidateEnums(proficiencyLevel, primaryGoal);
        var timestamp = now.ToUniversalTime();

        return new LearningProfile
        {
            Id = id,
            UserId = userId,
            TargetLanguage = NormalizeLanguage(targetLanguage, nameof(targetLanguage)),
            NativeLanguage = NormalizeLanguage(nativeLanguage, nameof(nativeLanguage)),
            ExplanationLanguage = NormalizeLanguage(explanationLanguage, nameof(explanationLanguage)),
            ProficiencyLevel = proficiencyLevel,
            PrimaryGoal = primaryGoal,
            GoalNote = NormalizeGoalNote(goalNote),
            Version = 1,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void UpdatePreferences(string nativeLanguage, string explanationLanguage,
        ProficiencyLevel proficiencyLevel, LearningGoal primaryGoal, string? goalNote, DateTimeOffset now)
    {
        var native = NormalizeLanguage(nativeLanguage, nameof(nativeLanguage));
        var explanation = NormalizeLanguage(explanationLanguage, nameof(explanationLanguage));
        var note = NormalizeGoalNote(goalNote);
        ValidateEnums(proficiencyLevel, primaryGoal);
        if (now < UpdatedAt)
            throw new ArgumentException("The update time cannot precede the previous update.", nameof(now));
        var nextVersion = checked(Version + 1);

        NativeLanguage = native;
        ExplanationLanguage = explanation;
        ProficiencyLevel = proficiencyLevel;
        PrimaryGoal = primaryGoal;
        GoalNote = note;
        UpdatedAt = now.ToUniversalTime();
        Version = nextVersion;
    }

    private static string NormalizeLanguage(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        var language = value.Trim().ToLowerInvariant();
        if (language.Length > LanguageMaxLength)
            throw new ArgumentException($"Language tags cannot exceed {LanguageMaxLength} characters.", parameterName);

        // Validate tag syntax without depending on the host's installed culture catalog.
        var subtags = language.Split('-');
        if (subtags[0].Length is < 2 or > 8 || subtags[0].Any(c => c is < 'a' or > 'z') ||
            subtags.Skip(1).Any(subtag => subtag.Length is < 1 or > 8 ||
                subtag.Any(c => c is not (>= 'a' and <= 'z') and not (>= '0' and <= '9'))))
            throw new ArgumentException("Use a language tag with hyphen-separated ASCII subtags, such as en-us or zh-hant.", parameterName);
        return language;
    }

    private static string? NormalizeGoalNote(string? value)
    {
        var note = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (note?.Length > GoalNoteMaxLength)
            throw new ArgumentException($"The goal note cannot exceed {GoalNoteMaxLength} characters.", nameof(value));
        return note;
    }

    private static void ValidateEnums(ProficiencyLevel proficiencyLevel, LearningGoal primaryGoal)
    {
        if (!Enum.IsDefined(proficiencyLevel)) throw new ArgumentOutOfRangeException(nameof(proficiencyLevel));
        if (!Enum.IsDefined(primaryGoal)) throw new ArgumentOutOfRangeException(nameof(primaryGoal));
    }
}
