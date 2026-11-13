using Domain.Grammar;
using Domain.GrammarPractice;
using Infrastructure.Persistence.Seeding.GrammarPractice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GrammarExerciseConfiguration : IEntityTypeConfiguration<GrammarExercise>
{
    public void Configure(EntityTypeBuilder<GrammarExercise> builder)
    {
        builder.ToTable("grammar_exercises", table =>
        {
            table.HasCheckConstraint("ck_grammar_exercises_ids", "id > 0 AND grammar_point_id > 0");
            table.HasCheckConstraint("ck_grammar_exercises_version", "version > 0");
            table.HasCheckConstraint("ck_grammar_exercises_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_grammar_exercises_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_grammar_exercises_code", "code ~ '^[a-z][a-z0-9-]*$'");
            table.HasCheckConstraint("ck_grammar_exercises_prompt", GrammarPracticeConstraints.Prompt);
            table.HasCheckConstraint("ck_grammar_exercises_options_shape", GrammarPracticeConstraints.OptionsShape);
            table.HasCheckConstraint("ck_grammar_exercises_options_text", GrammarPracticeConstraints.OptionsText);
            table.HasCheckConstraint("ck_grammar_exercises_options_distinct", GrammarPracticeConstraints.OptionsDistinct);
            table.HasCheckConstraint("ck_grammar_exercises_answer", "correct_option_index >= 0 AND correct_option_index < cardinality(options)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.GrammarPointId).HasColumnName("grammar_point_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Prompt).HasColumnName("prompt").HasMaxLength(GrammarExercise.PromptMaxLength).IsRequired();
        builder.Property(x => x.Options).HasField("_options").UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("options").HasColumnType("character varying(250)[]").IsRequired();
        builder.Property(x => x.CorrectOptionIndex).HasColumnName("correct_option_index").IsRequired();
        builder.Property(x => x.IsPublished).HasColumnName("is_published").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasOne<GrammarPoint>().WithMany().HasForeignKey(x => x.GrammarPointId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.GrammarPointId, x.Code }).IsUnique().HasDatabaseName("ux_grammar_exercises_point_code");
        builder.HasIndex(x => new { x.GrammarPointId, x.SortOrder, x.Id }).HasDatabaseName("ix_grammar_exercises_point_order_id");
        builder.HasAlternateKey(x => new { x.Id, x.GrammarPointId }).HasName("ak_grammar_exercises_id_point");
        builder.HasData(GrammarPracticeSeedData.ExerciseRows);
    }
}
