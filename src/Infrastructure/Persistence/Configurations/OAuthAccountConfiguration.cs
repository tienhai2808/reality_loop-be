using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OAuthAccountConfiguration : IEntityTypeConfiguration<OAuthAccount>
{
    public void Configure(EntityTypeBuilder<OAuthAccount> builder)
    {
        builder.ToTable("oauth_accounts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasColumnName("provider")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnType("character varying(32)");

        builder.Property(x => x.ProviderAccountId)
            .HasColumnName("provider_account_id")
            .HasMaxLength(255)
            .IsRequired()
            .HasColumnType("character varying(255)");

        builder.HasIndex(x => new
            {
                x.Provider,
                x.ProviderAccountId
            })
            .IsUnique()
            .HasDatabaseName("ux_oauth_accounts_provider_provider_account_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasColumnType("timestamp with time zone");
    }
}