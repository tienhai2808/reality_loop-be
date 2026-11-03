using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWritingSystemPracticeSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "writing_system_practice_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    learning_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_id = table.Column<long>(type: "bigint", nullable: false),
                    question_count = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "InProgress"),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_practice_sessions", x => x.id);
                    table.CheckConstraint("ck_ws_practice_sessions_end_state", "(status = 'InProgress' AND ended_at IS NULL)\nOR (status IN ('Completed', 'Abandoned') AND ended_at IS NOT NULL\n    AND ended_at >= created_at AND ended_at = updated_at)");
                    table.CheckConstraint("ck_ws_practice_sessions_positive_ids", "id > 0 AND learning_profile_id > 0 AND writing_system_id > 0");
                    table.CheckConstraint("ck_ws_practice_sessions_question_count", "question_count BETWEEN 1 AND 20");
                    table.CheckConstraint("ck_ws_practice_sessions_status", "status IN ('InProgress', 'Completed', 'Abandoned')");
                    table.CheckConstraint("ck_ws_practice_sessions_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_ws_practice_sessions_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_practice_sessions_learning_profiles_learning~",
                        column: x => x.learning_profile_id,
                        principalTable: "learning_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_writing_system_practice_sessions_writing_systems_writing_sy~",
                        column: x => x.writing_system_id,
                        principalTable: "writing_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "writing_system_practice_session_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    session_id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_item_id = table.Column<long>(type: "bigint", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    symbol = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    transcription_system = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    options = table.Column<string[]>(type: "character varying(200)[]", nullable: false),
                    correct_option_index = table.Column<int>(type: "integer", nullable: false),
                    selected_option_index = table.Column<int>(type: "integer", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_practice_session_items", x => x.id);
                    table.CheckConstraint("ck_ws_practice_items_answer_state", "(selected_option_index IS NULL AND answered_at IS NULL AND version = 1 AND updated_at = created_at)\nOR (selected_option_index IS NOT NULL AND answered_at IS NOT NULL\n    AND selected_option_index >= 0 AND selected_option_index < cardinality(options)\n    AND answered_at >= created_at AND answered_at = updated_at AND version = 2)");
                    table.CheckConstraint("ck_ws_practice_items_correct_option", "correct_option_index >= 0 AND correct_option_index < cardinality(options)");
                    table.CheckConstraint("ck_ws_practice_items_options_distinct", "options[1] <> options[2]\nAND (cardinality(options) < 3 OR (options[1] <> options[3] AND options[2] <> options[3]))\nAND (cardinality(options) < 4 OR (options[1] <> options[4] AND options[2] <> options[4] AND options[3] <> options[4]))");
                    table.CheckConstraint("ck_ws_practice_items_options_shape", "CASE WHEN array_ndims(options) = 1 AND array_lower(options, 1) = 1\n    THEN cardinality(options) BETWEEN 2 AND 4\n        AND array_position(options, NULL) IS NULL\n    ELSE FALSE END");
                    table.CheckConstraint("ck_ws_practice_items_options_text", "options[1] ~ '[^[:space:]]' AND options[2] ~ '[^[:space:]]'\nAND (cardinality(options) < 3 OR options[3] ~ '[^[:space:]]')\nAND (cardinality(options) < 4 OR options[4] ~ '[^[:space:]]')");
                    table.CheckConstraint("ck_ws_practice_items_position", "position >= 0 AND position < 20");
                    table.CheckConstraint("ck_ws_practice_items_positive_ids", "id > 0 AND session_id > 0 AND writing_system_item_id > 0");
                    table.CheckConstraint("ck_ws_practice_items_symbol", "length(btrim(symbol)) > 0");
                    table.CheckConstraint("ck_ws_practice_items_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_ws_practice_items_transcription_system", "transcription_system ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_ws_practice_items_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_practice_session_items_writing_system_items_~",
                        column: x => x.writing_system_item_id,
                        principalTable: "writing_system_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_writing_system_practice_session_items_writing_system_practi~",
                        column: x => x.session_id,
                        principalTable: "writing_system_practice_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ws_practice_items_writing_system_item_id",
                table: "writing_system_practice_session_items",
                column: "writing_system_item_id");

            migrationBuilder.CreateIndex(
                name: "ux_ws_practice_items_session_id_item_id",
                table: "writing_system_practice_session_items",
                columns: new[] { "session_id", "writing_system_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_ws_practice_items_session_id_position",
                table: "writing_system_practice_session_items",
                columns: new[] { "session_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ws_practice_sessions_profile_id_id",
                table: "writing_system_practice_sessions",
                columns: new[] { "learning_profile_id", "id" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_ws_practice_sessions_writing_system_id",
                table: "writing_system_practice_sessions",
                column: "writing_system_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "writing_system_practice_session_items");

            migrationBuilder.DropTable(
                name: "writing_system_practice_sessions");
        }
    }
}
