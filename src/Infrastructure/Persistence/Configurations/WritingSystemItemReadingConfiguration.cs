using Domain.WritingSystems;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WritingSystemItemReadingConfiguration : IEntityTypeConfiguration<WritingSystemItemReading>
{
    public void Configure(EntityTypeBuilder<WritingSystemItemReading> builder)
    {
        builder.ToTable("writing_system_item_readings", table =>
        {
            table.HasCheckConstraint("ck_writing_system_item_readings_positive_ids", "id > 0 AND writing_system_item_id > 0");
            table.HasCheckConstraint("ck_writing_system_item_readings_version", "version > 0");
            table.HasCheckConstraint("ck_writing_system_item_readings_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_writing_system_item_readings_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_writing_system_item_readings_transcription", "length(btrim(transcription)) > 0");
            table.HasCheckConstraint("ck_writing_system_item_readings_transcription_system", "transcription_system ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_writing_system_item_readings_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_writing_system_item_readings_audio_url", "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.WritingSystemItemId).HasColumnName("writing_system_item_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Transcription).HasColumnName("transcription").HasMaxLength(200).IsRequired();
        builder.Property(x => x.TranscriptionSystem).HasColumnName("transcription_system").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AudioUrl).HasColumnName("audio_url").HasMaxLength(1024);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<WritingSystemItem>().WithMany().HasForeignKey(x => x.WritingSystemItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.WritingSystemItemId, x.Code }).IsUnique().HasDatabaseName("ux_writing_system_item_readings_writing_system_item_id_code");
        builder.HasIndex(x => new { x.WritingSystemItemId, x.SortOrder, x.Id })
            .HasDatabaseName("ix_writing_item_readings_item_id_sort_order_id");
        builder.HasData(FoundationSeedData.WritingSystemItemReadingRows);
    }
}
