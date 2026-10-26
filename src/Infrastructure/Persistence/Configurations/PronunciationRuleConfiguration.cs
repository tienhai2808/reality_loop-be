using Domain.Languages;
using Domain.Pronunciation;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PronunciationRuleConfiguration : IEntityTypeConfiguration<PronunciationRule>
{
    public void Configure(EntityTypeBuilder<PronunciationRule> builder)
    {
        builder.ToTable("pronunciation_rules", table =>
        {
            table.HasCheckConstraint("ck_pronunciation_rules_positive_ids", "id > 0 AND language_id > 0");
            table.HasCheckConstraint("ck_pronunciation_rules_version", "version > 0");
            table.HasCheckConstraint("ck_pronunciation_rules_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_pronunciation_rules_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_pronunciation_rules_native_name", "length(btrim(native_name)) > 0");
            table.HasCheckConstraint("ck_pronunciation_rules_sort_order", "sort_order >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(PronunciationRule.CodeMaxLength).IsRequired();
        builder.Property(x => x.NativeName).HasColumnName("native_name")
            .HasMaxLength(PronunciationRule.NativeNameMaxLength).IsRequired();
        builder.Property(x => x.IsPublished).HasColumnName("is_published")
            .HasDefaultValue(false).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.LanguageId, x.Code }).IsUnique()
            .HasDatabaseName("ux_pronunciation_rules_language_id_code");
        builder.HasIndex(x => new { x.LanguageId, x.SortOrder, x.Id })
            .HasDatabaseName("ix_pronunciation_rules_language_id_sort_order_id");
        builder.HasData(PronunciationSeedData.RuleRows);
    }
}
