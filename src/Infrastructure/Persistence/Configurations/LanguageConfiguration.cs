using Domain.Languages;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages", table =>
        {
            table.HasCheckConstraint("ck_languages_positive_ids", "id > 0");
            table.HasCheckConstraint("ck_languages_version", "version > 0");
            table.HasCheckConstraint("ck_languages_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_languages_code", "code ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_languages_native_name", "length(btrim(native_name)) > 0");
            table.HasCheckConstraint("ck_languages_sort_order", "sort_order >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(35).IsRequired();
        builder.Property(x => x.NativeName).HasColumnName("native_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(false).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ux_languages_code");
        builder.HasIndex(x => new { x.IsEnabled, x.SortOrder, x.Id })
            .HasDatabaseName("ix_languages_is_enabled_sort_order_id");
        builder.HasData(FoundationSeedData.LanguageRows);
    }
}
