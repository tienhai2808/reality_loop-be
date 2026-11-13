using Domain.Languages;
using Domain.WritingSystems;

namespace Infrastructure.Persistence.Seeding.German;

internal static class FoundationSeedData
{
    // Dedicated Snowflake millisecond, generator 1023. Sequence allocations are documented with the release.
    internal const long GermanId = 1_563_703_640_076_578_816L;
    internal const long AlphabetId = GermanId + 10;
    internal static readonly DateTimeOffset PublishedAt = new(2026, 11, 7, 0, 0, 0, TimeSpan.Zero);

    public static readonly Language[] LanguageRows =
    [Language.Create(GermanId, "de", "Deutsch", true, 1, PublishedAt)];

    public static readonly LanguageTranslation[] LanguageTranslationRows =
    [
        LanguageTranslation.Create(GermanId + 1, GermanId, "vi", "Tiếng Đức", "Học chữ cái, phát âm, từ vựng và ngữ pháp tiếng Đức nhập môn.", PublishedAt),
        LanguageTranslation.Create(GermanId + 2, GermanId, "en", "German", "Study the alphabet, pronunciation, introductory German vocabulary and grammar.", PublishedAt)
    ];

    public static readonly WritingSystem[] WritingSystemRows =
    [WritingSystem.Create(AlphabetId, GermanId, "latin", "Lateinische Schrift", true, 0, PublishedAt)];

    public static readonly WritingSystemTranslation[] WritingSystemTranslationRows =
    [
        WritingSystemTranslation.Create(GermanId + 20, AlphabetId, "vi", "Chữ cái tiếng Đức",
            "26 chữ cái A–Z cùng Ä, Ö, Ü và ẞ, có cả chữ hoa và chữ thường, tổng cộng 60 mục luyện. Phiên âm IPA là tên chữ cái, cách đọc trong từ cần đối chiếu quy tắc phát âm.", PublishedAt),
        WritingSystemTranslation.Create(GermanId + 21, AlphabetId, "en", "German alphabet",
            "The 26 letters A–Z plus Ä, Ö, Ü and ẞ in upper and lower case, giving 60 practice items. IPA transcriptions name the letters. Consult pronunciation rules for their sounds within words.", PublishedAt)
    ];

    private static readonly (int Key, string Code, string Upper, string Lower, string Name)[] Letters =
    [
        (0, "a", "A", "a", "aː"),
        (1, "b", "B", "b", "beː"),
        (2, "c", "C", "c", "tseː"),
        (3, "d", "D", "d", "deː"),
        (4, "e", "E", "e", "eː"),
        (5, "f", "F", "f", "ɛf"),
        (6, "g", "G", "g", "ɡeː"),
        (7, "h", "H", "h", "haː"),
        (8, "i", "I", "i", "iː"),
        (9, "j", "J", "j", "jɔt"),
        (10, "k", "K", "k", "kaː"),
        (11, "l", "L", "l", "ɛl"),
        (12, "m", "M", "m", "ɛm"),
        (13, "n", "N", "n", "ɛn"),
        (14, "o", "O", "o", "oː"),
        (15, "p", "P", "p", "peː"),
        (16, "q", "Q", "q", "kuː"),
        (17, "r", "R", "r", "ɛʁ"),
        (18, "s", "S", "s", "ɛs"),
        (19, "t", "T", "t", "teː"),
        (20, "u", "U", "u", "uː"),
        (21, "v", "V", "v", "faʊ"),
        (22, "w", "W", "w", "veː"),
        (23, "x", "X", "x", "ɪks"),
        (24, "y", "Y", "y", "ˈʏpsilɔn"),
        (25, "z", "Z", "z", "tsɛt"),
        (26, "ae", "Ä", "ä", "ɛː"),
        (27, "oe", "Ö", "ö", "øː"),
        (28, "ue", "Ü", "ü", "yː"),
        (29, "eszett", "ẞ", "ß", "ɛsˈtsɛt"),
    ];

    public static readonly WritingSystemItem[] WritingSystemItemRows = BuildItems();
    public static readonly WritingSystemItemReading[] WritingSystemItemReadingRows = BuildReadings();

    private static WritingSystemItem[] BuildItems()
    {
        var items = new List<WritingSystemItem>();
        for (var letterCase = 0; letterCase < 2; letterCase++)
        foreach (var letter in Letters)
            items.Add(WritingSystemItem.Create(GermanId + 100 + letterCase * 30 + letter.Key, AlphabetId,
                (letterCase == 0 ? "upper-" : "lower-") + letter.Code, letterCase == 0 ? letter.Upper : letter.Lower,
                letterCase * 30 + letter.Key, letterCase * 5 + letter.Key / 6, letter.Key % 6, PublishedAt));
        return items.ToArray();
    }

    private static WritingSystemItemReading[] BuildReadings()
    {
        var readings = new List<WritingSystemItemReading>();
        for (var letterCase = 0; letterCase < 2; letterCase++)
        foreach (var letter in Letters)
            readings.Add(WritingSystemItemReading.Create(GermanId + 200 + letterCase * 30 + letter.Key,
                GermanId + 100 + letterCase * 30 + letter.Key, "letter-name", letter.Name, "ipa", null, 0, PublishedAt));
        return readings.ToArray();
    }
}
