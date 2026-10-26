using Domain.Common;
using Domain.Pronunciation;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PronunciationRuleTranslationConfiguration : IEntityTypeConfiguration<PronunciationRuleTranslation>
{
    public void Configure(EntityTypeBuilder<PronunciationRuleTranslation> builder)
    {
        builder.ToTable("pronunciation_rule_translations", table =>
        {
            table.HasCheckConstraint("ck_pronunciation_rule_translations_positive_ids", "id > 0 AND pronunciation_rule_id > 0");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_version", "version > 0");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_name", "length(btrim(name)) > 0");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_explanation", "length(btrim(explanation)) > 0");
            table.HasCheckConstraint("ck_pronunciation_rule_translations_usage_note",
                "usage_note IS NULL OR length(btrim(usage_note)) > 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.PronunciationRuleId).HasColumnName("pronunciation_rule_id").IsRequired();
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(LanguageTag.MaxLength).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name")
            .HasMaxLength(PronunciationRuleTranslation.NameMaxLength).IsRequired();
        builder.Property(x => x.Explanation).HasColumnName("explanation")
            .HasMaxLength(PronunciationRuleTranslation.ExplanationMaxLength).IsRequired();
        builder.Property(x => x.UsageNote).HasColumnName("usage_note")
            .HasMaxLength(PronunciationRuleTranslation.UsageNoteMaxLength);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<PronunciationRule>().WithMany().HasForeignKey(x => x.PronunciationRuleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.PronunciationRuleId, x.Locale }).IsUnique()
            .HasDatabaseName("ux_pronunciation_rule_translations_rule_id_locale");
        builder.HasData(PronunciationSeedData.TranslationRows);
    }
}
