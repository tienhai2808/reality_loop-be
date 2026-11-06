using Domain.VocabularyLearning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VocabularyLearningProgressConfiguration : IEntityTypeConfiguration<VocabularyLearningProgress>
{
    public void Configure(EntityTypeBuilder<VocabularyLearningProgress> builder)
    {
        builder.ToTable("vocabulary_learning_progress", table =>
        {
            table.HasCheckConstraint("ck_vocabulary_progress_ids", "id > 0 AND learning_profile_id > 0 AND vocabulary_sense_id > 0");
            table.HasCheckConstraint("ck_vocabulary_progress_counts", "review_count >= 0 AND again_count >= 0 AND consecutive_recall_count >= 0 AND consecutive_recall_count <= review_count AND again_count <= review_count - consecutive_recall_count");
            table.HasCheckConstraint("ck_vocabulary_progress_version", "version > 0 AND review_count < version");
            table.HasCheckConstraint("ck_vocabulary_progress_status", "status IN ('New', 'Learning', 'Reviewing')");
            table.HasCheckConstraint("ck_vocabulary_progress_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_vocabulary_progress_state", """
                (status = 'New' AND review_count = 0 AND again_count = 0 AND consecutive_recall_count = 0
                    AND last_reviewed_at IS NULL AND interval_minutes = 0 AND next_review_at = created_at
                    AND scheduler_version IS NULL AND version = 1 AND updated_at = created_at)
                OR (status IN ('Learning', 'Reviewing') AND review_count > 0 AND last_reviewed_at IS NOT NULL
                    AND last_reviewed_at >= created_at AND last_reviewed_at = updated_at
                    AND interval_minutes > 0 AND next_review_at = last_reviewed_at + interval_minutes * interval '1 minute'
                    AND scheduler_version IS NOT NULL AND scheduler_version ~ '^[a-z][a-z0-9-]*$')
                """);
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LearningProfileId).HasColumnName("learning_profile_id").IsRequired();
        builder.Property(x => x.VocabularySenseId).HasColumnName("vocabulary_sense_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.NextReviewAt).HasColumnName("next_review_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.LastReviewedAt).HasColumnName("last_reviewed_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.IntervalMinutes).HasColumnName("interval_minutes").IsRequired();
        builder.Property(x => x.ReviewCount).HasColumnName("review_count").IsRequired();
        builder.Property(x => x.AgainCount).HasColumnName("again_count").IsRequired();
        builder.Property(x => x.ConsecutiveRecallCount).HasColumnName("consecutive_recall_count").IsRequired();
        builder.Property(x => x.SchedulerVersion).HasColumnName("scheduler_version").HasMaxLength(64);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<Domain.LearningProfiles.LearningProfile>().WithMany().HasForeignKey(x => x.LearningProfileId).OnDelete(DeleteBehavior.Cascade);
        // Unpublish catalog content instead of silently deleting a learner's review history.
        builder.HasOne<Domain.Vocabulary.VocabularySense>().WithMany().HasForeignKey(x => x.VocabularySenseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.LearningProfileId, x.VocabularySenseId }).IsUnique().HasDatabaseName("ux_vocabulary_progress_profile_sense");
        builder.HasIndex(x => new { x.LearningProfileId, x.NextReviewAt, x.Id }).HasDatabaseName("ix_vocabulary_progress_profile_due_id");
        builder.HasIndex(x => x.VocabularySenseId).HasDatabaseName("ix_vocabulary_progress_sense");
    }
}
