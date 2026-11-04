using Domain.Handwriting;
using Infrastructure.Persistence.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class HandwritingPracticeCharacterConfiguration : IEntityTypeConfiguration<HandwritingPracticeCharacter>
{
    public void Configure(EntityTypeBuilder<HandwritingPracticeCharacter> builder)
    {
        builder.ToTable("handwriting_practice_characters", table =>
        {
            table.HasCheckConstraint("ck_handwriting_characters_positive_ids", "id > 0 AND attempt_id > 0 AND template_id > 0");
            table.HasCheckConstraint("ck_handwriting_characters_position", $"position BETWEEN 0 AND {HandwritingPracticeAttempt.MaxCharacters - 1}");
            table.HasCheckConstraint("ck_handwriting_characters_immutable_state", "version = 1 AND updated_at = created_at");
            table.HasCheckConstraint("ck_handwriting_characters_drawing_format", $"drawing_format_version = {InkDrawing.FormatVersion}");
            table.HasCheckConstraint("ck_handwriting_characters_drawing", HandwritingJsonConstraints.Drawing("drawing", false));
            table.HasCheckConstraint("ck_handwriting_characters_feedback", HandwritingJsonConstraints.Feedback);
            table.HasCheckConstraint("ck_handwriting_characters_feedback_format", $"feedback_format_version = {HandwritingFeedback.FormatVersion}");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.AttemptId).HasColumnName("attempt_id").IsRequired();
        builder.Property(x => x.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(x => x.Position).HasColumnName("position").IsRequired();
        builder.Property(x => x.Drawing).HasColumnName("drawing").HasColumnType("jsonb")
            .HasConversion(x => HandwritingJson.WriteDrawing(x), x => HandwritingJson.ReadDrawing(x)).IsRequired();
        builder.Property(x => x.DrawingFormatVersion).HasColumnName("drawing_format_version").IsRequired();
        builder.Property(x => x.Feedback).HasColumnName("feedback").HasColumnType("jsonb")
            .HasConversion(x => HandwritingJson.WriteFeedback(x), x => HandwritingJson.ReadFeedback(x)).IsRequired();
        builder.Property(x => x.FeedbackFormatVersion).HasColumnName("feedback_format_version").IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();
        builder.HasOne<HandwritingTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.AttemptId, x.Position }).IsUnique()
            .HasDatabaseName("ux_handwriting_characters_attempt_position");
        // Repeated letters are valid; do not make (attempt_id, template_id) unique.
        builder.HasIndex(x => x.TemplateId).HasDatabaseName("ix_handwriting_characters_template_id");
    }
}
