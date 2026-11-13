using Domain.Pronunciation;

namespace Infrastructure.Persistence.Seeding.German;

internal static class PronunciationSeedData
{
    private const long SeedBase = FoundationSeedData.GermanId;
    private static readonly (int Key, string Code, string NativeName, string VietnameseName, string EnglishName,
        string VietnameseExplanation, string EnglishExplanation, string VietnameseNote, string EnglishNote)[] Rules =
    [
        (0, "vowel-length", "Lange und kurze Vokale", "Nguyên âm dài và ngắn", "Long and short vowels", "Độ dài và chất lượng nguyên âm có thể phân biệt từ. So sánh Beet với Bett và Miete với Mitte. Dấu ː trong IPA biểu thị độ dài.", "Vowel length and quality can distinguish words. Compare Beet with Bett and Miete with Mitte. IPA ː marks length.", "Không suy ra toàn bộ cách đọc chỉ từ một chữ cái. Học độ dài cùng từng từ và ngữ cảnh.", "Do not infer the entire pronunciation from a single letter. Learn length with the word and context."),
        (1, "umlauts", "Umlautvokale", "Nguyên âm ä, ö, ü", "Umlaut vowels", "ä, ö và ü biểu thị những nguyên âm cần phân biệt với a, o và u. Với ö và ü, giữ môi tròn trong khi đưa lưỡi về phía trước.", "ä, ö and ü represent vowels to distinguish from a, o and u. For ö and ü, round the lips while keeping the tongue forward.", "Cả ö và ü đều có biến thể dài/ngắn. ä cũng thay đổi theo từ và cách nói chuẩn vùng miền.", "Both ö and ü have long and short variants. ä also varies by word and regional standard."),
        (2, "ei-and-ie", "ei und ie", "Phân biệt ei và ie", "Distinguishing ei and ie", "ei thường biểu thị âm đôi /aɪ/, còn ie thường biểu thị nguyên âm /iː/. So sánh Wein và viel.", "ei commonly represents the diphthong /aɪ/, while ie commonly represents /iː/. Compare Wein and viel.", "Đây là mẫu đọc thông dụng. Từ mượn và một số cách viết riêng có thể khác.", "These are common patterns. Loanwords and individual spellings can differ."),
        (3, "diphthongs", "au, eu und äu", "Âm đôi au, eu và äu", "Diphthongs au, eu and äu", "au thường đọc /aʊ/, eu và äu thường đọc /ɔʏ/. So sánh Haus với Häuser.", "au commonly represents /aʊ/, while eu and äu commonly represent /ɔʏ/. Compare Haus and Häuser.", "Giữ hai phần của âm đôi trong cùng một âm tiết.", "Keep the two vowel movements within one syllable."),
        (4, "ich-ach", "Ich-Laut und Ach-Laut", "Hai cách đọc ch", "The ich and ach sounds", "ch thường đọc /ç/ sau nguyên âm trước như i, và /x/ sau a, o, u. So sánh ich với Buch.", "ch commonly represents /ç/ after front vowels such as i, and /x/ after a, o or u. Compare ich and Buch.", "Đây là hướng dẫn cơ bản. ch ở đầu từ hoặc trong từ mượn có thể có cách đọc khác.", "This is an introductory guide. Word-initial ch and loanwords can have other pronunciations."),
        (5, "sch-sp-st", "sch, sp und st", "Các nhóm sch, sp, st", "The groups sch, sp and st", "sch thường đọc /ʃ/. sp và st thường bắt đầu bằng /ʃp/ và /ʃt/ ở đầu thân từ, như spielen và Straße.", "sch commonly represents /ʃ/. At the start of a word stem, sp and st commonly represent /ʃp/ and /ʃt/, as in spielen and Straße.", "Không áp dụng máy móc cách đọc đầu từ cho mọi sp hoặc st ở giữa và cuối từ.", "Do not apply the word-initial pattern mechanically to every medial or final sp or st."),
        (6, "z-w-v", "z, w und v", "Cách đọc z, w và v", "The letters z, w and v", "z thường đọc /ts/, w thường đọc /v/. v đọc /f/ trong nhiều từ gốc Đức như Vater nhưng có thể đọc /v/ trong từ mượn như Vase.", "z commonly represents /ts/ and w commonly /v/. v represents /f/ in many native words such as Vater, but can represent /v/ in loanwords such as Vase.", "Tên chữ cái khác âm của chữ trong từ. V có tên /faʊ/ nhưng từ Vater bắt đầu bằng /f/.", "Letter names differ from their sounds within words. V is named /faʊ/, but Vater begins with /f/."),
        (7, "s-ss-eszett", "s, ss und ß", "Phân biệt s, ss và ß", "Distinguishing s, ss and ß", "s trước nguyên âm thường đọc /z/ ở đầu âm tiết trong nhiều cách nói chuẩn. ss và ß thường đọc /s/. Cách viết ss/ß liên quan độ dài nguyên âm trước đó.", "Syllable-initial s before a vowel commonly represents /z/ in many standard varieties. ss and ß commonly represent /s/. Their spelling relates to the preceding vowel's length.", "Sau nguyên âm ngắn thường viết ss, sau nguyên âm dài hoặc âm đôi thường viết ß. Quy ước tiếng Đức ở Thụy Sĩ dùng ss thay ß.", "A short vowel commonly precedes ss, while a long vowel or diphthong commonly precedes ß. Swiss German spelling uses ss in place of ß."),
        (8, "final-devoicing", "Auslautverhärtung", "Vô thanh hóa phụ âm cuối", "Final devoicing", "Các phụ âm b, d và g thường vô thanh ở cuối âm tiết. Tag kết thúc bằng /k/, Hund bằng /t/ và lieb bằng /p/.", "b, d and g commonly devoice at syllable endings. Tag ends in /k/, Hund in /t/ and lieb in /p/.", "Cách viết vẫn giữ phụ âm gốc. Biến đổi dạng từ có thể làm âm hữu thanh xuất hiện lại.", "The spelling retains the underlying consonant. Another form of the word can reveal voicing again."),
        (9, "r-variants", "Aussprache des r", "Âm r và các biến thể", "Pronouncing r", "Tiếng Đức chuẩn chấp nhận nhiều cách phát âm r. Trong nhiều cách nói, r sau nguyên âm được nguyên âm hóa, như đuôi -er trong Mutter.", "Standard German allows several realizations of r. In many varieties, post-vocalic r is vocalized, as in the ending -er in Mutter.", "Ví dụ dùng một cách phiên âm rộng thông dụng. Khác biệt vùng miền phù hợp vẫn có thể đúng.", "The examples use one common broad transcription. Appropriate regional variants can also be correct."),
        (10, "silent-h", "Dehnungs-h", "Chữ h chỉ độ dài nguyên âm", "Length-marking h", "Trong nhiều từ, h sau nguyên âm chỉ độ dài và không được đọc như h ở đầu từ. Ví dụ gồm wohnen và fahren.", "In many words, h after a vowel signals length and is not pronounced like word-initial h. Examples include wohnen and fahren.", "Phân biệt với h đầu từ trong Haus, được phát âm. Đối chiếu toàn bộ từ.", "Distinguish word-initial h in Haus, which is pronounced. Check the whole word."),
        (11, "word-stress", "Wortakzent", "Trọng âm từ", "Word stress", "Học trọng âm cùng từ mới. IPA ˈ đặt trước âm tiết mang trọng âm chính. Tiền tố và nguồn gốc từ có thể ảnh hưởng vị trí trọng âm.", "Learn stress with new words. IPA ˈ precedes the syllable with primary stress. Prefixes and a word's origin can affect stress placement.", "Tiền tố tách thường mang trọng âm; nhiều tiền tố không tách như be- và ver- thường không mang trọng âm chính.", "Separable prefixes are commonly stressed; many inseparable prefixes such as be- and ver- commonly lack primary stress."),
    ];

