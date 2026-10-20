using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAiConnectionCreationReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_connection_creations",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    lease_token = table.Column<Guid>(type: "uuid", nullable: true),
                    lease_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    response_json = table.Column<string>(type: "jsonb", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_connection_creations", x => new { x.user_id, x.request_id });
                    table.CheckConstraint("ck_ai_connection_creations_fingerprint", "fingerprint ~ '^[0-9a-f]{64}$'");
                    table.CheckConstraint("ck_ai_connection_creations_state", "(response_json IS NULL AND lease_token IS NOT NULL AND lease_expires_at IS NOT NULL)\nOR (response_json IS NOT NULL AND lease_token IS NULL AND lease_expires_at IS NULL)");
                    table.ForeignKey(
                        name: "FK_ai_connection_creations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ai_connection_creations_expires_at",
                table: "ai_connection_creations",
                column: "expires_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_connection_creations");
        }
    }
}
