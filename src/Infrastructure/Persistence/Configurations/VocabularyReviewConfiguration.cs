using Domain.VocabularyLearning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VocabularyReviewConfiguration : IEntityTypeConfiguration<VocabularyReview>
{
    public void Configure(EntityTypeBuilder<VocabularyReview> builder)
    {
        builder.ToTable("vocabulary_reviews", table =>
        {
            table.HasCheckConstraint("ck_vocabulary_reviews_ids", "id > 0 AND progress_id > 0 AND request_id <> '00000000-0000-0000-0000-000000000000'::uuid");
            table.HasCheckConstraint("ck_vocabulary_reviews_rating", "rating IN ('Again', 'Hard', 'Good', 'Easy')");
            table.HasCheckConstraint("ck_vocabulary_reviews_status", "previous_status IN ('New', 'Learning', 'Reviewing') AND status IN ('Learning', 'Reviewing')");
            table.HasCheckConstraint("ck_vocabulary_reviews_previous", """
                (review_number = 1 AND previous_status = 'New' AND previous_interval_minutes = 0)
                OR (review_number > 1 AND previous_status IN ('Learning', 'Reviewing') AND previous_interval_minutes > 0)
                """);
            table.HasCheckConstraint("ck_vocabulary_reviews_counts", "review_number > 0 AND again_count >= 0 AND consecutive_recall_count >= 0 AND consecutive_recall_count <= review_number AND again_count <= review_number - consecutive_recall_count");
            table.HasCheckConstraint("ck_vocabulary_reviews_outcome", """
                (rating = 'Again' AND status = 'Learning' AND consecutive_recall_count = 0 AND again_count > 0)
                OR (rating <> 'Again' AND consecutive_recall_count > 0)
                """);
            table.HasCheckConstraint("ck_vocabulary_reviews_versions", "progress_version_before > 0 AND review_number <= progress_version_before AND progress_version_after > progress_version_before AND progress_version_after - progress_version_before = 1");
            table.HasCheckConstraint("ck_vocabulary_reviews_schedule", "interval_minutes > 0 AND next_review_at = reviewed_at + interval_minutes * interval '1 minute'");
            table.HasCheckConstraint("ck_vocabulary_reviews_scheduler", "scheduler_version ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_vocabulary_reviews_immutable_state", "version = 1 AND reviewed_at = created_at AND updated_at = created_at");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.ProgressId).HasColumnName("progress_id").IsRequired();
        builder.Property(x => x.RequestId).HasColumnName("request_id").IsRequired();
        builder.Property(x => x.ReviewNumber).HasColumnName("review_number").IsRequired();
        builder.Property(x => x.Rating).HasColumnName("rating").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PreviousStatus).HasColumnName("previous_status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PreviousDueAt).HasColumnName("previous_due_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.NextReviewAt).HasColumnName("next_review_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.PreviousIntervalMinutes).HasColumnName("previous_interval_minutes").IsRequired();
        builder.Property(x => x.IntervalMinutes).HasColumnName("interval_minutes").IsRequired();
        builder.Property(x => x.AgainCount).HasColumnName("again_count").IsRequired();
        builder.Property(x => x.ConsecutiveRecallCount).HasColumnName("consecutive_recall_count").IsRequired();
        builder.Property(x => x.ProgressVersionBefore).HasColumnName("progress_version_before").IsRequired();
        builder.Property(x => x.ProgressVersionAfter).HasColumnName("progress_version_after").IsRequired();
        builder.Property(x => x.SchedulerVersion).HasColumnName("scheduler_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReviewedAt).HasColumnName("reviewed_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<VocabularyLearningProgress>().WithMany().HasForeignKey(x => x.ProgressId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.ProgressId, x.RequestId }).IsUnique().HasDatabaseName("ux_vocabulary_reviews_progress_request");
        builder.HasIndex(x => new { x.ProgressId, x.ReviewNumber }).IsUnique().IsDescending(false, true).HasDatabaseName("ux_vocabulary_reviews_progress_number");
        builder.HasIndex(x => new { x.ProgressId, x.ProgressVersionBefore }).IsUnique().HasDatabaseName("ux_vocabulary_reviews_progress_version");
    }
}
