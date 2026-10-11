using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Id })
            .IsDescending(false, true)
            .HasFilter("revoked_at IS NULL")
            .HasDatabaseName("ix_sessions_user_id_active_id");

        builder.Property(x => x.DeviceType)
            .HasColumnName("device_type")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnType("character varying(32)");

        builder.Property(x => x.DeviceName)
            .HasColumnName("device_name")
            .HasMaxLength(255)
            .HasColumnType("character varying(255)");

        builder.Property(x => x.Os)
            .HasColumnName("os")
            .HasMaxLength(255)
            .HasColumnType("character varying(255)");

        builder.Property(x => x.Browser)
            .HasColumnName("browser")
            .HasMaxLength(255)
            .HasColumnType("character varying(255)");

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(2)
            .HasColumnType("character varying(2)");

        builder.Property(x => x.Region)
            .HasColumnName("region")
            .HasMaxLength(150)
            .HasColumnType("character varying(150)");

        builder.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(150)
            .HasColumnType("character varying(150)");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.LastSeenAt)
            .HasColumnName("last_seen_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamp with time zone");

        builder.HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.RefreshTokens)
            .HasField("_refreshTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
