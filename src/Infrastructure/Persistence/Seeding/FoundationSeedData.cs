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

    public static readonly LanguageTranslation[] LanguageTranslationRows =
    [
        LanguageTranslation.Create(JapaneseId + 1, JapaneseId, "vi", "Tiếng Nhật", "Bắt đầu với bảng Hiragana và Katakana cơ bản.", PublishedAt),
        LanguageTranslation.Create(JapaneseId + 2, JapaneseId, "en", "Japanese", "Start with the basic Hiragana and Katakana charts.", PublishedAt)
    ];

    public static readonly WritingSystem[] WritingSystemRows =
    [
        WritingSystem.Create(HiraganaId, JapaneseId, "hiragana", "ひらがな", true, 0, PublishedAt),
        WritingSystem.Create(KatakanaId, JapaneseId, "katakana", "カタカナ", true, 1, PublishedAt)
    ];

    public static readonly WritingSystemTranslation[] WritingSystemTranslationRows =
    [
        WritingSystemTranslation.Create(JapaneseId + 20, HiraganaId, "vi", "Hiragana", "Bảng 46 chữ Hiragana cơ bản. Phiên âm chỉ hỗ trợ ghi nhớ; chưa bao gồm âm đục, âm ghép và quy tắc đọc trong câu.", PublishedAt),
        WritingSystemTranslation.Create(JapaneseId + 21, HiraganaId, "en", "Hiragana", "The 46 basic Hiragana. Romanization is a learning aid; voiced sounds, combinations and contextual reading rules are not included yet.", PublishedAt),
        WritingSystemTranslation.Create(JapaneseId + 22, KatakanaId, "vi", "Katakana", "Bảng 46 chữ Katakana cơ bản. Chưa bao gồm âm đục, âm ghép và các tổ hợp mở rộng.", PublishedAt),
        WritingSystemTranslation.Create(JapaneseId + 23, KatakanaId, "en", "Katakana", "The 46 basic Katakana. Voiced sounds, combinations and extended forms are not included yet.", PublishedAt)
    ];

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
        return readings.ToArray();
    }
}
