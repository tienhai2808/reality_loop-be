using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("ix_refresh_tokens_session_id");

        builder.HasIndex(x => x.SessionId, "UnconsumedSessionToken")
            .IsUnique()
            .HasDatabaseName("ux_refresh_tokens_session_unconsumed")
            .HasFilter("consumed_at IS NULL AND revoked_at IS NULL");

        builder.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(255)
            .IsRequired()
            .HasColumnType("character varying(255)");

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("ux_refresh_tokens_token_hash");

        builder.Property(x => x.ParentTokenId)
            .HasColumnName("parent_token_id");

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(x => x.ParentTokenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ConsumedAt)
            .HasColumnName("consumed_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamp with time zone");
    }
}
