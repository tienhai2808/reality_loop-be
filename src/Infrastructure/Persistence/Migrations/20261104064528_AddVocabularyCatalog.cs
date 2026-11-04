using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVocabularyCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vocabulary_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    language_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    term = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reading = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    audio_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_entries", x => x.id);
                    table.CheckConstraint("ck_vocabulary_entries_audio_url", "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
                    table.CheckConstraint("ck_vocabulary_entries_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_vocabulary_entries_positive_ids", "id > 0 AND language_id > 0");
                    table.CheckConstraint("ck_vocabulary_entries_reading", "reading IS NULL OR reading ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_vocabulary_entries_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_vocabulary_entries_term", "term ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_vocabulary_entries_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_vocabulary_entries_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_vocabulary_entries_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_senses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_entry_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    part_of_speech = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_senses", x => x.id);
                    table.CheckConstraint("ck_vocabulary_senses_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_vocabulary_senses_part_of_speech", "part_of_speech IN ('Noun', 'Verb', 'Adjective', 'Adverb', 'Pronoun', 'Determiner', 'Adposition', 'Conjunction', 'Particle', 'Interjection', 'Numeral', 'Auxiliary', 'Other')");
                    table.CheckConstraint("ck_vocabulary_senses_positive_ids", "id > 0 AND vocabulary_entry_id > 0");
                    table.CheckConstraint("ck_vocabulary_senses_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_vocabulary_senses_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_vocabulary_senses_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_vocabulary_senses_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_sense_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_sense_id = table.Column<long>(type: "bigint", nullable: false),
                    locale = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    meaning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    usage_note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_sense_translations", x => x.id);
                    table.CheckConstraint("ck_vocabulary_sense_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_vocabulary_sense_translations_meaning", "meaning ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_vocabulary_sense_translations_positive_ids", "id > 0 AND vocabulary_sense_id > 0");
                    table.CheckConstraint("ck_vocabulary_sense_translations_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_vocabulary_sense_translations_usage_note", "usage_note IS NULL OR usage_note ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_vocabulary_sense_translations_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_vocabulary_sense_translations_vocabulary_senses_vocabulary_~",
                        column: x => x.vocabulary_sense_id,
                        principalTable: "vocabulary_senses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "vocabulary_entries",
                columns: new[] { "id", "audio_url", "code", "created_at", "is_published", "language_id", "reading", "sort_order", "term", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068192208L, null, "mizu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "みず", 0, "水", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192209L, null, "ocha", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "おちゃ", 1, "お茶", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192210L, null, "gohan", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ごはん", 2, "ご飯", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192211L, null, "pan", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "パン", 3, "パン", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192212L, null, "gyuunyuu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ぎゅうにゅう", 4, "牛乳", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192213L, null, "sakana", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "さかな", 5, "魚", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192214L, null, "niku", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "にく", 6, "肉", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192215L, null, "yasai", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "やさい", 7, "野菜", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192216L, null, "kudamono", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "くだもの", 8, "果物", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192217L, null, "hon", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ほん", 9, "本", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192218L, null, "gakkou", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "がっこう", 10, "学校", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192219L, null, "gakusei", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "がくせい", 11, "学生", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192220L, null, "sensei", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "せんせい", 12, "先生", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192221L, null, "tomodachi", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ともだち", 13, "友達", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192222L, null, "kazoku", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "かぞく", 14, "家族", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192223L, null, "ie", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "いえ", 15, "家", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192224L, null, "heya", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "へや", 16, "部屋", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192225L, null, "eki", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "えき", 17, "駅", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192226L, null, "densha", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "でんしゃ", 18, "電車", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192227L, null, "mise", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "みせ", 19, "店", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192228L, null, "inu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "いぬ", 20, "犬", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192229L, null, "neko", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ねこ", 21, "猫", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192230L, null, "kyou", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "きょう", 22, "今日", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192231L, null, "ashita", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "あした", 23, "明日", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192232L, null, "kinou", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "きのう", 24, "昨日", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192233L, null, "asa", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "あさ", 25, "朝", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192234L, null, "yoru", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "よる", 26, "夜", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192235L, null, "taberu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "たべる", 27, "食べる", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192236L, null, "nomu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "のむ", 28, "飲む", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192237L, null, "iku", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "いく", 29, "行く", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192238L, null, "kuru", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "くる", 30, "来る", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192239L, null, "kiku", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "きく", 31, "聞く", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192240L, null, "yomu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "よむ", 32, "読む", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192241L, null, "kaku", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "かく", 33, "書く", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192242L, null, "hanasu", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "はなす", 34, "話す", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192243L, null, "ookii", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "おおきい", 35, "大きい", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192244L, null, "chiisai", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ちいさい", 36, "小さい", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192245L, null, "atarashii", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "あたらしい", 37, "新しい", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192246L, null, "furui", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ふるい", 38, "古い", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068192247L, null, "yukkuri", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "ゆっくり", 39, "ゆっくり", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "vocabulary_senses",
                columns: new[] { "id", "code", "created_at", "part_of_speech", "sort_order", "updated_at", "version", "vocabulary_entry_id" },
                values: new object[,]
                {
                    { 1563703640068192308L, "water", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192208L },
                    { 1563703640068192309L, "tea", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192209L },
                    { 1563703640068192310L, "cooked-rice", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192210L },
                    { 1563703640068192311L, "bread", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192211L },
                    { 1563703640068192312L, "milk", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192212L },
                    { 1563703640068192313L, "fish", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192213L },
                    { 1563703640068192314L, "meat", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192214L },
                    { 1563703640068192315L, "vegetables", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192215L },
                    { 1563703640068192316L, "fruit", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192216L },
                    { 1563703640068192317L, "book", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192217L },
                    { 1563703640068192318L, "school", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192218L },
                    { 1563703640068192319L, "student", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192219L },
                    { 1563703640068192320L, "teacher", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192220L },
                    { 1563703640068192321L, "friend", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192221L },
                    { 1563703640068192322L, "family", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192222L },
                    { 1563703640068192323L, "home", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192223L },
                    { 1563703640068192324L, "room", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192224L },
                    { 1563703640068192325L, "station", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192225L },
                    { 1563703640068192326L, "train", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192226L },
                    { 1563703640068192327L, "shop", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192227L },
                    { 1563703640068192328L, "dog", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192228L },
                    { 1563703640068192329L, "cat", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192229L },
                    { 1563703640068192330L, "today", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192230L },
                    { 1563703640068192331L, "tomorrow", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192231L },
                    { 1563703640068192332L, "yesterday", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192232L },
                    { 1563703640068192333L, "morning", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192233L },
                    { 1563703640068192334L, "night", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192234L },
                    { 1563703640068192335L, "eat", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192235L },
                    { 1563703640068192336L, "drink", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192236L },
                    { 1563703640068192337L, "go", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192237L },
                    { 1563703640068192338L, "come", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192238L },
                    { 1563703640068192339L, "listen", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192239L },
                    { 1563703640068192340L, "read", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192240L },
                    { 1563703640068192341L, "write", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192241L },
                    { 1563703640068192342L, "speak", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192242L },
                    { 1563703640068192343L, "big", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192243L },
                    { 1563703640068192344L, "small", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192244L },
                    { 1563703640068192345L, "new", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192245L },
                    { 1563703640068192346L, "old", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192246L },
                    { 1563703640068192347L, "slowly", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192247L },
                    { 1563703640068192348L, "meal", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 1, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192210L },
                    { 1563703640068192349L, "ask", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 1, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640068192239L }
                });

            migrationBuilder.InsertData(
                table: "vocabulary_sense_translations",
                columns: new[] { "id", "created_at", "locale", "meaning", "updated_at", "usage_note", "version", "vocabulary_sense_id" },
                values: new object[,]
                {
                    { 1563703640068192408L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nước.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192308L },
                    { 1563703640068192409L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Water.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192308L },
                    { 1563703640068192410L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trà; nước trà.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192309L },
                    { 1563703640068192411L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tea.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192309L },
                    { 1563703640068192412L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cơm; gạo đã nấu chín.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt nghĩa chỉ cơm với nghĩa chỉ cả bữa ăn của cùng từ.", 1L, 1563703640068192310L },
                    { 1563703640068192413L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Cooked rice.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish rice itself from the separate meal sense.", 1L, 1563703640068192310L },
                    { 1563703640068192414L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bánh mì.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192311L },
                    { 1563703640068192415L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Bread.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192311L },
                    { 1563703640068192416L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sữa bò.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192312L },
                    { 1563703640068192417L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Cow's milk.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192312L },
                    { 1563703640068192418L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cá.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192313L },
                    { 1563703640068192419L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Fish.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192313L },
                    { 1563703640068192420L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thịt.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192314L },
                    { 1563703640068192421L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Meat.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192314L },
                    { 1563703640068192422L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Rau; rau củ.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192315L },
                    { 1563703640068192423L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Vegetables.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192315L },
                    { 1563703640068192424L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trái cây; hoa quả.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192316L },
                    { 1563703640068192425L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Fruit.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192316L },
                    { 1563703640068192426L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sách.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192317L },
                    { 1563703640068192427L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A book.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192317L },
                    { 1563703640068192428L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trường học.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192318L },
                    { 1563703640068192429L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A school.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192318L },
                    { 1563703640068192430L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Học sinh; sinh viên.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192319L },
                    { 1563703640068192431L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A student.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192319L },
                    { 1563703640068192432L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Giáo viên; thầy, cô.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ở đây giới thiệu nghĩa giáo viên; từ này còn được dùng để xưng gọi một số người có chuyên môn.", 1L, 1563703640068192320L },
                    { 1563703640068192433L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A teacher.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "This entry teaches the teacher sense; the same word also addresses some professionals.", 1L, 1563703640068192320L },
                    { 1563703640068192434L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bạn bè; người bạn.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192321L },
                    { 1563703640068192435L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A friend.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192321L },
                    { 1563703640068192436L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Gia đình.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192322L },
                    { 1563703640068192437L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Family.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192322L },
                    { 1563703640068192438L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhà; nơi ở.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192323L },
                    { 1563703640068192439L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A house or home.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192323L },
                    { 1563703640068192440L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phòng; căn phòng.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192324L },
                    { 1563703640068192441L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A room.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192324L },
                    { 1563703640068192442L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhà ga.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192325L },
                    { 1563703640068192443L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A railway station.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192325L },
                    { 1563703640068192444L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tàu điện.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192326L },
                    { 1563703640068192445L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An electric train.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192326L },
                    { 1563703640068192446L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cửa hàng; tiệm.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192327L },
                    { 1563703640068192447L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A shop.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192327L },
                    { 1563703640068192448L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Con chó.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192328L },
                    { 1563703640068192449L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A dog.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192328L },
                    { 1563703640068192450L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Con mèo.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192329L },
                    { 1563703640068192451L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A cat.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192329L },
                    { 1563703640068192452L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hôm nay.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192330L },
                    { 1563703640068192453L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Today.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192330L },
                    { 1563703640068192454L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngày mai.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192331L },
                    { 1563703640068192455L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tomorrow.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192331L },
                    { 1563703640068192456L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hôm qua.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192332L },
                    { 1563703640068192457L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Yesterday.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192332L },
                    { 1563703640068192458L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Buổi sáng.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192333L },
                    { 1563703640068192459L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Morning.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192333L },
                    { 1563703640068192460L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Buổi tối; ban đêm.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192334L },
                    { 1563703640068192461L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Night or evening.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192334L },
                    { 1563703640068192462L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ăn.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192335L },
                    { 1563703640068192463L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To eat.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192335L },
                    { 1563703640068192464L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Uống.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192336L },
                    { 1563703640068192465L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To drink.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192336L },
                    { 1563703640068192466L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đi.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192337L },
                    { 1563703640068192467L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To go.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192337L },
                    { 1563703640068192468L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đến; tới.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192338L },
                    { 1563703640068192469L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To come.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192338L },
                    { 1563703640068192470L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nghe; lắng nghe.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Cùng cách viết và cách đọc còn có nghĩa hỏi; chọn nghĩa theo ngữ cảnh.", 1L, 1563703640068192339L },
                    { 1563703640068192471L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To hear or listen.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The same spelling and reading also mean to ask; context determines the sense.", 1L, 1563703640068192339L },
                    { 1563703640068192472L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đọc.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192340L },
                    { 1563703640068192473L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To read.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192340L },
                    { 1563703640068192474L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Viết.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192341L },
                    { 1563703640068192475L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To write.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192341L },
                    { 1563703640068192476L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nói; trò chuyện.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192342L },
                    { 1563703640068192477L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To speak or talk.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192342L },
                    { 1563703640068192478L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "To; lớn.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192343L },
                    { 1563703640068192479L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Big or large.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192343L },
                    { 1563703640068192480L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhỏ; bé.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192344L },
                    { 1563703640068192481L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Small.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192344L },
                    { 1563703640068192482L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mới.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192345L },
                    { 1563703640068192483L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "New.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192345L },
                    { 1563703640068192484L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cũ; có từ lâu.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Không dùng tính từ này để nói một người cao tuổi.", 1L, 1563703640068192346L },
                    { 1563703640068192485L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Old; not new.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Do not use this adjective to describe an elderly person.", 1L, 1563703640068192346L },
                    { 1563703640068192486L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chậm rãi; từ từ.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192347L },
                    { 1563703640068192487L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Slowly; at an unhurried pace.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640068192347L },
                    { 1563703640068192488L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bữa ăn.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nghĩa này nói đến cả bữa ăn, không chỉ riêng cơm.", 1L, 1563703640068192348L },
                    { 1563703640068192489L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A meal.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "This sense refers to the whole meal rather than rice alone.", 1L, 1563703640068192348L },
                    { 1563703640068192490L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hỏi; hỏi để biết thông tin.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt với nghĩa nghe của cùng từ.", 1L, 1563703640068192349L },
                    { 1563703640068192491L, new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To ask for information.", new DateTimeOffset(new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish this sense from hearing or listening.", 1L, 1563703640068192349L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_language_id_sort_order_id",
                table: "vocabulary_entries",
                columns: new[] { "language_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_entries_language_id_code",
                table: "vocabulary_entries",
                columns: new[] { "language_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_sense_translations_sense_id_locale",
                table: "vocabulary_sense_translations",
                columns: new[] { "vocabulary_sense_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_senses_entry_id_sort_order_id",
                table: "vocabulary_senses",
                columns: new[] { "vocabulary_entry_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_vocabulary_senses_entry_id_code",
                table: "vocabulary_senses",
                columns: new[] { "vocabulary_entry_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vocabulary_sense_translations");

            migrationBuilder.DropTable(
                name: "vocabulary_senses");

            migrationBuilder.DropTable(
                name: "vocabulary_entries");
        }
    }
}
