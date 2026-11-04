using Domain.Vocabulary;
using Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VocabularySenseConfiguration : IEntityTypeConfiguration<VocabularySense>
{
    public void Configure(EntityTypeBuilder<VocabularySense> builder)
    {
        builder.ToTable("vocabulary_senses", table =>
        {
            table.HasCheckConstraint("ck_vocabulary_senses_positive_ids", "id > 0 AND vocabulary_entry_id > 0");
            table.HasCheckConstraint("ck_vocabulary_senses_version", "version > 0");
            table.HasCheckConstraint("ck_vocabulary_senses_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_vocabulary_senses_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_vocabulary_senses_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_vocabulary_senses_part_of_speech",
                $"part_of_speech IN ({string.Join(", ", Enum.GetNames<PartOfSpeech>().Select(value => $"'{value}'"))})");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(VocabularySense.CodeMaxLength).IsRequired();
        builder.Property(x => x.PartOfSpeech).HasColumnName("part_of_speech").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<VocabularyEntry>().WithMany().HasForeignKey(x => x.VocabularyEntryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.VocabularyEntryId, x.Code }).IsUnique().HasDatabaseName("ux_vocabulary_senses_entry_id_code");
        builder.HasIndex(x => new { x.VocabularyEntryId, x.SortOrder, x.Id }).HasDatabaseName("ix_vocabulary_senses_entry_id_sort_order_id");
        builder.HasData(VocabularySeedData.SenseRows);
    }
}