    private static readonly (int Key, int Rule, string Text, string Transcription, int Order)[] Examples =
    [
        (0, 0, "Beet", "beːt", 0),
        (1, 0, "Bett", "bɛt", 1),
        (2, 0, "Miete", "ˈmiːtə", 2),
        (3, 0, "Mitte", "ˈmɪtə", 3),
        (4, 0, "Ofen", "ˈoːfən", 4),
        (5, 1, "fünf", "fʏnf", 0),
        (6, 1, "schön", "ʃøːn", 1),
        (7, 1, "Tür", "tyːɐ̯", 2),
        (8, 1, "Bär", "bɛːɐ̯", 3),
        (9, 1, "Mütter", "ˈmʏtɐ", 4),
        (10, 2, "Eis", "aɪs", 0),
        (11, 2, "Reise", "ˈʁaɪzə", 1),
        (12, 2, "Wein", "vaɪn", 2),
        (13, 2, "Liebe", "ˈliːbə", 3),
        (14, 2, "viel", "fiːl", 4),
        (15, 3, "Haus", "haʊs", 0),
        (16, 3, "heute", "ˈhɔʏtə", 1),
        (17, 3, "neu", "nɔʏ", 2),
        (18, 3, "Häuser", "ˈhɔʏzɐ", 3),
        (19, 3, "Auto", "ˈaʊto", 4),
        (20, 4, "ich", "ɪç", 0),
        (21, 4, "nicht", "nɪçt", 1),
        (22, 4, "Nacht", "naxt", 2),
        (23, 4, "Buch", "buːx", 3),
        (24, 4, "Küche", "ˈkʏçə", 4),
        (25, 5, "Schule", "ˈʃuːlə", 0),
        (26, 5, "spielen", "ˈʃpiːlən", 1),
        (27, 5, "Straße", "ˈʃtʁaːsə", 2),
        (28, 5, "Fisch", "fɪʃ", 3),
        (29, 5, "Stadt", "ʃtat", 4),
        (30, 6, "Zeit", "tsaɪt", 0),
        (31, 6, "Wasser", "ˈvasɐ", 1),
        (32, 6, "Vater", "ˈfaːtɐ", 2),
        (33, 6, "vier", "fiːɐ̯", 3),
        (34, 6, "Vase", "ˈvaːzə", 4),
        (35, 7, "Sonne", "ˈzɔnə", 0),
        (36, 7, "groß", "ɡʁoːs", 1),
        (37, 7, "Fuß", "fuːs", 2),
        (38, 7, "Fluss", "flʊs", 3),
        (39, 7, "heißen", "ˈhaɪsən", 4),
        (40, 8, "Tag", "taːk", 0),
        (41, 8, "Hund", "hʊnt", 1),
        (42, 8, "lieb", "liːp", 2),
        (43, 8, "Bad", "baːt", 3),
        (44, 8, "Zug", "tsuːk", 4),
        (45, 9, "rot", "ʁoːt", 0),
        (46, 9, "Brot", "bʁoːt", 1),
        (47, 9, "Mutter", "ˈmʊtɐ", 2),
        (48, 9, "vier", "fiːɐ̯", 3),
        (49, 9, "Lehrer", "ˈleːʁɐ", 4),
        (50, 10, "sehen", "ˈzeːən", 0),
        (51, 10, "gehen", "ˈɡeːən", 1),
        (52, 10, "Uhr", "uːɐ̯", 2),
        (53, 10, "wohnen", "ˈvoːnən", 3),
        (54, 10, "fahren", "ˈfaːʁən", 4),
        (55, 11, "bezahlen", "bəˈtsaːlən", 0),
        (56, 11, "verstehen", "fɛɐ̯ˈʃteːən", 1),
        (57, 11, "aufstehen", "ˈaʊfˌʃteːən", 2),
        (58, 11, "arbeiten", "ˈaʁbaɪtən", 3),
        (59, 11, "lernen", "ˈlɛʁnən", 4),
    ];

    public static readonly PronunciationRule[] RuleRows = Rules.Select(rule => PronunciationRule.Create(
        SeedBase + 500 + rule.Key, SeedBase, rule.Code, rule.NativeName, true, rule.Key, FoundationSeedData.PublishedAt)).ToArray();

    public static readonly PronunciationRuleTranslation[] TranslationRows = Rules.SelectMany(rule => new[]
    {
        PronunciationRuleTranslation.Create(SeedBase + 550 + rule.Key * 2, SeedBase + 500 + rule.Key,
            "vi", rule.VietnameseName, rule.VietnameseExplanation, rule.VietnameseNote, FoundationSeedData.PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 551 + rule.Key * 2, SeedBase + 500 + rule.Key,
            "en", rule.EnglishName, rule.EnglishExplanation, rule.EnglishNote, FoundationSeedData.PublishedAt)
    }).ToArray();

    public static readonly PronunciationExample[] ExampleRows = Examples.Select(example => PronunciationExample.Create(
        SeedBase + 600 + example.Key, SeedBase + 500 + example.Rule, $"example-{example.Key}", example.Text,
        example.Transcription, "ipa", null, example.Order, FoundationSeedData.PublishedAt)).ToArray();
}
