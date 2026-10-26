using Domain.Languages;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LanguageTranslationConfiguration : IEntityTypeConfiguration<LanguageTranslation>
{
    public void Configure(EntityTypeBuilder<LanguageTranslation> builder)
    {
        builder.ToTable("language_translations", table =>
        {
            table.HasCheckConstraint("ck_language_translations_positive_ids", "id > 0 AND language_id > 0");
            table.HasCheckConstraint("ck_language_translations_version", "version > 0");
            table.HasCheckConstraint("ck_language_translations_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_language_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_language_translations_name", "length(btrim(name)) > 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LanguageId).HasColumnName("language_id").IsRequired();
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(35).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(2000);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.LanguageId, x.Locale }).IsUnique().HasDatabaseName("ux_language_translations_language_id_locale");
        builder.HasData(FoundationSeedData.LanguageTranslationRows);
    }
}
