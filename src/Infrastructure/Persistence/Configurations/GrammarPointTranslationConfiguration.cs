using Domain.Common;
using Domain.Grammar;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarPointTranslationConfiguration : IEntityTypeConfiguration<GrammarPointTranslation>
{
    public void Configure(EntityTypeBuilder<GrammarPointTranslation> builder)
    {
        builder.ToTable("grammar_point_translations", table =>
        {
            table.HasCheckConstraint("ck_grammar_point_translations_positive_ids", "id > 0 AND grammar_point_id > 0");
            table.HasCheckConstraint("ck_grammar_point_translations_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_point_translations_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_grammar_point_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_grammar_point_translations_name", "name ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_grammar_point_translations_explanation", "explanation ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_grammar_point_translations_usage_note",
                "usage_note IS NULL OR usage_note ~ '[^[:space:]]'");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.GrammarPointId).HasColumnName("grammar_point_id").IsRequired();
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(LanguageTag.MaxLength).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name")
            .HasMaxLength(GrammarPointTranslation.NameMaxLength).IsRequired();
        builder.Property(x => x.Explanation).HasColumnName("explanation")
            .HasMaxLength(GrammarPointTranslation.ExplanationMaxLength).IsRequired();
        builder.Property(x => x.UsageNote).HasColumnName("usage_note")
            .HasMaxLength(GrammarPointTranslation.UsageNoteMaxLength);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<GrammarPoint>().WithMany().HasForeignKey(x => x.GrammarPointId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.GrammarPointId, x.Locale }).IsUnique()
            .HasDatabaseName("ux_grammar_point_translations_point_id_locale");
        builder.HasData(GrammarSeedData.TranslationRows);
    }
}
