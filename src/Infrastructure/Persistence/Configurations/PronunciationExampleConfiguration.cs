using Domain.Pronunciation;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PronunciationExampleConfiguration : IEntityTypeConfiguration<PronunciationExample>
{
    public void Configure(EntityTypeBuilder<PronunciationExample> builder)
    {
        builder.ToTable("pronunciation_examples", table =>
        {
            table.HasCheckConstraint("ck_pronunciation_examples_positive_ids", "id > 0 AND pronunciation_rule_id > 0");
            table.HasCheckConstraint("ck_pronunciation_examples_version", "version > 0");
            table.HasCheckConstraint("ck_pronunciation_examples_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_pronunciation_examples_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_pronunciation_examples_text", "length(btrim(text)) > 0");
            table.HasCheckConstraint("ck_pronunciation_examples_transcription", "length(btrim(transcription)) > 0");
            table.HasCheckConstraint("ck_pronunciation_examples_transcription_system", "transcription_system ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_pronunciation_examples_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_pronunciation_examples_audio_url",
                "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.PronunciationRuleId).HasColumnName("pronunciation_rule_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(PronunciationExample.CodeMaxLength).IsRequired();
        builder.Property(x => x.Text).HasColumnName("text").HasMaxLength(PronunciationExample.TextMaxLength).IsRequired();
        builder.Property(x => x.Transcription).HasColumnName("transcription")
            .HasMaxLength(PronunciationExample.TranscriptionMaxLength).IsRequired();
        builder.Property(x => x.TranscriptionSystem).HasColumnName("transcription_system")
            .HasMaxLength(PronunciationExample.TranscriptionSystemMaxLength).IsRequired();
        builder.Property(x => x.AudioUrl).HasColumnName("audio_url").HasMaxLength(PronunciationExample.AudioUrlMaxLength);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<PronunciationRule>().WithMany().HasForeignKey(x => x.PronunciationRuleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.PronunciationRuleId, x.Code }).IsUnique()
            .HasDatabaseName("ux_pronunciation_examples_rule_id_code");
        builder.HasIndex(x => new { x.PronunciationRuleId, x.SortOrder, x.Id })
            .HasDatabaseName("ix_pronunciation_examples_rule_id_sort_order_id");
        builder.HasData(PronunciationSeedData.ExampleRows);
    }
}
