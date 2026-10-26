using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPronunciationRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pronunciation_rules",
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
                    table.PrimaryKey("PK_pronunciation_rules", x => x.id);
                    table.CheckConstraint("ck_pronunciation_rules_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_pronunciation_rules_native_name", "length(btrim(native_name)) > 0");
                    table.CheckConstraint("ck_pronunciation_rules_positive_ids", "id > 0 AND language_id > 0");
                    table.CheckConstraint("ck_pronunciation_rules_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_pronunciation_rules_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_pronunciation_rules_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_pronunciation_rules_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pronunciation_examples",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    pronunciation_rule_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    transcription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    transcription_system = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    audio_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pronunciation_examples", x => x.id);
                    table.CheckConstraint("ck_pronunciation_examples_audio_url", "audio_url IS NULL OR audio_url ~ '^https://[^/?#[:space:]@]+([/?][^#[:space:]]*)?$'");
                    table.CheckConstraint("ck_pronunciation_examples_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_pronunciation_examples_positive_ids", "id > 0 AND pronunciation_rule_id > 0");
                    table.CheckConstraint("ck_pronunciation_examples_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_pronunciation_examples_text", "length(btrim(text)) > 0");
                    table.CheckConstraint("ck_pronunciation_examples_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_pronunciation_examples_transcription", "length(btrim(transcription)) > 0");
                    table.CheckConstraint("ck_pronunciation_examples_transcription_system", "transcription_system ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_pronunciation_examples_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_pronunciation_examples_pronunciation_rules_pronunciation_ru~",
                        column: x => x.pronunciation_rule_id,
                        principalTable: "pronunciation_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pronunciation_rule_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    pronunciation_rule_id = table.Column<long>(type: "bigint", nullable: false),
                    locale = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    explanation = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    usage_note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pronunciation_rule_translations", x => x.id);
                    table.CheckConstraint("ck_pronunciation_rule_translations_explanation", "length(btrim(explanation)) > 0");
                    table.CheckConstraint("ck_pronunciation_rule_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_pronunciation_rule_translations_name", "length(btrim(name)) > 0");
                    table.CheckConstraint("ck_pronunciation_rule_translations_positive_ids", "id > 0 AND pronunciation_rule_id > 0");
                    table.CheckConstraint("ck_pronunciation_rule_translations_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_pronunciation_rule_translations_usage_note", "usage_note IS NULL OR length(btrim(usage_note)) > 0");
                    table.CheckConstraint("ck_pronunciation_rule_translations_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_pronunciation_rule_translations_pronunciation_rules_pronunc~",
                        column: x => x.pronunciation_rule_id,
                        principalTable: "pronunciation_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "pronunciation_rules",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "sort_order", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068191208L, "dakuten-handakuten", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "濁音・半濁音", 0, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191209L, "contracted-sounds", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "拗音", 1, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191210L, "geminate-consonants", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "促音", 2, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191211L, "long-vowels", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "長音", 3, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191212L, "particle-readings", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "助詞の読み方", 4, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "pronunciation_examples",
                columns: new[] { "id", "audio_url", "code", "created_at", "pronunciation_rule_id", "sort_order", "text", "transcription", "transcription_system", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068191308L, null, "ga", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191208L, 0, "が", "ga", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191309L, null, "ba", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191208L, 1, "ば", "ba", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191310L, null, "pa", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191208L, 2, "ぱ", "pa", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191311L, null, "kyaku", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191209L, 0, "きゃく", "kyaku", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191312L, null, "shumi", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191209L, 1, "しゅみ", "shumi", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191313L, null, "choko", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191209L, 2, "チョコ", "choko", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191314L, null, "kitte", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191210L, 0, "きって", "kitte", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191315L, null, "gakkou", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191210L, 1, "がっこう", "gakkō", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191316L, null, "kappu", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191210L, 2, "カップ", "kappu", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191317L, null, "obaasan", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191211L, 0, "おばあさん", "obāsan", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191318L, null, "ojiisan", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191211L, 1, "おじいさん", "ojīsan", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191319L, null, "coffee", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191211L, 2, "コーヒー", "kōhī", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191320L, null, "topic-wa", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191212L, 0, "わたしは がくせいです。", "watashi wa gakusei desu.", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191321L, null, "direction-e", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191212L, 1, "がっこうへ いきます。", "gakkō e ikimasu.", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068191322L, null, "object-o", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640068191212L, 2, "みずを のみます。", "mizu o nomimasu.", "hepburn", new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "pronunciation_rule_translations",
                columns: new[] { "id", "created_at", "explanation", "locale", "name", "pronunciation_rule_id", "updated_at", "usage_note", "version" },
                values: new object[,]
                {
                    { 1563703640068191228L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dấu dakuten ゛ làm đổi cách đọc của một số kana, như か thành が và は thành ば. Dấu handakuten ゜ dùng với hàng は để tạo hàng âm p, như は thành ぱ. Các dấu này cũng xuất hiện trong Katakana.", "vi", "Âm đục và âm bán đục", 1563703640068191208L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Quan sát cả chữ và dấu đi kèm; không đọc chữ đã thêm dấu như chữ gốc.", 1L },
                    { 1563703640068191229L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dakuten ゛ changes certain kana readings, such as か to が and は to ば. Handakuten ゜ forms the p series from the は row, such as は to ぱ. The same marks are used in Katakana.", "en", "Voiced and p sounds", 1563703640068191208L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Read the complete marked character rather than treating the mark as decoration.", 1L },
                    { 1563703640068191230L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một số kana hàng i kết hợp với ゃ, ゅ hoặc ょ nhỏ thành một đơn vị nhịp, như きゃ, しゅ, ちょ. Phân biệt きゃ với きや có や lớn: cách chia nhịp khác nhau.", "vi", "Âm ghép với ゃ・ゅ・ょ nhỏ", 1563703640068191209L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Katakana dùng ャ, ュ, ョ nhỏ theo cùng nguyên tắc; chú ý kích thước của chữ thứ hai.", 1L },
                    { 1563703640068191231L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Certain i-row kana combine with small ゃ, ゅ or ょ into one timing unit, as in きゃ, しゅ and ちょ. Compare きゃ with きや: the full-sized や adds a separate timing unit.", "en", "Contracted sounds with small ya, yu and yo", 1563703640068191209L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Katakana uses small ャ, ュ and ョ in the same way. Check the size of the second character.", 1L },
                    { 1563703640068191232L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Chữ っ nhỏ biểu thị một nhịp kéo dài phụ âm kế tiếp. Tùy phụ âm, giữ chặn hoặc kéo dài luồng âm; không đọc thành tsu riêng. Trong きって, phần tt cần dài hơn một t đơn.", "vi", "Âm ngắt với っ nhỏ", 1563703640068191210L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Katakana dùng ッ nhỏ. Phân biệt với つ hoặc ツ lớn, được đọc là tsu.", 1L },
                    { 1563703640068191233L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Small っ adds a timing unit to the following consonant. Hold the closure or extend the consonant as appropriate; do not pronounce a separate tsu. In きって, tt lasts longer than a single t.", "en", "Long consonants with small tsu", 1563703640068191210L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Katakana uses small ッ. Full-sized つ and ツ are read as tsu instead.", 1L },
                    { 1563703640068191234L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Giữ nguyên âm dài thêm một nhịp. Độ dài có thể phân biệt từ: おばさん và おばあさん khác nhau. Cách viết thể hiện độ dài tùy từ; trong Katakana thường gặp dấu ー, như コーヒー.", "vi", "Nguyên âm dài", 1563703640068191211L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phiên âm ở đây dùng dấu dài Hepburn như ā, ī, ō. Không coi mọi chuỗi hai nguyên âm là một quy tắc duy nhất; đối chiếu từng ví dụ.", 1L },
                    { 1563703640068191235L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Sustain a long vowel for an extra timing unit. Length can distinguish words, as in おばさん and おばあさん. Spelling varies between words; Katakana often marks vowel length with ー, as in コーヒー.", "en", "Long vowels", 1563703640068191211L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "These examples use Hepburn macrons such as ā, ī and ō. Do not assume every pair of vowel letters follows one identical rule.", 1L },
                    { 1563703640068191236L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Khi dùng làm trợ từ, は được đọc là wa, へ là e và を thường được đọc là o. Giữ cách viết kana của trợ từ; phiên âm trong ví dụ thể hiện cách đọc theo ngữ cảnh.", "vi", "Cách đọc は・へ・を khi là trợ từ", 1563703640068191212L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Chỉ áp dụng cách đọc này đúng ngữ cảnh trợ từ; không đổi mọi は trong từ thành wa hoặc mọi へ thành e.", 1L },
                    { 1563703640068191237L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "As particles, は is read wa, へ is read e and を is normally read o. Keep their kana spelling; the example transcriptions show their contextual readings.", "en", "Reading the particles ha, he and wo", 1563703640068191212L, new DateTimeOffset(new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Apply these readings in particle contexts. Do not change every は within a word to wa or every へ to e.", 1L }
                });

            migrationBuilder.CreateIndex(
                name: "ix_pronunciation_examples_rule_id_sort_order_id",
                table: "pronunciation_examples",
                columns: new[] { "pronunciation_rule_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_pronunciation_examples_rule_id_code",
                table: "pronunciation_examples",
                columns: new[] { "pronunciation_rule_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_pronunciation_rule_translations_rule_id_locale",
                table: "pronunciation_rule_translations",
                columns: new[] { "pronunciation_rule_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pronunciation_rules_language_id_sort_order_id",
                table: "pronunciation_rules",
                columns: new[] { "language_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_pronunciation_rules_language_id_code",
                table: "pronunciation_rules",
                columns: new[] { "language_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pronunciation_examples");

            migrationBuilder.DropTable(
                name: "pronunciation_rule_translations");

            migrationBuilder.DropTable(
                name: "pronunciation_rules");
        }
    }
}
