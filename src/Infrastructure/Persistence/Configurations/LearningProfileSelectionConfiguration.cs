using Domain.LearningProfiles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LearningProfileSelectionConfiguration : IEntityTypeConfiguration<LearningProfileSelection>
{
    public void Configure(EntityTypeBuilder<LearningProfileSelection> builder)
    {
        builder.ToTable("learning_profile_selections", table =>
            table.HasCheckConstraint("ck_learning_profile_selections_positive_ids", "user_id > 0 AND active_profile_id > 0"));

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).HasColumnName("user_id").ValueGeneratedNever();
        builder.Property(x => x.ActiveProfileId).HasColumnName("active_profile_id").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<User>().WithOne().HasForeignKey<LearningProfileSelection>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<LearningProfile>().WithMany()
            .HasForeignKey(x => new { x.UserId, x.ActiveProfileId })
            .HasPrincipalKey(x => new { x.UserId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
