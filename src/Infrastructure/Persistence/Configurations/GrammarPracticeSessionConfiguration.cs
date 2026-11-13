using Domain.Grammar;
using Domain.GrammarPractice;
using Domain.Languages;
using Domain.LearningProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarPracticeSessionConfiguration : IEntityTypeConfiguration<GrammarPracticeSession>
{
    public void Configure(EntityTypeBuilder<GrammarPracticeSession> builder)
    {
        builder.ToTable("grammar_practice_sessions", table =>
        {
            table.HasCheckConstraint("ck_grammar_practice_sessions_positive_ids",
                "id > 0 AND learning_profile_id > 0 AND language_id > 0 AND (grammar_point_id IS NULL OR grammar_point_id > 0)");
            table.HasCheckConstraint("ck_grammar_practice_sessions_question_count",
                $"question_count BETWEEN {GrammarPracticeSession.MinQuestions} AND {GrammarPracticeSession.MaxQuestions}");
            table.HasCheckConstraint("ck_grammar_practice_sessions_locale", "explanation_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_grammar_practice_sessions_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_practice_sessions_status",
                $"status IN ({string.Join(", ", Enum.GetNames<GrammarPracticeSessionStatus>().Select(x => $"'{x}'"))})");
            table.HasCheckConstraint("ck_grammar_practice_sessions_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_grammar_practice_sessions_end_state", """
                (status = 'InProgress' AND ended_at IS NULL)
                OR (status IN ('Completed', 'Abandoned') AND ended_at IS NOT NULL
                    AND ended_at >= created_at AND ended_at = updated_at)
                """);
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LearningProfileId).HasColumnName("learning_profile_id").IsRequired();
        builder.Property(x => x.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(x => x.GrammarPointId).HasColumnName("grammar_point_id");
        builder.Property(x => x.ExplanationLanguage).HasColumnName("explanation_language").HasMaxLength(35).IsRequired();
        builder.Property(x => x.QuestionCount).HasColumnName("question_count").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32)
            .HasDefaultValue(GrammarPracticeSessionStatus.InProgress).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.EndedAt).HasColumnName("ended_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<LearningProfile>().WithMany().HasForeignKey(x => x.LearningProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        // Catalog removal must not silently erase completed practice history. Unpublish content instead.
        builder.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<GrammarPoint>().WithMany().HasForeignKey(x => x.GrammarPointId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Items).HasField("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
        // Covers profile ownership/history lookups and FK cascade; no standalone profile index is needed.
        builder.HasIndex(x => new { x.LearningProfileId, x.Id }).IsDescending(false, true)
            .HasDatabaseName("ix_grammar_practice_sessions_profile_id_id");
        builder.HasIndex(x => x.LanguageId).HasDatabaseName("ix_grammar_practice_sessions_language");
        builder.HasIndex(x => x.GrammarPointId).HasDatabaseName("ix_grammar_practice_sessions_point");
    }
}
