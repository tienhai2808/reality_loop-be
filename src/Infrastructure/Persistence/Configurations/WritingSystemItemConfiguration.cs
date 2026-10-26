using Domain.WritingSystems;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WritingSystemItemConfiguration : IEntityTypeConfiguration<WritingSystemItem>
{
    public void Configure(EntityTypeBuilder<WritingSystemItem> builder)
    {
        builder.ToTable("writing_system_items", table =>
        {
            table.HasCheckConstraint("ck_writing_system_items_positive_ids", "id > 0 AND writing_system_id > 0");
            table.HasCheckConstraint("ck_writing_system_items_version", "version > 0");
            table.HasCheckConstraint("ck_writing_system_items_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_writing_system_items_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_writing_system_items_symbol", "length(btrim(symbol)) > 0");
            table.HasCheckConstraint("ck_writing_system_items_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_writing_system_items_grid", "(row_index IS NULL AND column_index IS NULL) OR (row_index IS NOT NULL AND column_index IS NOT NULL AND row_index >= 0 AND column_index >= 0)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.WritingSystemId).HasColumnName("writing_system_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Symbol).HasColumnName("symbol").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.RowIndex).HasColumnName("row_index");
        builder.Property(x => x.ColumnIndex).HasColumnName("column_index");
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L)
            .ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<WritingSystem>().WithMany().HasForeignKey(x => x.WritingSystemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.WritingSystemId, x.Code }).IsUnique().HasDatabaseName("ux_writing_system_items_writing_system_id_code");
        builder.HasIndex(x => new { x.WritingSystemId, x.SortOrder, x.Id })
            .HasDatabaseName("ix_writing_system_items_writing_system_id_sort_order_id");
        builder.HasData(FoundationSeedData.WritingSystemItemRows);
    }
}
