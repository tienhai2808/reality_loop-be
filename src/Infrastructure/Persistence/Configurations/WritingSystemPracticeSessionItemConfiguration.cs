using Domain.WritingSystemPractice;
using Domain.WritingSystems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WritingSystemPracticeSessionItemConfiguration : IEntityTypeConfiguration<WritingSystemPracticeSessionItem>
{
    public void Configure(EntityTypeBuilder<WritingSystemPracticeSessionItem> builder)
    {
        builder.ToTable("writing_system_practice_session_items", table =>
        {
            table.HasCheckConstraint("ck_ws_practice_items_positive_ids",
                "id > 0 AND session_id > 0 AND writing_system_item_id > 0");
            table.HasCheckConstraint("ck_ws_practice_items_position",
                $"position >= 0 AND position < {WritingSystemPracticeSession.MaxQuestions}");
            table.HasCheckConstraint("ck_ws_practice_items_version", "version > 0");
            table.HasCheckConstraint("ck_ws_practice_items_symbol", "length(btrim(symbol)) > 0");
            table.HasCheckConstraint("ck_ws_practice_items_transcription_system",
                "transcription_system ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_ws_practice_items_options_shape", $"""
                CASE WHEN array_ndims(options) = 1 AND array_lower(options, 1) = 1
                    THEN cardinality(options) BETWEEN {WritingSystemPracticeQuestion.MinOptions} AND {WritingSystemPracticeQuestion.MaxOptions}
                        AND array_position(options, NULL) IS NULL
                    ELSE FALSE END
                """);
            table.HasCheckConstraint("ck_ws_practice_items_options_text", """
                options[1] ~ '[^[:space:]]' AND options[2] ~ '[^[:space:]]'
                AND (cardinality(options) < 3 OR options[3] ~ '[^[:space:]]')
                AND (cardinality(options) < 4 OR options[4] ~ '[^[:space:]]')
                """);
            table.HasCheckConstraint("ck_ws_practice_items_options_distinct", """
                options[1] <> options[2]
                AND (cardinality(options) < 3 OR (options[1] <> options[3] AND options[2] <> options[3]))
                AND (cardinality(options) < 4 OR (options[1] <> options[4] AND options[2] <> options[4] AND options[3] <> options[4]))
                """);
            table.HasCheckConstraint("ck_ws_practice_items_correct_option",
                "correct_option_index >= 0 AND correct_option_index < cardinality(options)");
            table.HasCheckConstraint("ck_ws_practice_items_answer_state", """
                (selected_option_index IS NULL AND answered_at IS NULL AND version = 1 AND updated_at = created_at)
                OR (selected_option_index IS NOT NULL AND answered_at IS NOT NULL
                    AND selected_option_index >= 0 AND selected_option_index < cardinality(options)
                    AND answered_at >= created_at AND answered_at = updated_at AND version = 2)
                """);
            table.HasCheckConstraint("ck_ws_practice_items_timestamps", "updated_at >= created_at");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(x => x.WritingSystemItemId).HasColumnName("writing_system_item_id").IsRequired();
        builder.Property(x => x.Position).HasColumnName("position").IsRequired();
        builder.Property(x => x.Symbol).HasColumnName("symbol").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TranscriptionSystem).HasColumnName("transcription_system").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Options).HasField("_options").UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("options").HasColumnType("character varying(200)[]").IsRequired();
        builder.Property(x => x.CorrectOptionIndex).HasColumnName("correct_option_index").IsRequired();
        builder.Property(x => x.SelectedOptionIndex).HasColumnName("selected_option_index");
        builder.Property(x => x.AnsweredAt).HasColumnName("answered_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.Ignore(x => x.IsCorrect);

        builder.HasOne<WritingSystemItem>().WithMany().HasForeignKey(x => x.WritingSystemItemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.SessionId, x.Position }).IsUnique()
            .HasDatabaseName("ux_ws_practice_items_session_id_position");
        builder.HasIndex(x => new { x.SessionId, x.WritingSystemItemId }).IsUnique()
            .HasDatabaseName("ux_ws_practice_items_session_id_item_id");
        builder.HasIndex(x => x.WritingSystemItemId).HasDatabaseName("ix_ws_practice_items_writing_system_item_id");
    }
}
