using Domain.Languages;
using Domain.Grammar;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarPointConfiguration : IEntityTypeConfiguration<GrammarPoint>
{
    public void Configure(EntityTypeBuilder<GrammarPoint> builder)
    {
        builder.ToTable("grammar_points", table =>
        {
            table.HasCheckConstraint("ck_grammar_points_positive_ids", "id > 0 AND language_id > 0");
            table.HasCheckConstraint("ck_grammar_points_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_points_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_grammar_points_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_grammar_points_native_name", "native_name ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_grammar_points_pattern", "pattern ~ '[^[:space:]]'");
            table.HasCheckConstraint("ck_grammar_points_sort_order", "sort_order >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(GrammarPoint.CodeMaxLength).IsRequired();
        builder.Property(x => x.NativeName).HasColumnName("native_name")
            .HasMaxLength(GrammarPoint.NativeNameMaxLength).IsRequired();
        builder.Property(x => x.Pattern).HasColumnName("pattern").HasMaxLength(GrammarPoint.PatternMaxLength).IsRequired();
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
            .HasDatabaseName("ux_grammar_points_language_id_code");
        builder.HasIndex(x => new { x.LanguageId, x.SortOrder, x.Id })
            .HasDatabaseName("ix_grammar_points_language_id_sort_order_id");
        builder.HasData(GrammarSeedData.PointRows);
    }
}
