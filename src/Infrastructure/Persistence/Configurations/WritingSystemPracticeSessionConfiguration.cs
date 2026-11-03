using Domain.LearningProfiles;
using Domain.WritingSystemPractice;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WritingSystemPracticeSessionConfiguration : IEntityTypeConfiguration<WritingSystemPracticeSession>
{
    public void Configure(EntityTypeBuilder<WritingSystemPracticeSession> builder)
    {
        builder.ToTable("writing_system_practice_sessions", table =>
        {
            table.HasCheckConstraint("ck_ws_practice_sessions_positive_ids",
                "id > 0 AND learning_profile_id > 0 AND writing_system_id > 0");
            table.HasCheckConstraint("ck_ws_practice_sessions_question_count",
                $"question_count BETWEEN 1 AND {WritingSystemPracticeSession.MaxQuestions}");
            table.HasCheckConstraint("ck_ws_practice_sessions_version", "version > 0");
            table.HasCheckConstraint("ck_ws_practice_sessions_status",
                $"status IN ({string.Join(", ", Enum.GetNames<WritingSystemPracticeSessionStatus>().Select(x => $"'{x}'"))})");
            table.HasCheckConstraint("ck_ws_practice_sessions_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_ws_practice_sessions_end_state", """
                (status = 'InProgress' AND ended_at IS NULL)
                OR (status IN ('Completed', 'Abandoned') AND ended_at IS NOT NULL
                    AND ended_at >= created_at AND ended_at = updated_at)
                """);
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LearningProfileId).HasColumnName("learning_profile_id").IsRequired();
        builder.Property(x => x.WritingSystemId).HasColumnName("writing_system_id").IsRequired();
        builder.Property(x => x.QuestionCount).HasColumnName("question_count").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32)
            .HasDefaultValue(WritingSystemPracticeSessionStatus.InProgress).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.EndedAt).HasColumnName("ended_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<LearningProfile>().WithMany().HasForeignKey(x => x.LearningProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        // Catalog removal must not silently erase completed practice history. Unpublish content instead.
        builder.HasOne<WritingSystem>().WithMany().HasForeignKey(x => x.WritingSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Items).HasField("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
        // Covers profile ownership/history lookups and FK cascade; no standalone profile index is needed.
        builder.HasIndex(x => new { x.LearningProfileId, x.Id }).IsDescending(false, true)
            .HasDatabaseName("ix_ws_practice_sessions_profile_id_id");
        builder.HasIndex(x => x.WritingSystemId).HasDatabaseName("ix_ws_practice_sessions_writing_system_id");
    }
}
