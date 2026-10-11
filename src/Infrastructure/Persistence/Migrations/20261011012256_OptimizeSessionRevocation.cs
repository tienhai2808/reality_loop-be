using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeSessionRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sessions_user_id_active_id",
                table: "sessions",
                columns: new[] { "user_id", "id" },
                descending: new[] { false, true },
                filter: "revoked_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_refresh_tokens_session_unconsumed",
                table: "refresh_tokens",
                column: "session_id",
                unique: true,
                filter: "consumed_at IS NULL AND revoked_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sessions_user_id_active_id",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "ux_refresh_tokens_session_unconsumed",
                table: "refresh_tokens");
        }
    }
}
