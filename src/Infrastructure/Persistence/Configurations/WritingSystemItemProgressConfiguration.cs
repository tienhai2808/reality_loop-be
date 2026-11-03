using Domain.LearningProfiles;
using Domain.LearningProgress;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WritingSystemItemProgressConfiguration : IEntityTypeConfiguration<WritingSystemItemProgress>
{
    public void Configure(EntityTypeBuilder<WritingSystemItemProgress> builder)
    {
        builder.ToTable("writing_system_item_progress", table =>
        {
            table.HasCheckConstraint("ck_writing_system_item_progress_positive_ids",
                "id > 0 AND learning_profile_id > 0 AND writing_system_item_id > 0");
            table.HasCheckConstraint("ck_writing_system_item_progress_version", "version > 0");
            table.HasCheckConstraint("ck_writing_system_item_progress_status",
                $"status IN ({EnumValues<WritingSystemItemProgressStatus>()})");
            table.HasCheckConstraint("ck_writing_system_item_progress_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_writing_system_item_progress_study_time",
                "last_studied_at >= created_at AND last_studied_at <= updated_at");
            table.HasCheckConstraint("ck_writing_system_item_progress_completion_state", """
                (status = 'Learning' AND completed_at IS NULL)
                OR (status = 'Completed' AND completed_at IS NOT NULL)
                """);
            table.HasCheckConstraint("ck_writing_system_item_progress_completion_time",
                "completed_at IS NULL OR (completed_at >= created_at AND completed_at <= last_studied_at)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LearningProfileId).HasColumnName("learning_profile_id").IsRequired();
        builder.Property(x => x.WritingSystemItemId).HasColumnName("writing_system_item_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32)
            .HasDefaultValue(WritingSystemItemProgressStatus.Learning).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.LastStudiedAt).HasColumnName("last_studied_at")
            .HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<LearningProfile>().WithMany().HasForeignKey(x => x.LearningProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<WritingSystemItem>().WithMany().HasForeignKey(x => x.WritingSystemItemId)
            .OnDelete(DeleteBehavior.Cascade);
        // Also covers queries and cascading deletes filtered by learning_profile_id.
        builder.HasIndex(x => new { x.LearningProfileId, x.WritingSystemItemId }).IsUnique()
            .HasDatabaseName("ux_writing_system_item_progress_profile_id_item_id");
        builder.HasIndex(x => x.WritingSystemItemId)
            .HasDatabaseName("ix_writing_system_item_progress_writing_system_item_id");
    }

    private static string EnumValues<TEnum>() where TEnum : struct, Enum =>
        string.Join(", ", Enum.GetNames<TEnum>().Select(value => $"'{value}'"));
}
