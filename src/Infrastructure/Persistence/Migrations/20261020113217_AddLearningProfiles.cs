using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "learning_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    target_language = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    native_language = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    explanation_language = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    proficiency_level = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    primary_goal = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    goal_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_profiles", x => x.id);
                    table.UniqueConstraint("ak_learning_profiles_user_id_id", x => new { x.user_id, x.id });
                    table.CheckConstraint("ck_learning_profiles_language_tags", "target_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'\nAND native_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'\nAND explanation_language ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_learning_profiles_positive_ids", "id > 0 AND user_id > 0");
                    table.CheckConstraint("ck_learning_profiles_primary_goal", "primary_goal IN ('Travel', 'Work', 'Study', 'DailyLife', 'Social', 'Media', 'Relocation', 'Other')");
                    table.CheckConstraint("ck_learning_profiles_proficiency_level", "proficiency_level IN ('Beginner', 'Elementary', 'Intermediate', 'UpperIntermediate', 'Advanced')");
                    table.CheckConstraint("ck_learning_profiles_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_learning_profiles_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_learning_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_profile_selections",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    active_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_profile_selections", x => x.user_id);
                    table.CheckConstraint("ck_learning_profile_selections_positive_ids", "user_id > 0 AND active_profile_id > 0");
                    table.ForeignKey(
                        name: "FK_learning_profile_selections_learning_profiles_user_id_activ~",
                        columns: x => new { x.user_id, x.active_profile_id },
                        principalTable: "learning_profiles",
                        principalColumns: new[] { "user_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_learning_profile_selections_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_learning_profile_selections_user_id_active_profile_id",
                table: "learning_profile_selections",
                columns: new[] { "user_id", "active_profile_id" });

            migrationBuilder.CreateIndex(
                name: "ux_learning_profiles_user_id_target_language",
                table: "learning_profiles",
                columns: new[] { "user_id", "target_language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "learning_profile_selections");

            migrationBuilder.DropTable(
                name: "learning_profiles");
        }
    }
}
