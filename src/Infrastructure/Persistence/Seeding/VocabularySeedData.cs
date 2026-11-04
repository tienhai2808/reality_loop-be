using Domain.Vocabulary;

namespace Infrastructure.Persistence.Seeding;

internal static class VocabularySeedData
{
    // Fixed Snowflake allocation: offsets 2000..2283 within the existing Japanese seed's millisecond.
    // Foundation uses 0..445; pronunciation uses 1000..1114. Never derive identities from array position.
    private const long SeedBase = FoundationSeedData.JapaneseId + 2000;
    private static readonly DateTimeOffset PublishedAt = new(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);

    private static readonly (int Key, string Code, string Term, string Reading, int Order)[] Entries =
    [
        (0, "mizu", "水", "みず", 0),
        (1, "ocha", "お茶", "おちゃ", 1),
        (2, "gohan", "ご飯", "ごはん", 2),
        (3, "pan", "パン", "パン", 3),
        (4, "gyuunyuu", "牛乳", "ぎゅうにゅう", 4),
        (5, "sakana", "魚", "さかな", 5),
        (6, "niku", "肉", "にく", 6),
        (7, "yasai", "野菜", "やさい", 7),
        (8, "kudamono", "果物", "くだもの", 8),
        (9, "hon", "本", "ほん", 9),
        (10, "gakkou", "学校", "がっこう", 10),
        (11, "gakusei", "学生", "がくせい", 11),
        (12, "sensei", "先生", "せんせい", 12),
        (13, "tomodachi", "友達", "ともだち", 13),
        (14, "kazoku", "家族", "かぞく", 14),
        (15, "ie", "家", "いえ", 15),
        (16, "heya", "部屋", "へや", 16),
        (17, "eki", "駅", "えき", 17),
        (18, "densha", "電車", "でんしゃ", 18),
        (19, "mise", "店", "みせ", 19),
        (20, "inu", "犬", "いぬ", 20),
        (21, "neko", "猫", "ねこ", 21),
        (22, "kyou", "今日", "きょう", 22),
        (23, "ashita", "明日", "あした", 23),
        (24, "kinou", "昨日", "きのう", 24),
        (25, "asa", "朝", "あさ", 25),
        (26, "yoru", "夜", "よる", 26),
        (27, "taberu", "食べる", "たべる", 27),
        (28, "nomu", "飲む", "のむ", 28),
        (29, "iku", "行く", "いく", 29),
        (30, "kuru", "来る", "くる", 30),
        (31, "kiku", "聞く", "きく", 31),
        (32, "yomu", "読む", "よむ", 32),
        (33, "kaku", "書く", "かく", 33),
        (34, "hanasu", "話す", "はなす", 34),
        (35, "ookii", "大きい", "おおきい", 35),
        (36, "chiisai", "小さい", "ちいさい", 36),
        (37, "atarashii", "新しい", "あたらしい", 37),
        (38, "furui", "古い", "ふるい", 38),
        (39, "yukkuri", "ゆっくり", "ゆっくり", 39)
    ];

