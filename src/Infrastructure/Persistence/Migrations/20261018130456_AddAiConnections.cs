using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAiConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_connections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    display_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    encrypted_api_key = table.Column<string>(type: "text", nullable: false),
                    api_key_last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    model_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Ready"),
                    retry_after_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_failure_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_connections", x => x.id);
                    table.CheckConstraint("ck_ai_connections_failure_code", "last_failure_code IS NULL OR last_failure_code IN ('InvalidApiKey', 'QuotaExceeded', 'RateLimited', 'ProviderUnavailable', 'ModelUnavailable')");
                    table.CheckConstraint("ck_ai_connections_key_hint", "char_length(api_key_last4) = 4 AND octet_length(api_key_last4) = 4 AND api_key_last4 !~ '[[:space:][:cntrl:]]'");
                    table.CheckConstraint("ck_ai_connections_positive_ids", "id > 0 AND user_id > 0");
                    table.CheckConstraint("ck_ai_connections_provider", "provider IN ('OpenAI', 'Anthropic', 'Google')");
                    table.CheckConstraint("ck_ai_connections_required_text", "length(btrim(display_name)) > 0 AND length(btrim(model_id)) > 0 AND length(btrim(encrypted_api_key)) > 0");
                    table.CheckConstraint("ck_ai_connections_retry_time", "retry_after_at IS NULL OR retry_after_at > created_at");
                    table.CheckConstraint("ck_ai_connections_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_ai_connections_state", "(status = 'Ready' AND retry_after_at IS NULL AND last_failure_code IS NULL)\nOR (status = 'CoolingDown' AND retry_after_at IS NOT NULL AND last_failure_code IS NOT NULL\n    AND last_failure_code IN ('QuotaExceeded', 'RateLimited', 'ProviderUnavailable'))\nOR (status = 'NeedsAttention' AND retry_after_at IS NULL AND last_failure_code IS NOT NULL)");
                    table.CheckConstraint("ck_ai_connections_status", "status IN ('Ready', 'CoolingDown', 'NeedsAttention')");
                    table.CheckConstraint("ck_ai_connections_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_ai_connections_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_ai_connections_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ai_connections_user_id_sort_order_id",
                table: "ai_connections",
                columns: new[] { "user_id", "sort_order", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_connections");
        }
    }
}
