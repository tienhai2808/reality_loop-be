using Domain.LearningProfiles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LearningProfileConfiguration : IEntityTypeConfiguration<LearningProfile>
{
    public void Configure(EntityTypeBuilder<LearningProfile> builder)
    {
        builder.ToTable("learning_profiles", table =>
        {
            table.HasCheckConstraint("ck_learning_profiles_positive_ids", "id > 0 AND user_id > 0");
            table.HasCheckConstraint("ck_learning_profiles_version", "version > 0");
            table.HasCheckConstraint("ck_learning_profiles_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_learning_profiles_language_tags", """
                target_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'
                AND native_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'
                AND explanation_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'
                """);
            table.HasCheckConstraint("ck_learning_profiles_proficiency_level",
                $"proficiency_level IN ({EnumValues<ProficiencyLevel>()})");
            table.HasCheckConstraint("ck_learning_profiles_primary_goal",
                $"primary_goal IN ({EnumValues<LearningGoal>()})");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.TargetLanguage).HasColumnName("target_language")
            .HasMaxLength(LearningProfile.LanguageMaxLength).IsRequired();
        builder.Property(x => x.NativeLanguage).HasColumnName("native_language")
            .HasMaxLength(LearningProfile.LanguageMaxLength).IsRequired();
        builder.Property(x => x.ExplanationLanguage).HasColumnName("explanation_language")
            .HasMaxLength(LearningProfile.LanguageMaxLength).IsRequired();
        builder.Property(x => x.ProficiencyLevel).HasColumnName("proficiency_level")
            .HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PrimaryGoal).HasColumnName("primary_goal")
            .HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.GoalNote).HasColumnName("goal_note").HasMaxLength(LearningProfile.GoalNoteMaxLength);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        // The unique index also covers queries filtered by user_id.
        builder.HasIndex(x => new { x.UserId, x.TargetLanguage }).IsUnique()
            .HasDatabaseName("ux_learning_profiles_user_id_target_language");
        // Required as the principal key for the selection's owner-qualified foreign key.
        builder.HasAlternateKey(x => new { x.UserId, x.Id }).HasName("ak_learning_profiles_user_id_id");
    }

    private static string EnumValues<TEnum>() where TEnum : struct, Enum =>
        string.Join(", ", Enum.GetNames<TEnum>().Select(value => $"'{value}'"));
}
