using Domain.Common;
using Domain.Vocabulary;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VocabularySenseTranslationConfiguration : IEntityTypeConfiguration<VocabularySenseTranslation>
{
    public void Configure(EntityTypeBuilder<VocabularySenseTranslation> builder)
    {
        builder.ToTable("vocabulary_sense_translations", table =>
        {
            table.HasCheckConstraint("ck_vocabulary_sense_translations_positive_ids", "id > 0 AND vocabulary_sense_id > 0");
            table.HasCheckConstraint("ck_vocabulary_sense_translations_version", "version > 0");
            table.HasCheckConstraint("ck_vocabulary_sense_translations_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_vocabulary_sense_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_vocabulary_sense_translations_meaning", "meaning ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_vocabulary_sense_translations_usage_note", "usage_note IS NULL OR usage_note ~ '[^[:space:]]'");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.VocabularySenseId).HasColumnName("vocabulary_sense_id").IsRequired();
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(LanguageTag.MaxLength).IsRequired();
        builder.Property(x => x.Meaning).HasColumnName("meaning").HasMaxLength(VocabularySenseTranslation.MeaningMaxLength).IsRequired();
        builder.Property(x => x.UsageNote).HasColumnName("usage_note").HasMaxLength(VocabularySenseTranslation.UsageNoteMaxLength);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<VocabularySense>().WithMany().HasForeignKey(x => x.VocabularySenseId).OnDelete(DeleteBehavior.Cascade);
        // Locales are explanation languages, not foreign keys to the target-language catalog.
        builder.HasIndex(x => new { x.VocabularySenseId, x.Locale }).IsUnique()
            .HasDatabaseName("ux_vocabulary_sense_translations_sense_id_locale");
        builder.HasData(VocabularySeedData.TranslationRows);
    }
}
