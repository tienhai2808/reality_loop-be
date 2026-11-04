using Domain.Languages;
using Domain.Vocabulary;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VocabularyEntryConfiguration : IEntityTypeConfiguration<VocabularyEntry>
{
    public void Configure(EntityTypeBuilder<VocabularyEntry> builder)
    {
        builder.ToTable("vocabulary_entries", table =>
        {
            table.HasCheckConstraint("ck_vocabulary_entries_positive_ids", "id > 0 AND language_id > 0");
            table.HasCheckConstraint("ck_vocabulary_entries_version", "version > 0");
            table.HasCheckConstraint("ck_vocabulary_entries_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_vocabulary_entries_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_vocabulary_entries_term", "term ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_vocabulary_entries_reading", "reading IS NULL OR reading ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_vocabulary_entries_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_vocabulary_entries_audio_url",
                "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(VocabularyEntry.CodeMaxLength).IsRequired();
        builder.Property(x => x.Term).HasColumnName("term").HasMaxLength(VocabularyEntry.TermMaxLength).IsRequired();
        builder.Property(x => x.Reading).HasColumnName("reading").HasMaxLength(VocabularyEntry.ReadingMaxLength);
        builder.Property(x => x.AudioUrl).HasColumnName("audio_url").HasMaxLength(VocabularyEntry.AudioUrlMaxLength);
        builder.Property(x => x.IsPublished).HasColumnName("is_published").HasDefaultValue(false).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Cascade);
        // Term is deliberately not unique: homographs/readings can be separate entries with stable codes.
        builder.HasIndex(x => new { x.LanguageId, x.Code }).IsUnique().HasDatabaseName("ux_vocabulary_entries_language_id_code");
        builder.HasIndex(x => new { x.LanguageId, x.SortOrder, x.Id }).HasDatabaseName("ix_vocabulary_entries_language_id_sort_order_id");
        builder.HasData(VocabularySeedData.EntryRows);
    }
}
