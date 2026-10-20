using Domain.AI;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class AiConnectionConfiguration : IEntityTypeConfiguration<AiConnection>
{
    public void Configure(EntityTypeBuilder<AiConnection> builder)
    {
        builder.ToTable("ai_connections", table =>
        {
            table.HasCheckConstraint("ck_ai_connections_positive_ids", "id > 0 AND user_id > 0");
            table.HasCheckConstraint("ck_ai_connections_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("ck_ai_connections_version", "version > 0");
            table.HasCheckConstraint("ck_ai_connections_timestamps", "updated_at >= created_at");
            table.HasCheckConstraint("ck_ai_connections_provider", $"provider IN ({EnumValues<AiProvider>()})");
            table.HasCheckConstraint("ck_ai_connections_status", $"status IN ({EnumValues<AiConnectionStatus>()})");
            table.HasCheckConstraint("ck_ai_connections_failure_code",
                $"last_failure_code IS NULL OR last_failure_code IN ({EnumValues<AiFailureCode>()})");
            table.HasCheckConstraint("ck_ai_connections_required_text",
                "length(btrim(display_name)) > 0 AND length(btrim(model_id)) > 0 AND length(btrim(encrypted_api_key)) > 0");
            table.HasCheckConstraint("ck_ai_connections_key_hint", "char_length(api_key_last4) = 4 AND octet_length(api_key_last4) = 4 AND api_key_last4 !~ '[[:space:][:cntrl:]]'");
            table.HasCheckConstraint("ck_ai_connections_state", """
                (status = 'Ready' AND retry_after_at IS NULL AND last_failure_code IS NULL)
                OR (status = 'CoolingDown' AND retry_after_at IS NOT NULL AND last_failure_code IS NOT NULL
                    AND last_failure_code IN ('QuotaExceeded', 'RateLimited', 'ProviderUnavailable'))
                OR (status = 'NeedsAttention' AND retry_after_at IS NULL AND last_failure_code IS NOT NULL)
                """);
            table.HasCheckConstraint("ck_ai_connections_retry_time", "retry_after_at IS NULL OR retry_after_at > created_at");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.Provider).HasColumnName("provider")
            .HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.DisplayName).HasColumnName("display_name")
            .HasMaxLength(AiConnection.DisplayNameMaxLength).IsRequired();
        builder.Property(x => x.EncryptedApiKey).HasColumnName("encrypted_api_key").HasColumnType("text").IsRequired();
        builder.Property(x => x.ApiKeyLast4).HasColumnName("api_key_last4")
            .HasMaxLength(AiConnection.ApiKeyLast4Length).IsRequired();
        builder.Property(x => x.ModelId).HasColumnName("model_id").HasMaxLength(AiConnection.ModelIdMaxLength).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>()
            .HasMaxLength(32).HasDefaultValue(AiConnectionStatus.Ready).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.RetryAfterAt).HasColumnName("retry_after_at").HasColumnType("timestamp with time zone");
        builder.Property(x => x.LastFailureCode).HasColumnName("last_failure_code").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Version).HasColumnName("version").HasDefaultValue(1L).ValueGeneratedNever().IsConcurrencyToken();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        // Covers user ownership/listing and ordered fallback. Ties are resolved by the Snowflake ID.
        builder.HasIndex(x => new { x.UserId, x.SortOrder, x.Id }).HasDatabaseName("ix_ai_connections_user_id_sort_order_id");
    }

    private static string EnumValues<TEnum>() where TEnum : struct, Enum =>
        string.Join(", ", Enum.GetNames<TEnum>().Select(value => $"'{value}'"));
}
