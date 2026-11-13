using Domain.GrammarPractice;
using Infrastructure.Persistence.Seeding.GrammarPractice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarExerciseTranslationConfiguration : IEntityTypeConfiguration<GrammarExerciseTranslation>
{
    public void Configure(EntityTypeBuilder<GrammarExerciseTranslation> builder)
    {
        builder.ToTable("grammar_exercise_translations", table =>
        {
            table.HasCheckConstraint("ck_grammar_exercise_translations_ids", "id > 0 AND grammar_exercise_id > 0");
            table.HasCheckConstraint("ck_grammar_exercise_translations_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_exercise_translations_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_grammar_exercise_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
            table.HasCheckConstraint("ck_grammar_exercise_translations_text", "instruction ~ '[^[:space:]]' AND explanation ~ '[^[:space:]]'");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.GrammarExerciseId).HasColumnName("grammar_exercise_id").IsRequired();
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(35).IsRequired();
        builder.Property(x => x.Instruction).HasColumnName("instruction").HasMaxLength(GrammarExerciseTranslation.InstructionMaxLength).IsRequired();
        builder.Property(x => x.Explanation).HasColumnName("explanation").HasMaxLength(GrammarExerciseTranslation.ExplanationMaxLength).IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasOne<GrammarExercise>().WithMany().HasForeignKey(x => x.GrammarExerciseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.GrammarExerciseId, x.Locale }).IsUnique().HasDatabaseName("ux_grammar_exercise_translations_exercise_locale");
        builder.HasData(GrammarPracticeSeedData.TranslationRows);
    }
}
