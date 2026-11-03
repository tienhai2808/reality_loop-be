using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWritingSystemItemProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "writing_system_item_progress",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    learning_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_item_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Learning"),
                    last_studied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_item_progress", x => x.id);
                    table.CheckConstraint("ck_writing_system_item_progress_completion_state", "(status = 'Learning' AND completed_at IS NULL)\nOR (status = 'Completed' AND completed_at IS NOT NULL)");
                    table.CheckConstraint("ck_writing_system_item_progress_completion_time", "completed_at IS NULL OR (completed_at >= created_at AND completed_at <= last_studied_at)");
                    table.CheckConstraint("ck_writing_system_item_progress_positive_ids", "id > 0 AND learning_profile_id > 0 AND writing_system_item_id > 0");
                    table.CheckConstraint("ck_writing_system_item_progress_status", "status IN ('Learning', 'Completed')");
                    table.CheckConstraint("ck_writing_system_item_progress_study_time", "last_studied_at >= created_at AND last_studied_at <= updated_at");
                    table.CheckConstraint("ck_writing_system_item_progress_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_writing_system_item_progress_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_item_progress_learning_profiles_learning_pro~",
                        column: x => x.learning_profile_id,
                        principalTable: "learning_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_writing_system_item_progress_writing_system_items_writing_s~",
                        column: x => x.writing_system_item_id,
                        principalTable: "writing_system_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_writing_system_item_progress_writing_system_item_id",
                table: "writing_system_item_progress",
                column: "writing_system_item_id");

            migrationBuilder.CreateIndex(
                name: "ux_writing_system_item_progress_profile_id_item_id",
                table: "writing_system_item_progress",
                columns: new[] { "learning_profile_id", "writing_system_item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "writing_system_item_progress");
        }
    }
}