    // Original concise meanings/notes, not an imported dictionary or a copy of textbook explanations.
    // Keys are explicit across all senses so adding a meaning never renumbers existing rows.
    private static readonly (int Key, int EntryKey, string Code, PartOfSpeech PartOfSpeech, int Order,
        string Vietnamese, string English, string? VietnameseNote, string? EnglishNote)[] Senses =
    [
        (0, 0, "water", PartOfSpeech.Noun, 0, "Nước.", "Water.", null, null),
        (1, 1, "tea", PartOfSpeech.Noun, 0, "Trà; nước trà.", "Tea.", null, null),
        (2, 2, "cooked-rice", PartOfSpeech.Noun, 0, "Cơm; gạo đã nấu chín.", "Cooked rice.",
            "Phân biệt nghĩa chỉ cơm với nghĩa chỉ cả bữa ăn của cùng từ.", "Distinguish rice itself from the separate meal sense."),
        (3, 3, "bread", PartOfSpeech.Noun, 0, "Bánh mì.", "Bread.", null, null),
        (4, 4, "milk", PartOfSpeech.Noun, 0, "Sữa bò.", "Cow's milk.", null, null),
        (5, 5, "fish", PartOfSpeech.Noun, 0, "Cá.", "Fish.", null, null),
        (6, 6, "meat", PartOfSpeech.Noun, 0, "Thịt.", "Meat.", null, null),
        (7, 7, "vegetables", PartOfSpeech.Noun, 0, "Rau; rau củ.", "Vegetables.", null, null),
        (8, 8, "fruit", PartOfSpeech.Noun, 0, "Trái cây; hoa quả.", "Fruit.", null, null),
        (9, 9, "book", PartOfSpeech.Noun, 0, "Sách.", "A book.", null, null),
        (10, 10, "school", PartOfSpeech.Noun, 0, "Trường học.", "A school.", null, null),
        (11, 11, "student", PartOfSpeech.Noun, 0, "Học sinh; sinh viên.", "A student.", null, null),
        (12, 12, "teacher", PartOfSpeech.Noun, 0, "Giáo viên; thầy, cô.", "A teacher.",
            "Ở đây giới thiệu nghĩa giáo viên; từ này còn được dùng để xưng gọi một số người có chuyên môn.",
            "This entry teaches the teacher sense; the same word also addresses some professionals."),
        (13, 13, "friend", PartOfSpeech.Noun, 0, "Bạn bè; người bạn.", "A friend.", null, null),
        (14, 14, "family", PartOfSpeech.Noun, 0, "Gia đình.", "Family.", null, null),
        (15, 15, "home", PartOfSpeech.Noun, 0, "Nhà; nơi ở.", "A house or home.", null, null),
        (16, 16, "room", PartOfSpeech.Noun, 0, "Phòng; căn phòng.", "A room.", null, null),
        (17, 17, "station", PartOfSpeech.Noun, 0, "Nhà ga.", "A railway station.", null, null),
        (18, 18, "train", PartOfSpeech.Noun, 0, "Tàu điện.", "An electric train.", null, null),
        (19, 19, "shop", PartOfSpeech.Noun, 0, "Cửa hàng; tiệm.", "A shop.", null, null),
        (20, 20, "dog", PartOfSpeech.Noun, 0, "Con chó.", "A dog.", null, null),
        (21, 21, "cat", PartOfSpeech.Noun, 0, "Con mèo.", "A cat.", null, null),
        (22, 22, "today", PartOfSpeech.Noun, 0, "Hôm nay.", "Today.", null, null),
        (23, 23, "tomorrow", PartOfSpeech.Noun, 0, "Ngày mai.", "Tomorrow.", null, null),
        (24, 24, "yesterday", PartOfSpeech.Noun, 0, "Hôm qua.", "Yesterday.", null, null),
        (25, 25, "morning", PartOfSpeech.Noun, 0, "Buổi sáng.", "Morning.", null, null),
        (26, 26, "night", PartOfSpeech.Noun, 0, "Buổi tối; ban đêm.", "Night or evening.", null, null),
        (27, 27, "eat", PartOfSpeech.Verb, 0, "Ăn.", "To eat.", null, null),
        (28, 28, "drink", PartOfSpeech.Verb, 0, "Uống.", "To drink.", null, null),
        (29, 29, "go", PartOfSpeech.Verb, 0, "Đi.", "To go.", null, null),
        (30, 30, "come", PartOfSpeech.Verb, 0, "Đến; tới.", "To come.", null, null),
        (31, 31, "listen", PartOfSpeech.Verb, 0, "Nghe; lắng nghe.", "To hear or listen.",
            "Cùng cách viết và cách đọc còn có nghĩa hỏi; chọn nghĩa theo ngữ cảnh.",
            "The same spelling and reading also mean to ask; context determines the sense."),
        (32, 32, "read", PartOfSpeech.Verb, 0, "Đọc.", "To read.", null, null),
        (33, 33, "write", PartOfSpeech.Verb, 0, "Viết.", "To write.", null, null),
        (34, 34, "speak", PartOfSpeech.Verb, 0, "Nói; trò chuyện.", "To speak or talk.", null, null),
        (35, 35, "big", PartOfSpeech.Adjective, 0, "To; lớn.", "Big or large.", null, null),
        (36, 36, "small", PartOfSpeech.Adjective, 0, "Nhỏ; bé.", "Small.", null, null),
        (37, 37, "new", PartOfSpeech.Adjective, 0, "Mới.", "New.", null, null),
        (38, 38, "old", PartOfSpeech.Adjective, 0, "Cũ; có từ lâu.", "Old; not new.",
            "Không dùng tính từ này để nói một người cao tuổi.", "Do not use this adjective to describe an elderly person."),
        (39, 39, "slowly", PartOfSpeech.Adverb, 0, "Chậm rãi; từ từ.", "Slowly; at an unhurried pace.", null, null),
        (40, 2, "meal", PartOfSpeech.Noun, 1, "Bữa ăn.", "A meal.",
            "Nghĩa này nói đến cả bữa ăn, không chỉ riêng cơm.", "This sense refers to the whole meal rather than rice alone."),
        (41, 31, "ask", PartOfSpeech.Verb, 1, "Hỏi; hỏi để biết thông tin.", "To ask for information.",
            "Phân biệt với nghĩa nghe của cùng từ.", "Distinguish this sense from hearing or listening.")
    ];

    public static readonly VocabularyEntry[] EntryRows = Entries.Select(row => VocabularyEntry.Create(
        SeedBase + row.Key, FoundationSeedData.JapaneseId, row.Code, row.Term, row.Reading, null, true, row.Order, PublishedAt)).ToArray();

    public static readonly VocabularySense[] SenseRows = Senses.Select(row => VocabularySense.Create(
        SeedBase + 100 + row.Key, SeedBase + row.EntryKey, row.Code, row.PartOfSpeech, row.Order, PublishedAt)).ToArray();

    public static readonly VocabularySenseTranslation[] TranslationRows = Senses.SelectMany(row => new[]
    {
        VocabularySenseTranslation.Create(SeedBase + 200 + row.Key * 2, SeedBase + 100 + row.Key, "vi",
            row.Vietnamese, row.VietnameseNote, PublishedAt),
        VocabularySenseTranslation.Create(SeedBase + 201 + row.Key * 2, SeedBase + 100 + row.Key, "en",
            row.English, row.EnglishNote, PublishedAt)
    }).ToArray();
}
