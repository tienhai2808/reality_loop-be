using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVocabularyLearningProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vocabulary_learning_progress",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    learning_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_sense_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    next_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    interval_minutes = table.Column<int>(type: "integer", nullable: false),
                    review_count = table.Column<long>(type: "bigint", nullable: false),
                    again_count = table.Column<long>(type: "bigint", nullable: false),
                    consecutive_recall_count = table.Column<long>(type: "bigint", nullable: false),
                    scheduler_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_learning_progress", x => x.id);
                    table.CheckConstraint("ck_vocabulary_progress_counts", "review_count >= 0 AND again_count >= 0 AND consecutive_recall_count >= 0 AND consecutive_recall_count <= review_count AND again_count <= review_count - consecutive_recall_count");
                    table.CheckConstraint("ck_vocabulary_progress_ids", "id > 0 AND learning_profile_id > 0 AND vocabulary_sense_id > 0");
                    table.CheckConstraint("ck_vocabulary_progress_state", "(status = 'New' AND review_count = 0 AND again_count = 0 AND consecutive_recall_count = 0\n    AND last_reviewed_at IS NULL AND interval_minutes = 0 AND next_review_at = created_at\n    AND scheduler_version IS NULL AND version = 1 AND updated_at = created_at)\nOR (status IN ('Learning', 'Reviewing') AND review_count > 0 AND last_reviewed_at IS NOT NULL\n    AND last_reviewed_at >= created_at AND last_reviewed_at = updated_at\n    AND interval_minutes > 0 AND next_review_at = last_reviewed_at + interval_minutes * interval '1 minute'\n    AND scheduler_version IS NOT NULL AND scheduler_version ~ '^[a-z][a-z0-9-]*$')");
                    table.CheckConstraint("ck_vocabulary_progress_status", "status IN ('New', 'Learning', 'Reviewing')");
                    table.CheckConstraint("ck_vocabulary_progress_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_vocabulary_progress_version", "version > 0 AND review_count < version");
                    table.ForeignKey(
                        name: "FK_vocabulary_learning_progress_learning_profiles_learning_pro~",
                        column: x => x.learning_profile_id,
                        principalTable: "learning_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vocabulary_learning_progress_vocabulary_senses_vocabulary_s~",
                        column: x => x.vocabulary_sense_id,
                        principalTable: "vocabulary_senses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_reviews",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    progress_id = table.Column<long>(type: "bigint", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    review_number = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    previous_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    previous_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    next_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    previous_interval_minutes = table.Column<int>(type: "integer", nullable: false),
                    interval_minutes = table.Column<int>(type: "integer", nullable: false),
                    again_count = table.Column<long>(type: "bigint", nullable: false),
                    consecutive_recall_count = table.Column<long>(type: "bigint", nullable: false),
                    progress_version_before = table.Column<long>(type: "bigint", nullable: false),
                    progress_version_after = table.Column<long>(type: "bigint", nullable: false),
                    scheduler_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_reviews", x => x.id);
                    table.CheckConstraint("ck_vocabulary_reviews_counts", "review_number > 0 AND again_count >= 0 AND consecutive_recall_count >= 0 AND consecutive_recall_count <= review_number AND again_count <= review_number - consecutive_recall_count");
                    table.CheckConstraint("ck_vocabulary_reviews_ids", "id > 0 AND progress_id > 0 AND request_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                    table.CheckConstraint("ck_vocabulary_reviews_immutable_state", "version = 1 AND reviewed_at = created_at AND updated_at = created_at");
                    table.CheckConstraint("ck_vocabulary_reviews_outcome", "(rating = 'Again' AND status = 'Learning' AND consecutive_recall_count = 0 AND again_count > 0)\nOR (rating <> 'Again' AND consecutive_recall_count > 0)");
                    table.CheckConstraint("ck_vocabulary_reviews_previous", "(review_number = 1 AND previous_status = 'New' AND previous_interval_minutes = 0)\nOR (review_number > 1 AND previous_status IN ('Learning', 'Reviewing') AND previous_interval_minutes > 0)");
                    table.CheckConstraint("ck_vocabulary_reviews_rating", "rating IN ('Again', 'Hard', 'Good', 'Easy')");
                    table.CheckConstraint("ck_vocabulary_reviews_schedule", "interval_minutes > 0 AND next_review_at = reviewed_at + interval_minutes * interval '1 minute'");
                    table.CheckConstraint("ck_vocabulary_reviews_scheduler", "scheduler_version ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_vocabulary_reviews_status", "previous_status IN ('New', 'Learning', 'Reviewing') AND status IN ('Learning', 'Reviewing')");
                    table.CheckConstraint("ck_vocabulary_reviews_versions", "progress_version_before > 0 AND review_number <= progress_version_before AND progress_version_after > progress_version_before AND progress_version_after - progress_version_before = 1");
                    table.ForeignKey(
                        name: "FK_vocabulary_reviews_vocabulary_learning_progress_progress_id",
                        column: x => x.progress_id,
                        principalTable: "vocabulary_learning_progress",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_progress_profile_due_id",
                table: "vocabulary_learning_progress",
                columns: new[] { "learning_profile_id", "next_review_at", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_progress_sense",
                table: "vocabulary_learning_progress",
                column: "vocabulary_sense_id");

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_progress_profile_sense",
                table: "vocabulary_learning_progress",
                columns: new[] { "learning_profile_id", "vocabulary_sense_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_reviews_progress_number",
                table: "vocabulary_reviews",
                columns: new[] { "progress_id", "review_number" },
                unique: true,
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_reviews_progress_request",
                table: "vocabulary_reviews",
                columns: new[] { "progress_id", "request_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_reviews_progress_version",
                table: "vocabulary_reviews",
                columns: new[] { "progress_id", "progress_version_before" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vocabulary_reviews");

            migrationBuilder.DropTable(
                name: "vocabulary_learning_progress");
        }
    }
}
