using Infrastructure.Persistence.Entities;
using Domain.AI;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class AiConnectionCreationConfiguration : IEntityTypeConfiguration<AiConnectionCreation>
{
    public void Configure(EntityTypeBuilder<AiConnectionCreation> builder)
    {
        builder.ToTable("ai_connection_creations", table =>
        {
            table.HasCheckConstraint("ck_ai_connection_creations_fingerprint", "fingerprint ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_ai_connection_creations_state", """
                (response_json IS NULL AND lease_token IS NOT NULL AND lease_expires_at IS NOT NULL)
                OR (response_json IS NOT NULL AND lease_token IS NULL AND lease_expires_at IS NULL)
                """);
        });
        builder.HasKey(x => new { x.UserId, x.RequestId });
        builder.Property(x => x.UserId).HasColumnName("user_id").ValueGeneratedNever();
        builder.Property(x => x.RequestId).HasColumnName("request_id").ValueGeneratedNever();
        builder.Property(x => x.Fingerprint).HasColumnName("fingerprint").HasMaxLength(64).IsRequired();
        builder.Property(x => x.LeaseToken).HasColumnName("lease_token");
        builder.Property(x => x.LeaseExpiresAt).HasColumnName("lease_expires_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.ResponseJson).HasColumnName("response_json").HasColumnType("jsonb");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamp with time zone");
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_ai_connection_creations_expires_at");
    }
}
