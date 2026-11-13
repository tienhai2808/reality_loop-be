using Domain.GrammarPractice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarPracticeSessionItemConfiguration : IEntityTypeConfiguration<GrammarPracticeSessionItem>
{
    public void Configure(EntityTypeBuilder<GrammarPracticeSessionItem> builder)
    {
        builder.ToTable("grammar_practice_session_items", table =>
        {
            table.HasCheckConstraint("ck_grammar_practice_items_positive_ids",
                "id > 0 AND session_id > 0 AND grammar_exercise_id > 0");
            table.HasCheckConstraint("ck_grammar_practice_items_position",
                $"position >= 0 AND position < {GrammarPracticeSession.MaxQuestions}");
            table.HasCheckConstraint("ck_grammar_practice_items_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_practice_items_prompt", GrammarPracticeConstraints.Prompt);
            table.HasCheckConstraint("ck_grammar_practice_items_translation_locale",
                GrammarPracticeConstraints.Locale);
            table.HasCheckConstraint("ck_grammar_practice_items_options_shape", GrammarPracticeConstraints.OptionsShape);
            table.HasCheckConstraint("ck_grammar_practice_items_options_text", GrammarPracticeConstraints.OptionsText);
            table.HasCheckConstraint("ck_grammar_practice_items_options_distinct", GrammarPracticeConstraints.OptionsDistinct);
            table.HasCheckConstraint("ck_grammar_practice_items_correct_option", "correct_option_index >= 0 AND correct_option_index < cardinality(options)");
            table.HasCheckConstraint("ck_grammar_practice_items_source_versions", "exercise_version > 0 AND translation_version > 0 AND grammar_point_id > 0");
            table.HasCheckConstraint("ck_grammar_practice_items_text", "instruction ~ '[^[:space:]]' AND explanation ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_grammar_practice_items_answer_state", """
                (selected_option_index IS NULL AND answered_at IS NULL AND version = 1 AND updated_at = created_at)
                OR (selected_option_index IS NOT NULL AND answered_at IS NOT NULL
                    AND selected_option_index >= 0 AND selected_option_index < cardinality(options)
                    AND answered_at >= created_at AND answered_at = updated_at AND version = 2)
                """);
            table.HasCheckConstraint("ck_grammar_practice_items_timestamps", "updated_at >= created_at");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(x => x.GrammarExerciseId).HasColumnName("grammar_exercise_id").IsRequired();
        builder.Property(x => x.GrammarPointId).HasColumnName("grammar_point_id").IsRequired();
        builder.Property(x => x.ExerciseVersion).HasColumnName("exercise_version").IsRequired();
        builder.Property(x => x.TranslationVersion).HasColumnName("translation_version").IsRequired();
        builder.Property(x => x.Instruction).HasColumnName("instruction").HasMaxLength(GrammarExerciseTranslation.InstructionMaxLength).IsRequired();
        builder.Property(x => x.Explanation).HasColumnName("explanation").HasMaxLength(GrammarExerciseTranslation.ExplanationMaxLength).IsRequired();
        builder.Property(x => x.Position).HasColumnName("position").IsRequired();
        builder.Property(x => x.Prompt).HasColumnName("prompt").HasMaxLength(GrammarExercise.PromptMaxLength).IsRequired();
        builder.Property(x => x.TranslationLocale).HasColumnName("translation_locale").HasMaxLength(35).IsRequired();
        builder.Property(x => x.Options).HasField("_options").UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("options").HasColumnType("character varying(250)[]").IsRequired();
        builder.Property(x => x.CorrectOptionIndex).HasColumnName("correct_option_index").IsRequired();
        builder.Property(x => x.SelectedOptionIndex).HasColumnName("selected_option_index");
        builder.Property(x => x.AnsweredAt).HasColumnName("answered_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Ignore(x => x.IsCorrect);

        builder.HasOne<GrammarExercise>().WithMany().HasForeignKey(x => new { x.GrammarExerciseId, x.GrammarPointId })
            .HasPrincipalKey(x => new { x.Id, x.GrammarPointId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.SessionId, x.Position }).IsUnique()
            .HasDatabaseName("ux_grammar_practice_items_session_id_position");
        builder.HasIndex(x => new { x.SessionId, x.GrammarExerciseId }).IsUnique()
            .HasDatabaseName("ux_grammar_practice_items_session_exercise");
        builder.HasIndex(x => new { x.GrammarExerciseId, x.GrammarPointId }).HasDatabaseName("ix_grammar_practice_items_exercise_point");
    }
}
