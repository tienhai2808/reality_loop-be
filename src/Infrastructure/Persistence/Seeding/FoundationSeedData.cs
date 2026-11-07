using Domain.Languages;
using Domain.WritingSystems;

namespace Infrastructure.Persistence.Seeding;

internal static class FoundationSeedData
{
    // Preallocated Snowflake IDs: IdGen default epoch/bit layout, 2026-10-25T00:00:00Z, generator 1023.
    // Keep these IDs and the timestamp fixed; codes are the natural keys for future imports.
    internal const long JapaneseId = 1_563_703_640_068_190_208L;
    private const long HiraganaId = JapaneseId + 10;
    private const long KatakanaId = JapaneseId + 11;
    private static readonly DateTimeOffset PublishedAt = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);

    public static readonly Language[] LanguageRows =
    [
        Language.Create(JapaneseId, "ja", "日本語", true, 0, PublishedAt)
    ];

    public static readonly LanguageTranslation[] LanguageTranslationRows = BuildLanguageTranslations();

    private static LanguageTranslation[] BuildLanguageTranslations()
    {
        LanguageTranslation[] rows =
        [
            LanguageTranslation.Create(JapaneseId + 1, JapaneseId, "vi", "Tiếng Nhật", "Học Hiragana, Katakana, phát âm, từ vựng và ngữ pháp nhập môn.", PublishedAt),
            LanguageTranslation.Create(JapaneseId + 2, JapaneseId, "en", "Japanese", "Study Hiragana, Katakana, pronunciation, introductory vocabulary and grammar.", PublishedAt)
        ];

        foreach (var row in rows)
            row.Update(row.Name, row.Description, new DateTimeOffset(2026, 11, 7, 0, 0, 0, TimeSpan.Zero));
        return rows;
    }

    public static readonly WritingSystem[] WritingSystemRows =
    [
        WritingSystem.Create(HiraganaId, JapaneseId, "hiragana", "ひらがな", true, 0, PublishedAt),
        WritingSystem.Create(KatakanaId, JapaneseId, "katakana", "カタカナ", true, 1, PublishedAt)
    ];

    public static readonly WritingSystemTranslation[] WritingSystemTranslationRows = BuildSystemTranslations();

    private static WritingSystemTranslation[] BuildSystemTranslations()
    {
        WritingSystemTranslation[] rows =
        [
            WritingSystemTranslation.Create(JapaneseId + 20, HiraganaId, "vi", "Hiragana", "104 mục Hiragana gồm 46 chữ cơ bản, 25 âm đục/bán đục và 33 âm ghép thông dụng. Đối chiếu quy tắc phát âm khi đọc trong từ và câu.", PublishedAt),
            WritingSystemTranslation.Create(JapaneseId + 21, HiraganaId, "en", "Hiragana", "104 Hiragana items covering 46 basic kana, 25 voiced/p sounds and 33 common contracted sounds. Consult pronunciation rules for readings in words and sentences.", PublishedAt),
            WritingSystemTranslation.Create(JapaneseId + 22, KatakanaId, "vi", "Katakana", "104 mục Katakana gồm 46 chữ cơ bản, 25 âm đục/bán đục và 33 âm ghép thông dụng. Các tổ hợp đặc biệt cho từ mượn được giải thích trong phần phát âm.", PublishedAt),
            WritingSystemTranslation.Create(JapaneseId + 23, KatakanaId, "en", "Katakana", "104 Katakana items covering 46 basic kana, 25 voiced/p sounds and 33 common contracted sounds. Special loanword combinations are explained in the pronunciation section.", PublishedAt)
        ];

        foreach (var row in rows)
            row.Update(row.Name, row.Description, new DateTimeOffset(2026, 11, 7, 0, 0, 0, TimeSpan.Zero));
        return rows;
    }

    // Keys/codes are explicit: inserting or reordering this array must not change existing identities.
    private static readonly (int Key, string Code, string Hiragana, string Katakana, string Reading, int Row, int Column)[] Kana =
    [
        (0, "kana-r0-c0", "あ", "ア", "a", 0, 0), (1, "kana-r0-c1", "い", "イ", "i", 0, 1), (2, "kana-r0-c2", "う", "ウ", "u", 0, 2), (3, "kana-r0-c3", "え", "エ", "e", 0, 3), (4, "kana-r0-c4", "お", "オ", "o", 0, 4),
        (5, "kana-r1-c0", "か", "カ", "ka", 1, 0), (6, "kana-r1-c1", "き", "キ", "ki", 1, 1), (7, "kana-r1-c2", "く", "ク", "ku", 1, 2), (8, "kana-r1-c3", "け", "ケ", "ke", 1, 3), (9, "kana-r1-c4", "こ", "コ", "ko", 1, 4),
        (10, "kana-r2-c0", "さ", "サ", "sa", 2, 0), (11, "kana-r2-c1", "し", "シ", "shi", 2, 1), (12, "kana-r2-c2", "す", "ス", "su", 2, 2), (13, "kana-r2-c3", "せ", "セ", "se", 2, 3), (14, "kana-r2-c4", "そ", "ソ", "so", 2, 4),
        (15, "kana-r3-c0", "た", "タ", "ta", 3, 0), (16, "kana-r3-c1", "ち", "チ", "chi", 3, 1), (17, "kana-r3-c2", "つ", "ツ", "tsu", 3, 2), (18, "kana-r3-c3", "て", "テ", "te", 3, 3), (19, "kana-r3-c4", "と", "ト", "to", 3, 4),
        (20, "kana-r4-c0", "な", "ナ", "na", 4, 0), (21, "kana-r4-c1", "に", "ニ", "ni", 4, 1), (22, "kana-r4-c2", "ぬ", "ヌ", "nu", 4, 2), (23, "kana-r4-c3", "ね", "ネ", "ne", 4, 3), (24, "kana-r4-c4", "の", "ノ", "no", 4, 4),
        (25, "kana-r5-c0", "は", "ハ", "ha", 5, 0), (26, "kana-r5-c1", "ひ", "ヒ", "hi", 5, 1), (27, "kana-r5-c2", "ふ", "フ", "fu", 5, 2), (28, "kana-r5-c3", "へ", "ヘ", "he", 5, 3), (29, "kana-r5-c4", "ほ", "ホ", "ho", 5, 4),
        (30, "kana-r6-c0", "ま", "マ", "ma", 6, 0), (31, "kana-r6-c1", "み", "ミ", "mi", 6, 1), (32, "kana-r6-c2", "む", "ム", "mu", 6, 2), (33, "kana-r6-c3", "め", "メ", "me", 6, 3), (34, "kana-r6-c4", "も", "モ", "mo", 6, 4),
        (35, "kana-r7-c0", "や", "ヤ", "ya", 7, 0), (36, "kana-r7-c2", "ゆ", "ユ", "yu", 7, 2), (37, "kana-r7-c4", "よ", "ヨ", "yo", 7, 4),
        (38, "kana-r8-c0", "ら", "ラ", "ra", 8, 0), (39, "kana-r8-c1", "り", "リ", "ri", 8, 1), (40, "kana-r8-c2", "る", "ル", "ru", 8, 2), (41, "kana-r8-c3", "れ", "レ", "re", 8, 3), (42, "kana-r8-c4", "ろ", "ロ", "ro", 8, 4),
        (43, "kana-r9-c0", "わ", "ワ", "wa", 9, 0), (44, "kana-r9-c4", "を", "ヲ", "wo", 9, 4), (45, "kana-r10-c0", "ん", "ン", "n", 10, 0)
    ];

    // A separate millisecond reserves sequences for this content release. Original IDs remain stable.
    internal const long ExpansionId = JapaneseId + (1L << 22);
    internal static readonly DateTimeOffset ExpandedAt = new(2026, 11, 7, 0, 0, 0, TimeSpan.Zero);

    // Explicit keys and codes keep IDs stable when content is inserted or reordered.
    private static readonly (int Key, string Code, string Hiragana, string Katakana, string Reading, int Row, int Column)[] AdditionalKana =
    [
        (0, "kana-voiced-ga", "が", "ガ", "ga", 11, 0),
        (1, "kana-voiced-gi", "ぎ", "ギ", "gi", 11, 1),
        (2, "kana-voiced-gu", "ぐ", "グ", "gu", 11, 2),
        (3, "kana-voiced-ge", "げ", "ゲ", "ge", 11, 3),
        (4, "kana-voiced-go", "ご", "ゴ", "go", 11, 4),
        (5, "kana-voiced-za", "ざ", "ザ", "za", 12, 0),
        (6, "kana-voiced-ji", "じ", "ジ", "ji", 12, 1),
        (7, "kana-voiced-zu", "ず", "ズ", "zu", 12, 2),
        (8, "kana-voiced-ze", "ぜ", "ゼ", "ze", 12, 3),
        (9, "kana-voiced-zo", "ぞ", "ゾ", "zo", 12, 4),
        (10, "kana-voiced-da", "だ", "ダ", "da", 13, 0),
        (11, "kana-voiced-di", "ぢ", "ヂ", "ji", 13, 1),
        (12, "kana-voiced-du", "づ", "ヅ", "zu", 13, 2),
        (13, "kana-voiced-de", "で", "デ", "de", 13, 3),
        (14, "kana-voiced-do", "ど", "ド", "do", 13, 4),
        (15, "kana-voiced-ba", "ば", "バ", "ba", 14, 0),
        (16, "kana-voiced-bi", "び", "ビ", "bi", 14, 1),
        (17, "kana-voiced-bu", "ぶ", "ブ", "bu", 14, 2),
        (18, "kana-voiced-be", "べ", "ベ", "be", 14, 3),
        (19, "kana-voiced-bo", "ぼ", "ボ", "bo", 14, 4),
        (20, "kana-voiced-pa", "ぱ", "パ", "pa", 15, 0),
        (21, "kana-voiced-pi", "ぴ", "ピ", "pi", 15, 1),
        (22, "kana-voiced-pu", "ぷ", "プ", "pu", 15, 2),
        (23, "kana-voiced-pe", "ぺ", "ペ", "pe", 15, 3),
        (24, "kana-voiced-po", "ぽ", "ポ", "po", 15, 4),
        (25, "kana-contracted-kya", "きゃ", "キャ", "kya", 16, 0),
        (26, "kana-contracted-kyu", "きゅ", "キュ", "kyu", 16, 2),
        (27, "kana-contracted-kyo", "きょ", "キョ", "kyo", 16, 4),
        (28, "kana-contracted-sha", "しゃ", "シャ", "sha", 17, 0),
        (29, "kana-contracted-shu", "しゅ", "シュ", "shu", 17, 2),
        (30, "kana-contracted-sho", "しょ", "ショ", "sho", 17, 4),
        (31, "kana-contracted-cha", "ちゃ", "チャ", "cha", 18, 0),
        (32, "kana-contracted-chu", "ちゅ", "チュ", "chu", 18, 2),
        (33, "kana-contracted-cho", "ちょ", "チョ", "cho", 18, 4),
        (34, "kana-contracted-nya", "にゃ", "ニャ", "nya", 19, 0),
        (35, "kana-contracted-nyu", "にゅ", "ニュ", "nyu", 19, 2),
        (36, "kana-contracted-nyo", "にょ", "ニョ", "nyo", 19, 4),
        (37, "kana-contracted-hya", "ひゃ", "ヒャ", "hya", 20, 0),
        (38, "kana-contracted-hyu", "ひゅ", "ヒュ", "hyu", 20, 2),
        (39, "kana-contracted-hyo", "ひょ", "ヒョ", "hyo", 20, 4),
        (40, "kana-contracted-mya", "みゃ", "ミャ", "mya", 21, 0),
        (41, "kana-contracted-myu", "みゅ", "ミュ", "myu", 21, 2),
        (42, "kana-contracted-myo", "みょ", "ミョ", "myo", 21, 4),
        (43, "kana-contracted-rya", "りゃ", "リャ", "rya", 22, 0),
        (44, "kana-contracted-ryu", "りゅ", "リュ", "ryu", 22, 2),
        (45, "kana-contracted-ryo", "りょ", "リョ", "ryo", 22, 4),
        (46, "kana-contracted-gya", "ぎゃ", "ギャ", "gya", 23, 0),
        (47, "kana-contracted-gyu", "ぎゅ", "ギュ", "gyu", 23, 2),
        (48, "kana-contracted-gyo", "ぎょ", "ギョ", "gyo", 23, 4),
        (49, "kana-contracted-ja", "じゃ", "ジャ", "ja", 24, 0),
        (50, "kana-contracted-ju", "じゅ", "ジュ", "ju", 24, 2),
        (51, "kana-contracted-jo", "じょ", "ジョ", "jo", 24, 4),
        (52, "kana-contracted-bya", "びゃ", "ビャ", "bya", 25, 0),
        (53, "kana-contracted-byu", "びゅ", "ビュ", "byu", 25, 2),
        (54, "kana-contracted-byo", "びょ", "ビョ", "byo", 25, 4),
        (55, "kana-contracted-pya", "ぴゃ", "ピャ", "pya", 26, 0),
        (56, "kana-contracted-pyu", "ぴゅ", "ピュ", "pyu", 26, 2),
        (57, "kana-contracted-pyo", "ぴょ", "ピョ", "pyo", 26, 4),
    ];

    public static readonly WritingSystemItem[] WritingSystemItemRows = BuildItems();
    public static readonly WritingSystemItemReading[] WritingSystemItemReadingRows = BuildReadings();

    private static WritingSystemItem[] BuildItems()
    {
        var items = new List<WritingSystemItem>();
        for (var script = 0; script < 2; script++)
        for (var index = 0; index < Kana.Length; index++)
        {
            var kana = Kana[index];
            items.Add(WritingSystemItem.Create(JapaneseId + 100 + script * 100 + kana.Key,
                script == 0 ? HiraganaId : KatakanaId, kana.Code, script == 0 ? kana.Hiragana : kana.Katakana,
                index, kana.Row, kana.Column, PublishedAt));
        }
        for (var script = 0; script < 2; script++)
        foreach (var kana in AdditionalKana)
            items.Add(WritingSystemItem.Create(ExpansionId + script * 100 + kana.Key,
                script == 0 ? HiraganaId : KatakanaId, kana.Code, script == 0 ? kana.Hiragana : kana.Katakana,
                46 + kana.Key, kana.Row, kana.Column, ExpandedAt));
        return items.ToArray();
    }

    private static WritingSystemItemReading[] BuildReadings()
    {
        var readings = new List<WritingSystemItemReading>();
        for (var script = 0; script < 2; script++)
        for (var index = 0; index < Kana.Length; index++)
        {
            // "wo" is the chart romanization; contextual pronunciation is a separate rule.
            readings.Add(WritingSystemItemReading.Create(JapaneseId + 300 + script * 100 + Kana[index].Key,
                JapaneseId + 100 + script * 100 + Kana[index].Key, "hepburn", Kana[index].Reading, "hepburn", null, 0, PublishedAt));
        }
        for (var script = 0; script < 2; script++)
        foreach (var kana in AdditionalKana)
            readings.Add(WritingSystemItemReading.Create(ExpansionId + 200 + script * 100 + kana.Key,
                ExpansionId + script * 100 + kana.Key, "hepburn", kana.Reading, "hepburn", null, 0, ExpandedAt));
        return readings.ToArray();
    }
}
