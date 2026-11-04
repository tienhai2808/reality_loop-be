using Domain.Handwriting;
using Domain.WritingSystems;
using Infrastructure.Persistence.Seeding;
using Infrastructure.Persistence.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class HandwritingTemplateConfiguration : IEntityTypeConfiguration<HandwritingTemplate>
{
    public void Configure(EntityTypeBuilder<HandwritingTemplate> builder)
    {
        builder.ToTable("handwriting_templates", table =>
        {
            table.HasCheckConstraint("ck_handwriting_templates_positive_ids", "id > 0 AND writing_system_item_id > 0");
            table.HasCheckConstraint("ck_handwriting_templates_revision", "revision > 0");
            table.HasCheckConstraint("ck_handwriting_templates_version", "version > 0");
            table.HasCheckConstraint("ck_handwriting_templates_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_handwriting_templates_text", """
                symbol ~ '[^[:space:]]' AND source_revision ~ '[^[:space:]]'
                AND attribution ~ '[^[:space:]]' AND license ~ '[^[:space:]]'
                """);
            table.HasCheckConstraint("ck_handwriting_templates_urls", """
                source_url ~ '^https://[^/@?#[:space:]\\]+(/[^#[:space:]\\]*)?$'
                AND license_url ~ '^https://[^/@?#[:space:]\\]+(/[^#[:space:]\\]*)?$'
                """);
            table.HasCheckConstraint("ck_handwriting_templates_drawing_format", $"drawing_format_version = {InkDrawing.FormatVersion}");
            table.HasCheckConstraint("ck_handwriting_templates_drawing", HandwritingJsonConstraints.Drawing("drawing", true));
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.WritingSystemItemId).HasColumnName("writing_system_item_id").IsRequired();
        builder.Property(x => x.Revision).HasColumnName("revision").IsRequired();
        builder.Property(x => x.Symbol).HasColumnName("symbol").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Drawing).HasColumnName("drawing").HasColumnType("jsonb")
            .HasConversion(x => HandwritingJson.WriteDrawing(x), x => HandwritingJson.ReadDrawing(x)).IsRequired();
        builder.Property(x => x.DrawingFormatVersion).HasColumnName("drawing_format_version").IsRequired();
        builder.Property(x => x.SourceUrl).HasColumnName("source_url").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.SourceRevision).HasColumnName("source_revision").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Attribution).HasColumnName("attribution").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.License).HasColumnName("license").HasMaxLength(128).IsRequired();
        builder.Property(x => x.LicenseUrl).HasColumnName("license_url").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.IsPublished).HasColumnName("is_published").IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasOne<WritingSystemItem>().WithMany().HasForeignKey(x => x.WritingSystemItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.WritingSystemItemId, x.Revision }).IsUnique()
            .HasDatabaseName("ux_handwriting_templates_item_revision");
        builder.HasData(HandwritingSeedData.Templates);
    }
}
