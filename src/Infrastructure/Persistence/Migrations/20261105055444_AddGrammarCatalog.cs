using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGrammarCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grammar_points",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    language_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    native_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    pattern = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grammar_points", x => x.id);
                    table.CheckConstraint("ck_grammar_points_code", "code ~ '^[a-z][a-z0-9-]*$'");
                    table.CheckConstraint("ck_grammar_points_native_name", "native_name ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_grammar_points_pattern", "pattern ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_grammar_points_positive_ids", "id > 0 AND language_id > 0");
                    table.CheckConstraint("ck_grammar_points_sort_order", "sort_order >= 0");
                    table.CheckConstraint("ck_grammar_points_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_grammar_points_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_grammar_points_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grammar_point_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    grammar_point_id = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_grammar_point_translations", x => x.id);
                    table.CheckConstraint("ck_grammar_point_translations_explanation", "explanation ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_grammar_point_translations_locale", "locale ~ '^[a-z]{2,8}(-[a-z0-9]{1,8})*$'");
                    table.CheckConstraint("ck_grammar_point_translations_name", "name ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_grammar_point_translations_positive_ids", "id > 0 AND grammar_point_id > 0");
                    table.CheckConstraint("ck_grammar_point_translations_timestamps", "updated_at >= created_at");
                    table.CheckConstraint("ck_grammar_point_translations_usage_note", "usage_note IS NULL OR usage_note ~ '[^[:space:]]'");
                    table.CheckConstraint("ck_grammar_point_translations_version", "version > 0");
                    table.ForeignKey(
                        name: "FK_grammar_point_translations_grammar_points_grammar_point_id",
                        column: x => x.grammar_point_id,
                        principalTable: "grammar_points",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "grammar_points",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "pattern", "sort_order", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640068193708L, "noun-copula", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "名詞文の肯定", "N1 は N2 です", 0, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193709L, "noun-negation", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "名詞文の否定", "N1 は N2 ではありません", 1, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193710L, "question-ka", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "疑問の助詞か", "N は N ですか／Vますか", 2, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193711L, "noun-modifier-no", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "名詞をつなぐの", "N1 の N2", 3, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193712L, "additive-mo", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "追加を表すも", "N も …", 4, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193713L, "demonstratives", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "指示詞", "これ・それ・あれ／この・その・あの + N", 5, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193714L, "polite-verbs", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "動詞の丁寧形", "Vます", 6, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193715L, "polite-verb-negation", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "動詞の丁寧な否定", "Vません", 7, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193716L, "polite-verb-past", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "動詞の丁寧な過去", "Vました／Vませんでした", 8, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193717L, "object-o", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "目的語のを", "N を Vます", 9, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193718L, "time-ni", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "時を表すに", "時刻 に Vます", 10, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193719L, "action-location-de", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "動作の場所を表すで", "場所 で Vます", 11, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193720L, "existence-arimasu", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "存在を表すあります", "場所 に N が あります", 12, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640068193721L, "existence-imasu", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640068190208L, "存在を表すいます", "場所 に N が います", 13, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "grammar_point_translations",
                columns: new[] { "id", "created_at", "explanation", "grammar_point_id", "locale", "name", "updated_at", "usage_note", "version" },
                values: new object[,]
                {
                    { 1563703640068193758L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng は để nêu chủ đề và kết thúc bằng danh từ kèm です khi giới thiệu tên, nghề nghiệp hoặc phân loại. N1 và N2 là các danh từ.", 1563703640068193708L, "vi", "Câu khẳng định với danh từ", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trợ từ は được đọc là wa. Có thể lược chủ đề đã rõ trong hội thoại.", 1L },
                    { 1563703640068193759L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use は to introduce a topic and a noun followed by です to identify or classify it. N1 and N2 stand for nouns.", 1563703640068193708L, "en", "Affirmative noun sentences", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The particle は is pronounced wa. A topic already clear from context can be omitted.", 1L },
                    { 1563703640068193760L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng ではありません sau danh từ để phủ định việc đồng nhất hay phân loại chủ đề. Đây là dạng lịch sự.", 1563703640068193709L, "vi", "Phủ định câu danh từ", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong hội thoại cũng gặp じゃありません và じゃないです.", 1L },
                    { 1563703640068193761L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Follow a noun with ではありません to politely deny an identity or classification.", 1563703640068193709L, "en", "Negative noun sentences", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Conversational alternatives include じゃありません and じゃないです.", 1L },
                    { 1563703640068193762L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Thêm か vào cuối câu lịch sự để đặt câu hỏi. Giữ dạng động từ hoặc です phù hợp với câu.", 1563703640068193710L, "vi", "Câu hỏi với か", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Khi trả lời, có thể lược phần thông tin đã rõ và dùng はい hoặc いいえ cùng câu trả lời.", 1L },
                    { 1563703640068193763L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Add か to a polite sentence to form a question, retaining the appropriate verb or です form.", 1563703640068193710L, "en", "Questions with ka", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A response can omit understood information and combine はい or いいえ with the answer.", 1L },
                    { 1563703640068193764L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "N1 の bổ nghĩa cho N2, thể hiện quan hệ sở hữu, thuộc về, nguồn gốc hoặc loại tùy ngữ cảnh. Danh từ chính là N2.", 1563703640068193711L, "vi", "Nối danh từ bằng の", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ý nghĩa cụ thể của quan hệ phụ thuộc hai danh từ và tình huống sử dụng.", 1L },
                    { 1563703640068193765L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "N1 の modifies N2, expressing a relationship such as possession, affiliation, origin or type. N2 is the head noun.", 1563703640068193711L, "en", "Linking nouns with no", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interpret the relationship from the nouns and the context.", 1L },
                    { 1563703640068193766L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng も sau danh từ để thêm một đối tượng có cùng đặc điểm hay hành động đã đề cập.", 1563703640068193712L, "vi", "Bổ sung ý với も", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong mẫu nhập môn này, も thay vị trí của は hoặc が.", 1L },
                    { 1563703640068193767L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Attach も to a noun to include another item sharing a previously mentioned property or action.", 1563703640068193712L, "en", "Adding information with mo", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In this introductory pattern, も occupies the position of は or が.", 1L },
                    { 1563703640068193768L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "これ, それ, あれ lần lượt chỉ vật gần người nói, gần người nghe và xa cả hai. この, その, あの đứng trước danh từ.", 1563703640068193713L, "vi", "Chỉ định sự vật", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Chọn cách chỉ định theo vị trí và thông tin chung giữa những người đang nói chuyện.", 1L },
                    { 1563703640068193769L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "これ, それ and あれ identify things near the speaker, near the listener and distant from both. この, その and あの precede a noun.", 1563703640068193713L, "en", "Demonstratives", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Choose the form using physical position and the speakers’ shared context.", 1L },
                    { 1563703640068193770L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng ます dùng trong lời nói lịch sự để diễn đạt hành động theo thói quen, hiện tại hoặc tương lai tùy ngữ cảnh.", 1563703640068193714L, "vi", "Động từ lịch sự với ます", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học cách tạo dạng ます theo nhóm động từ và dùng từ chỉ thời gian để làm rõ mốc thời gian.", 1L },
                    { 1563703640068193771L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The ます form expresses actions politely, including habits and present or future situations according to context.", 1563703640068193714L, "en", "Polite verbs with masu", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn the form for each verb group and use time expressions to clarify when the action occurs.", 1L },
                    { 1563703640068193772L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng kết thúc bằng ません diễn đạt phủ định lịch sự ở thời không quá khứ.", 1563703640068193715L, "vi", "Phủ định động từ lịch sự", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đối chiếu các dạng khẳng định và phủ định của cùng một động từ.", 1L },
                    { 1563703640068193773L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The ending ません expresses a polite negative in the non-past.", 1563703640068193715L, "en", "Polite negative verbs", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Study the affirmative and negative forms of the same verb together.", 1L },
                    { 1563703640068193774L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng ました để diễn đạt khẳng định quá khứ và ませんでした để diễn đạt phủ định quá khứ trong lời nói lịch sự.", 1563703640068193716L, "vi", "Quá khứ động từ lịch sự", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Từ chỉ thời gian giúp người nghe xác định hành động được nói tới.", 1L },
                    { 1563703640068193775L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use ました for a polite affirmative past and ませんでした for a polite negative past.", 1563703640068193716L, "en", "Polite past verbs", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Time expressions help identify the action being discussed.", 1L },
                    { 1563703640068193776L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đặt を sau danh từ chỉ đối tượng trực tiếp của một động từ phù hợp. Động từ thường đứng cuối câu.", 1563703640068193717L, "vi", "Tân ngữ với を", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong cách dùng trợ từ này, を được đọc là o.", 1L },
                    { 1563703640068193777L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Place を after the noun serving as the direct object of an appropriate verb. The verb normally comes at the end.", 1563703640068193717L, "en", "Direct objects with o", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The particle を is pronounced o.", 1L },
                    { 1563703640068193778L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng に sau một thời điểm xác định để nói khi hành động xảy ra, chẳng hạn giờ trên đồng hồ.", 1563703640068193718L, "vi", "Thời điểm với に", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các từ thời gian tương đối như 今日 và 明日 thường đứng trực tiếp trong câu.", 1L },
                    { 1563703640068193779L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use に after a specific time point, such as a clock time, to indicate when an action takes place.", 1563703640068193718L, "en", "Time points with ni", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Relative time words such as 今日 and 明日 commonly stand directly in the sentence.", 1L },
                    { 1563703640068193780L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đặt で sau tên địa điểm để chỉ nơi một hành động diễn ra.", 1563703640068193719L, "vi", "Nơi diễn ra hành động với で", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các cách diễn đạt sự tồn tại với あります và います thường dùng に để đánh dấu vị trí.", 1L },
                    { 1563703640068193781L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Attach で to a place to identify where an action takes place.", 1563703640068193719L, "en", "Action locations with de", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Existence expressions with あります and います commonly mark location with に.", 1L },
                    { 1563703640068193782L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng mẫu này để nói một vật hoặc cây cối tồn tại ở địa điểm nào. が đánh dấu vật được giới thiệu.", 1563703640068193720L, "vi", "Sự tồn tại với あります", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một chủ đề đã biết cũng có thể được nêu bằng は rồi đến địa điểm và にあります.", 1L },
                    { 1563703640068193783L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use this pattern to introduce an object or plant at a location. が marks what is being introduced.", 1563703640068193720L, "en", "Existence with arimasu", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "An established topic can instead be marked with は, followed by the location and にあります.", 1L },
                    { 1563703640068193784L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng mẫu này để giới thiệu người hoặc động vật ở một địa điểm. が đánh dấu người hay con vật được giới thiệu.", 1563703640068193721L, "vi", "Sự tồn tại với います", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một chủ đề đã biết có thể dùng mẫu N は 場所 に います.", 1L },
                    { 1563703640068193785L, new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use this pattern to introduce a person or animal at a location. が marks the person or animal being introduced.", 1563703640068193721L, "en", "Existence with imasu", new DateTimeOffset(new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "For an established topic, use N は 場所 に います.", 1L }
                });

            migrationBuilder.CreateIndex(
                name: "ux_grammar_point_translations_point_id_locale",
                table: "grammar_point_translations",
                columns: new[] { "grammar_point_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_grammar_points_language_id_sort_order_id",
                table: "grammar_points",
                columns: new[] { "language_id", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ux_grammar_points_language_id_code",
                table: "grammar_points",
                columns: new[] { "language_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grammar_point_translations");

            migrationBuilder.DropTable(
                name: "grammar_points");
        }
    }
}
