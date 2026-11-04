using Domain.Handwriting;
using Domain.LearningProfiles;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class HandwritingPracticeAttemptConfiguration : IEntityTypeConfiguration<HandwritingPracticeAttempt>
{
    public void Configure(EntityTypeBuilder<HandwritingPracticeAttempt> builder)
    {
        builder.ToTable("handwriting_practice_attempts", table =>
        {
            table.HasCheckConstraint("ck_handwriting_attempts_positive_ids", "id > 0 AND learning_profile_id > 0 AND writing_system_id > 0");
            table.HasCheckConstraint("ck_handwriting_attempts_characters", $"character_count BETWEEN 1 AND {HandwritingPracticeAttempt.MaxCharacters}");
            table.HasCheckConstraint("ck_handwriting_attempts_score", "match_score BETWEEN 0 AND 100");
            table.HasCheckConstraint("ck_handwriting_attempts_algorithm", "algorithm_version ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_handwriting_attempts_text", "target_text ~ '[^[:space:]]'");
            // Attempts are complete at creation; resubmission creates a new attempt.
            table.HasCheckConstraint("ck_handwriting_attempts_immutable_state", "version = 1 AND updated_at = created_at");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LearningProfileId).HasColumnName("learning_profile_id").IsRequired();
        builder.Property(x => x.WritingSystemId).HasColumnName("writing_system_id").IsRequired();
        builder.Property(x => x.TargetText).HasColumnName("target_text").HasMaxLength(64 * HandwritingPracticeAttempt.MaxCharacters).IsRequired();
        builder.Property(x => x.AlgorithmVersion).HasColumnName("algorithm_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CharacterCount).HasColumnName("character_count").IsRequired();
        builder.Property(x => x.Passed).HasColumnName("passed").IsRequired();
        builder.Property(x => x.MatchScore).HasColumnName("match_score").IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasOne<LearningProfile>().WithMany().HasForeignKey(x => x.LearningProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<WritingSystem>().WithMany().HasForeignKey(x => x.WritingSystemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Characters).WithOne().HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Characters).HasField("_characters").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.LearningProfileId, x.Id }).IsDescending(false, true)
            .HasDatabaseName("ix_handwriting_attempts_profile_id_id");
        builder.HasIndex(x => x.WritingSystemId).HasDatabaseName("ix_handwriting_attempts_writing_system_id");
    }
}
