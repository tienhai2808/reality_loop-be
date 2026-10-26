using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguagesAndWritingSystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    native_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.id);
                    table.CheckConstraint("ck_languages_code", "code ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_languages_native_name", "length(btrim(native_name)) > 0");
                    table.CheckConstraint("ck_languages_positive_ids", "id > 0");
                    table.CheckConstraint("ck_languages_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_languages_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_languages_version", "version > 0");
                });

            migrationBuilder.CreateTable(
                name: "language_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    language_id = table.Column<long>(type: "bigint", nullable: false),
                    locale = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_translations", x => x.id);
                    table.CheckConstraint("ck_language_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_language_translations_name", "length(btrim(name)) > 0");
                    table.CheckConstraint("ck_language_translations_positive_ids", "id > 0 AND language_id > 0");
                    table.CheckConstraint("ck_language_translations_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_language_translations_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_language_translations_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "writing_systems",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    language_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    native_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_systems", x => x.id);
                    table.CheckConstraint("ck_writing_systems_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_writing_systems_native_name", "length(btrim(native_name)) > 0");
                    table.CheckConstraint("ck_writing_systems_positive_ids", "id > 0 AND language_id > 0");
                    table.CheckConstraint("ck_writing_systems_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_writing_systems_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_writing_systems_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_systems_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "writing_system_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    symbol = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    row_index = table.Column<int>(type: "integer", nullable: true),
                    column_index = table.Column<int>(type: "integer", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_items", x => x.id);
                    table.CheckConstraint("ck_writing_system_items_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_writing_system_items_grid", "(row_index IS NULL AND column_index IS NULL) OR (row_index IS NOT NULL AND column_index IS NOT NULL AND row_index >= 0 AND column_index >= 0)");
                    table.CheckConstraint("ck_writing_system_items_positive_ids", "id > 0 AND writing_system_id > 0");
                    table.CheckConstraint("ck_writing_system_items_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_writing_system_items_symbol", "length(btrim(symbol)) > 0");
                    table.CheckConstraint("ck_writing_system_items_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_writing_system_items_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_items_writing_systems_writing_system_id",
                        column: x => x.writing_system_id,
                        principalTable: "writing_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "writing_system_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_id = table.Column<long>(type: "bigint", nullable: false),
                    locale = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_translations", x => x.id);
                    table.CheckConstraint("ck_writing_system_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_writing_system_translations_name", "length(btrim(name)) > 0");
                    table.CheckConstraint("ck_writing_system_translations_positive_ids", "id > 0 AND writing_system_id > 0");
                    table.CheckConstraint("ck_writing_system_translations_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_writing_system_translations_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_translations_writing_systems_writing_system_~",
                        column: x => x.writing_system_id,
                        principalTable: "writing_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "writing_system_item_readings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    writing_system_item_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    transcription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    transcription_system = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    audio_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_system_item_readings", x => x.id);
                    table.CheckConstraint("ck_writing_system_item_readings_audio_url", "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
                    table.CheckConstraint("ck_writing_system_item_readings_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_writing_system_item_readings_positive_ids", "id > 0 AND writing_system_item_id > 0");
                    table.CheckConstraint("ck_writing_system_item_readings_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_writing_system_item_readings_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_writing_system_item_readings_transcription", "length(btrim(transcription)) > 0");
                    table.CheckConstraint("ck_writing_system_item_readings_transcription_system", "transcription_system ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_writing_system_item_readings_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_writing_system_item_readings_writing_system_items_writing_s~",
                        column: x => x.writing_system_item_id,
                        principalTable: "writing_system_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "languages",
                columns: new[] { "id", "code", "created_at", "is_enabled", "native_name", "sort_order", "updated_at", "version" },
                values: new object[] { 1563703640068190208L, "ja", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "日本語", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L });

            migrationBuilder.InsertData(
                table: "language_translations",
                columns: new[] { "id", "created_at", "description", "language_id", "locale", "name", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068190209L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bắt đầu với bảng Hiragana và Katakana cơ bản.", 1563703640068190208L, "vi", "Tiếng Nhật", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068190210L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Start with the basic Hiragana and Katakana charts.", 1563703640068190208L, "en", "Japanese", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "writing_systems",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "sort_order", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068190218L, "hiragana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ひらがな", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068190219L, "katakana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "カタカナ", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_items",
                columns: new[] { "id", "code", "column_index", "created_at", "row_index", "sort_order", "symbol", "updated_at", "version", "writing_system_id" },
                values: new object[,]
                {
                    { 1563703640068190308L, "kana-r0-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 0, "あ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190309L, "kana-r0-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 1, "い", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190310L, "kana-r0-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 2, "う", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190311L, "kana-r0-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 3, "え", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190312L, "kana-r0-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 4, "お", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190313L, "kana-r1-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 5, "か", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190314L, "kana-r1-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 6, "き", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190315L, "kana-r1-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 7, "く", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190316L, "kana-r1-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 8, "け", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190317L, "kana-r1-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 9, "こ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190318L, "kana-r2-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 10, "さ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190319L, "kana-r2-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 11, "し", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190320L, "kana-r2-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 12, "す", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190321L, "kana-r2-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 13, "せ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190322L, "kana-r2-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 14, "そ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190323L, "kana-r3-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 15, "た", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190324L, "kana-r3-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 16, "ち", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190325L, "kana-r3-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 17, "つ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190326L, "kana-r3-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 18, "て", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190327L, "kana-r3-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 19, "と", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190328L, "kana-r4-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 20, "な", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190329L, "kana-r4-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 21, "に", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190330L, "kana-r4-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 22, "ぬ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190331L, "kana-r4-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 23, "ね", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190332L, "kana-r4-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 24, "の", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190333L, "kana-r5-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 25, "は", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190334L, "kana-r5-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 26, "ひ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190335L, "kana-r5-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 27, "ふ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190336L, "kana-r5-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 28, "へ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190337L, "kana-r5-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 29, "ほ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190338L, "kana-r6-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 30, "ま", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190339L, "kana-r6-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 31, "み", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190340L, "kana-r6-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 32, "む", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190341L, "kana-r6-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 33, "め", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190342L, "kana-r6-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 34, "も", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190343L, "kana-r7-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 35, "や", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190344L, "kana-r7-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 36, "ゆ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190345L, "kana-r7-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 37, "よ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190346L, "kana-r8-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 38, "ら", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190347L, "kana-r8-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 39, "り", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190348L, "kana-r8-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 40, "る", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190349L, "kana-r8-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 41, "れ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190350L, "kana-r8-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 42, "ろ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190351L, "kana-r9-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 43, "わ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190352L, "kana-r9-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 44, "を", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190353L, "kana-r10-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, 45, "ん", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190408L, "kana-r0-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 0, "ア", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190409L, "kana-r0-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 1, "イ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190410L, "kana-r0-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 2, "ウ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190411L, "kana-r0-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 3, "エ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190412L, "kana-r0-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 4, "オ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190413L, "kana-r1-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 5, "カ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190414L, "kana-r1-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 6, "キ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190415L, "kana-r1-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 7, "ク", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190416L, "kana-r1-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 8, "ケ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190417L, "kana-r1-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 9, "コ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190418L, "kana-r2-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 10, "サ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190419L, "kana-r2-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 11, "シ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190420L, "kana-r2-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 12, "ス", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190421L, "kana-r2-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 13, "セ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190422L, "kana-r2-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 14, "ソ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190423L, "kana-r3-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 15, "タ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190424L, "kana-r3-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 16, "チ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190425L, "kana-r3-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 17, "ツ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190426L, "kana-r3-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 18, "テ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190427L, "kana-r3-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 19, "ト", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190428L, "kana-r4-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 20, "ナ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190429L, "kana-r4-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 21, "ニ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190430L, "kana-r4-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 22, "ヌ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190431L, "kana-r4-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 23, "ネ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190432L, "kana-r4-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 24, "ノ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190433L, "kana-r5-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 25, "ハ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190434L, "kana-r5-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 26, "ヒ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190435L, "kana-r5-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 27, "フ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190436L, "kana-r5-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 28, "ヘ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190437L, "kana-r5-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 29, "ホ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190438L, "kana-r6-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 30, "マ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190439L, "kana-r6-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 31, "ミ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190440L, "kana-r6-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 32, "ム", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190441L, "kana-r6-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 33, "メ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190442L, "kana-r6-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 34, "モ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190443L, "kana-r7-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 35, "ヤ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190444L, "kana-r7-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 36, "ユ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190445L, "kana-r7-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 37, "ヨ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190446L, "kana-r8-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 38, "ラ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190447L, "kana-r8-c1", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 39, "リ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190448L, "kana-r8-c2", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 40, "ル", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190449L, "kana-r8-c3", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 41, "レ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190450L, "kana-r8-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 42, "ロ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190451L, "kana-r9-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 43, "ワ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190452L, "kana-r9-c4", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 44, "ヲ", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190453L, "kana-r10-c0", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, 45, "ン", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_translations",
                columns: new[] { "id", "created_at", "description", "locale", "name", "updated_at", "version", "writing_system_id" },
                values: new object[,]
                {
                    { 1563703640068190228L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bảng 46 chữ Hiragana cơ bản. Phiên âm chỉ hỗ trợ ghi nhớ; chưa bao gồm âm đục, âm ghép và quy tắc đọc trong câu.", "vi", "Hiragana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190229L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The 46 basic Hiragana. Romanization is a learning aid; voiced sounds, combinations and contextual reading rules are not included yet.", "en", "Hiragana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190218L },
                    { 1563703640068190230L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bảng 46 chữ Katakana cơ bản. Chưa bao gồm âm đục, âm ghép và các tổ hợp mở rộng.", "vi", "Katakana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L },
                    { 1563703640068190231L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The 46 basic Katakana. Voiced sounds, combinations and extended forms are not included yet.", "en", "Katakana", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190219L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_item_readings",
                columns: new[] { "id", "audio_url", "code", "created_at", "sort_order", "transcription", "transcription_system", "updated_at", "version", "writing_system_item_id" },
                values: new object[,]
                {
                    { 1563703640068190508L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "a", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190308L },
                    { 1563703640068190509L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "i", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190309L },
                    { 1563703640068190510L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "u", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190310L },
                    { 1563703640068190511L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "e", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190311L },
                    { 1563703640068190512L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "o", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190312L },
                    { 1563703640068190513L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ka", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190313L },
                    { 1563703640068190514L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ki", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190314L },
                    { 1563703640068190515L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ku", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190315L },
                    { 1563703640068190516L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ke", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190316L },
                    { 1563703640068190517L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ko", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190317L },
                    { 1563703640068190518L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "sa", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190318L },
                    { 1563703640068190519L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "shi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190319L },
                    { 1563703640068190520L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "su", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190320L },
                    { 1563703640068190521L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "se", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190321L },
                    { 1563703640068190522L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "so", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190322L },
                    { 1563703640068190523L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ta", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190323L },
                    { 1563703640068190524L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "chi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190324L },
                    { 1563703640068190525L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tsu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190325L },
                    { 1563703640068190526L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "te", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190326L },
                    { 1563703640068190527L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "to", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190327L },
                    { 1563703640068190528L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "na", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190328L },
                    { 1563703640068190529L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ni", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190329L },
                    { 1563703640068190530L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "nu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190330L },
                    { 1563703640068190531L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ne", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190331L },
                    { 1563703640068190532L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "no", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190332L },
                    { 1563703640068190533L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ha", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190333L },
                    { 1563703640068190534L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "hi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190334L },
                    { 1563703640068190535L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "fu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190335L },
                    { 1563703640068190536L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "he", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190336L },
                    { 1563703640068190537L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ho", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190337L },
                    { 1563703640068190538L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ma", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190338L },
                    { 1563703640068190539L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190339L },
                    { 1563703640068190540L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190340L },
                    { 1563703640068190541L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "me", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190341L },
                    { 1563703640068190542L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190342L },
                    { 1563703640068190543L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ya", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190343L },
                    { 1563703640068190544L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190344L },
                    { 1563703640068190545L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190345L },
                    { 1563703640068190546L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ra", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190346L },
                    { 1563703640068190547L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ri", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190347L },
                    { 1563703640068190548L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ru", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190348L },
                    { 1563703640068190549L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "re", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190349L },
                    { 1563703640068190550L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ro", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190350L },
                    { 1563703640068190551L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "wa", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190351L },
                    { 1563703640068190552L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "wo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190352L },
                    { 1563703640068190553L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "n", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190353L },
                    { 1563703640068190608L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "a", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190408L },
                    { 1563703640068190609L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "i", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190409L },
                    { 1563703640068190610L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "u", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190410L },
                    { 1563703640068190611L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "e", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190411L },
                    { 1563703640068190612L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "o", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190412L },
                    { 1563703640068190613L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ka", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190413L },
                    { 1563703640068190614L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ki", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190414L },
                    { 1563703640068190615L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ku", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190415L },
                    { 1563703640068190616L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ke", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190416L },
                    { 1563703640068190617L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ko", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190417L },
                    { 1563703640068190618L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "sa", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190418L },
                    { 1563703640068190619L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "shi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190419L },
                    { 1563703640068190620L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "su", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190420L },
                    { 1563703640068190621L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "se", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190421L },
                    { 1563703640068190622L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "so", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190422L },
                    { 1563703640068190623L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ta", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190423L },
                    { 1563703640068190624L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "chi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190424L },
                    { 1563703640068190625L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tsu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190425L },
                    { 1563703640068190626L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "te", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190426L },
                    { 1563703640068190627L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "to", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190427L },
                    { 1563703640068190628L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "na", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190428L },
                    { 1563703640068190629L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ni", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190429L },
                    { 1563703640068190630L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "nu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190430L },
                    { 1563703640068190631L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ne", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190431L },
                    { 1563703640068190632L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "no", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190432L },
                    { 1563703640068190633L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ha", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190433L },
                    { 1563703640068190634L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "hi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190434L },
                    { 1563703640068190635L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "fu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190435L },
                    { 1563703640068190636L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "he", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190436L },
                    { 1563703640068190637L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ho", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190437L },
                    { 1563703640068190638L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ma", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190438L },
                    { 1563703640068190639L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190439L },
                    { 1563703640068190640L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190440L },
                    { 1563703640068190641L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "me", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190441L },
                    { 1563703640068190642L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "mo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190442L },
                    { 1563703640068190643L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ya", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190443L },
                    { 1563703640068190644L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190444L },
                    { 1563703640068190645L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190445L },
                    { 1563703640068190646L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ra", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190446L },
                    { 1563703640068190647L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ri", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190447L },
                    { 1563703640068190648L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ru", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190448L },
                    { 1563703640068190649L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "re", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190449L },
                    { 1563703640068190650L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ro", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190450L },
                    { 1563703640068190651L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "wa", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190451L },
                    { 1563703640068190652L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "wo", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190452L },
                    { 1563703640068190653L, null, "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "n", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068190453L }
                });

            migrationBuilder.CreateIndex(
                name: "ux_language_translations_language_id_locale",
                table: "language_translations",
                columns: new[] { "language_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_languages_is_enabled_sort_order_id",
                table: "languages",
                columns: new[] { "is_enabled", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_languages_code",
                table: "languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_writing_item_readings_item_id_sort_order_id",
                table: "writing_system_item_readings",
                columns: new[] { "writing_system_item_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_writing_system_item_readings_writing_system_item_id_code",
                table: "writing_system_item_readings",
                columns: new[] { "writing_system_item_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_writing_system_items_writing_system_id_sort_order_id",
                table: "writing_system_items",
                columns: new[] { "writing_system_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_writing_system_items_writing_system_id_code",
                table: "writing_system_items",
                columns: new[] { "writing_system_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_writing_system_translations_writing_system_id_locale",
                table: "writing_system_translations",
                columns: new[] { "writing_system_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_writing_systems_language_id_sort_order_id",
                table: "writing_systems",
                columns: new[] { "language_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_writing_systems_language_id_code",
                table: "writing_systems",
                columns: new[] { "language_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "language_translations");

            migrationBuilder.DropTable(
                name: "writing_system_item_readings");

            migrationBuilder.DropTable(
                name: "writing_system_translations");

            migrationBuilder.DropTable(
                name: "writing_system_items");

            migrationBuilder.DropTable(
                name: "writing_systems");

            migrationBuilder.DropTable(
                name: "languages");
        }
    }
}
