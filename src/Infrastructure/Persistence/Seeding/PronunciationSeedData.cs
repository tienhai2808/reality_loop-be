using Domain.Pronunciation;

namespace Infrastructure.Persistence.Seeding;

internal static class PronunciationSeedData
{
    // Fixed Snowflake allocation within the existing Japanese seed's millisecond.
    // Offsets 1000..1114 do not overlap the foundation seed (offsets 0..445).
    private const long SeedBase = FoundationSeedData.JapaneseId + 1000;
    private const long VoicedSoundsId = SeedBase;
    private const long ContractedSoundsId = SeedBase + 1;
    private const long GeminateConsonantsId = SeedBase + 2;
    private const long LongVowelsId = SeedBase + 3;
    private const long ParticleReadingsId = SeedBase + 4;
    private static readonly DateTimeOffset PublishedAt = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);

    private const long ExpansionBase = FoundationSeedData.ExpansionId + 2400;
    private static readonly (int Key, string Code, string NativeName, string VietnameseName, string EnglishName,
        string VietnameseExplanation, string EnglishExplanation, string VietnameseNote, string EnglishNote)[] AdditionalRules =
    [
        (0, "mora-timing", "拍のリズム", "Nhịp mora", "Mora timing", "Đếm theo mora, đơn vị nhịp của tiếng Nhật. Một kana thường là một mora, nhưng âm ghép như きゃ là một mora; ん, っ và phần kéo dài nguyên âm đều chiếm nhịp riêng.", "Count Japanese rhythm in morae. A kana usually represents one mora, while a contracted sound such as きゃ is one mora; ん, っ and the extension of a long vowel each occupy their own timing unit.", "Giữ nhịp tương đối đều. Các dấu nối trong phiên âm ví dụ chỉ dùng để minh họa cách đếm nhịp.", "Keep the timing relatively even. Hyphens in the example transcriptions illustrate mora boundaries."),
        (1, "syllabic-n", "撥音ん", "Âm ん", "The moraic nasal n", "ん chiếm một mora riêng. Vị trí phát âm thay đổi theo âm đi sau nên không cố đọc mọi ん giống n trong tiếng Việt hoặc tiếng Anh.", "ん occupies a separate mora. Its articulation changes with the following sound, so avoid forcing every ん into one fixed English or Vietnamese n sound.", "Trong Hepburn có thể dùng dấu nháy sau n trước nguyên âm hoặc y để tránh đọc nhầm, như ほんや hon'ya.", "Hepburn can use an apostrophe after n before a vowel or y to clarify the boundary, as in ほんや hon'ya."),
        (2, "vowel-devoicing", "母音の無声化", "Nguyên âm có thể bị vô thanh", "Vowel devoicing", "Trong nhiều cách nói chuẩn, i và u giữa các phụ âm vô thanh có thể rất nhẹ hoặc bị vô thanh. Điều này thường gặp trong すき và phần cuối です, ます tùy ngữ cảnh.", "In many standard speech contexts, i and u between voiceless consonants can be very weak or devoiced. Common examples include すき and the endings です and ます, depending on context.", "Đây là xu hướng phát âm, không phải quy tắc xóa nguyên âm khỏi cách viết. Mức độ thay đổi theo người nói và ngữ cảnh.", "This is a pronunciation tendency. Keep the vowel in the spelling. The degree varies by speaker and context."),
        (3, "long-vowel-spelling", "長母音のつづり", "Cách viết nguyên âm dài", "Spelling long vowels", "Học độ dài cùng cách viết từng từ. Âm dài o thường viết おう hoặc おお, âm dài e thường viết えい hoặc ええ. Trong Katakana thường dùng ー.", "Learn vowel length together with each word's spelling. Long o is often written おう or おお, and long e often えい or ええ. Katakana commonly uses ー.", "Không đổi mọi おう hoặc えい thành một âm dài trong mọi ngữ cảnh; đối chiếu từ và cách nói. Phiên âm mẫu dùng dấu dài Hepburn.", "Do not treat every おう or えい sequence identically in all contexts. Check the word and speech style. The examples use Hepburn macrons."),
        (4, "loanword-combinations", "外来語の特別な音", "Tổ hợp Katakana trong từ mượn", "Katakana loanword combinations", "Katakana có thể kết hợp với nguyên âm nhỏ để biểu thị âm trong từ mượn, như ファ fa, ティ ti, フォ fo. Chữ nhỏ kết hợp với chữ trước trong cùng một nhịp.", "Katakana can combine with small vowels to represent loanword sounds, such as ファ fa, ティ ti and フォ fo. The small character combines with the preceding one within a single mora.", "Phân biệt フィ với フイ và ティ với テイ. Đây là tổ hợp mở rộng, ngoài bảng âm ghép với ャ・ュ・ョ.", "Distinguish フィ from フイ and ティ from テイ. These combinations extend beyond those with ャ, ュ and ョ."),
        (5, "ji-zu-spelling", "じ・ぢ・ず・づ", "Phân biệt cách viết じ・ぢ・ず・づ", "The spellings ji and zu", "Trong cách nói chuẩn hiện đại, じ và ぢ thường cùng được đọc ji, còn ず và づ thường cùng được đọc zu. Cách viết vẫn cần học theo từng từ.", "In modern standard speech, じ and ぢ usually share the reading ji, and ず and づ usually share zu. Learn the spelling of each word even when the pronunciation matches.", "Hepburn trong ứng dụng giữ cùng ji hoặc zu cho các âm tương ứng. Một số phương ngữ vẫn phân biệt chúng.", "The app's Hepburn transcriptions use ji or zu for the matching sounds. Some dialects preserve distinctions."),
        (6, "pitch-and-intonation", "アクセントとイントネーション", "Cao độ và ngữ điệu", "Pitch and intonation", "Tiếng Nhật dùng cao độ và ngữ điệu để phân biệt hoặc làm rõ lời nói. Chỉ nhìn romaji không đủ để biết đường cao độ của một từ hoặc câu.", "Japanese uses pitch and intonation to distinguish words and shape utterances. Romanization alone does not specify a word or sentence's pitch pattern.", "Các ví dụ chỉ cung cấp cách đọc chữ, chưa ghi mẫu trọng âm cao độ. Học bằng bản ghi của đúng từ và ngữ cảnh, tránh tự suy ra cao độ từ romaji.", "The examples provide segmental readings without pitch-accent annotations. Learn from recordings of the actual word and context."),
    ];

    private static readonly (int Key, int Rule, string Code, string Text, string Transcription, int Order)[] AdditionalExamples =
    [
        (0, 0, "gi", "ぎ", "gi", 3),
        (1, 0, "po", "ポ", "po", 4),
        (2, 1, "kyou", "きょう", "kyō", 3),
        (3, 1, "byouin", "びょういん", "byōin", 4),
        (4, 2, "zasshi", "ざっし", "zasshi", 3),
        (5, 2, "beddo", "ベッド", "beddo", 4),
        (6, 3, "sensei", "せんせい", "sensei", 3),
        (7, 3, "keeki", "ケーキ", "kēki", 4),
        (8, 4, "greeting-konnichiwa", "こんにちは", "konnichiwa", 3),
        (9, 4, "greeting-konbanwa", "こんばんは", "konbanwa", 4),
        (10, 5, "mora-kana", "かな", "ka-na", 0),
        (11, 5, "mora-kya", "きゃ", "kya", 1),
        (12, 5, "mora-kitte", "きって", "ki-t-te", 2),
        (13, 5, "mora-san", "さん", "sa-n", 3),
        (14, 5, "mora-obasan", "おばあさん", "o-ba-a-sa-n", 4),
        (15, 6, "san", "さん", "san", 0),
        (16, 6, "hon", "ほん", "hon", 1),
        (17, 6, "honya", "ほんや", "hon'ya", 2),
        (18, 6, "shinbun", "しんぶん", "shinbun", 3),
        (19, 6, "tenin", "てんいん", "ten'in", 4),
        (20, 7, "suki", "すき", "suki", 0),
        (21, 7, "kutsu", "くつ", "kutsu", 1),
        (22, 7, "desu", "です", "desu", 2),
        (23, 7, "masu", "ます", "masu", 3),
        (24, 7, "hito", "ひと", "hito", 4),
        (25, 8, "ookii", "おおきい", "ōkii", 0),
        (26, 8, "tokei", "とけい", "tokei", 1),
        (27, 8, "oneesan", "おねえさん", "onēsan", 2),
        (28, 8, "suupaa", "スーパー", "sūpā", 3),
        (29, 8, "toukyou", "とうきょう", "Tōkyō", 4),
        (30, 9, "fairu", "ファイル", "fairu", 0),
        (31, 9, "firumu", "フィルム", "firumu", 1),
        (32, 9, "fooku", "フォーク", "fōku", 2),
        (33, 9, "tiishatsu", "ティーシャツ", "tīshatsu", 3),
        (34, 9, "shefu", "シェフ", "shefu", 4),
        (35, 10, "jikan", "じかん", "jikan", 0),
        (36, 10, "hanaji", "はなぢ", "hanaji", 1),
        (37, 10, "mizu", "みず", "mizu", 2),
        (38, 10, "tsuzuku", "つづく", "tsuzuku", 3),
        (39, 10, "chijimu", "ちぢむ", "chijimu", 4),
        (40, 11, "hashi-bridge", "はし", "hashi", 0),
        (41, 11, "ame-rain", "あめ", "ame", 1),
        (42, 11, "sakura", "さくら", "sakura", 2),
        (43, 11, "gakkou", "がっこう", "gakkō", 3),
        (44, 11, "nihongo", "にほんご", "nihongo", 4),
    ];

    public static readonly PronunciationRule[] RuleRows =
    [
        .. AdditionalRules.Select(row => PronunciationRule.Create(ExpansionBase + row.Key,
            FoundationSeedData.JapaneseId, row.Code, row.NativeName, true, 5 + row.Key, FoundationSeedData.ExpandedAt)),
        PronunciationRule.Create(VoicedSoundsId, FoundationSeedData.JapaneseId,
            "dakuten-handakuten", "濁音・半濁音", true, 0, PublishedAt),
        PronunciationRule.Create(ContractedSoundsId, FoundationSeedData.JapaneseId,
            "contracted-sounds", "拗音", true, 1, PublishedAt),
        PronunciationRule.Create(GeminateConsonantsId, FoundationSeedData.JapaneseId,
            "geminate-consonants", "促音", true, 2, PublishedAt),
        PronunciationRule.Create(LongVowelsId, FoundationSeedData.JapaneseId,
            "long-vowels", "長音", true, 3, PublishedAt),
        PronunciationRule.Create(ParticleReadingsId, FoundationSeedData.JapaneseId,
            "particle-readings", "助詞の読み方", true, 4, PublishedAt)
    ];

    // Original introductory explanations. Reference material is listed in docs/pronunciation-model.md.
    public static readonly PronunciationRuleTranslation[] TranslationRows =
    [
        .. AdditionalRules.SelectMany(row => new[]
        {
            PronunciationRuleTranslation.Create(ExpansionBase + 20 + row.Key * 2, ExpansionBase + row.Key,
                "vi", row.VietnameseName, row.VietnameseExplanation, row.VietnameseNote, FoundationSeedData.ExpandedAt),
            PronunciationRuleTranslation.Create(ExpansionBase + 21 + row.Key * 2, ExpansionBase + row.Key,
                "en", row.EnglishName, row.EnglishExplanation, row.EnglishNote, FoundationSeedData.ExpandedAt)
        }),
        PronunciationRuleTranslation.Create(SeedBase + 20, VoicedSoundsId, "vi", "Âm đục và âm bán đục",
            "Dấu dakuten ゛ làm đổi cách đọc của một số kana, như か thành が và は thành ば. Dấu handakuten ゜ dùng với hàng は để tạo hàng âm p, như は thành ぱ. Các dấu này cũng xuất hiện trong Katakana.",
            "Quan sát cả chữ và dấu đi kèm; không đọc chữ đã thêm dấu như chữ gốc.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 21, VoicedSoundsId, "en", "Voiced and p sounds",
            "Dakuten ゛ changes certain kana readings, such as か to が and は to ば. Handakuten ゜ forms the p series from the は row, such as は to ぱ. The same marks are used in Katakana.",
            "Read the complete marked character rather than treating the mark as decoration.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 22, ContractedSoundsId, "vi", "Âm ghép với ゃ・ゅ・ょ nhỏ",
            "Một số kana hàng i kết hợp với ゃ, ゅ hoặc ょ nhỏ thành một đơn vị nhịp, như きゃ, しゅ, ちょ. Phân biệt きゃ với きや có や lớn: cách chia nhịp khác nhau.",
            "Katakana dùng ャ, ュ, ョ nhỏ theo cùng nguyên tắc; chú ý kích thước của chữ thứ hai.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 23, ContractedSoundsId, "en", "Contracted sounds with small ya, yu and yo",
            "Certain i-row kana combine with small ゃ, ゅ or ょ into one timing unit, as in きゃ, しゅ and ちょ. Compare きゃ with きや: the full-sized や adds a separate timing unit.",
            "Katakana uses small ャ, ュ and ョ in the same way. Check the size of the second character.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 24, GeminateConsonantsId, "vi", "Âm ngắt với っ nhỏ",
            "Chữ っ nhỏ biểu thị một nhịp kéo dài phụ âm kế tiếp. Tùy phụ âm, giữ chặn hoặc kéo dài luồng âm; không đọc thành tsu riêng. Trong きって, phần tt cần dài hơn một t đơn.",
            "Katakana dùng ッ nhỏ. Phân biệt với つ hoặc ツ lớn, được đọc là tsu.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 25, GeminateConsonantsId, "en", "Long consonants with small tsu",
            "Small っ adds a timing unit to the following consonant. Hold the closure or extend the consonant as appropriate; do not pronounce a separate tsu. In きって, tt lasts longer than a single t.",
            "Katakana uses small ッ. Full-sized つ and ツ are read as tsu instead.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 26, LongVowelsId, "vi", "Nguyên âm dài",
            "Giữ nguyên âm dài thêm một nhịp. Độ dài có thể phân biệt từ: おばさん và おばあさん khác nhau. Cách viết thể hiện độ dài tùy từ; trong Katakana thường gặp dấu ー, như コーヒー.",
            "Phiên âm ở đây dùng dấu dài Hepburn như ā, ī, ō. Không coi mọi chuỗi hai nguyên âm là một quy tắc duy nhất; đối chiếu từng ví dụ.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 27, LongVowelsId, "en", "Long vowels",
            "Sustain a long vowel for an extra timing unit. Length can distinguish words, as in おばさん and おばあさん. Spelling varies between words; Katakana often marks vowel length with ー, as in コーヒー.",
            "These examples use Hepburn macrons such as ā, ī and ō. Do not assume every pair of vowel letters follows one identical rule.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 28, ParticleReadingsId, "vi", "Cách đọc は・へ・を khi là trợ từ",
            "Khi dùng làm trợ từ, は được đọc là wa, へ là e và を thường được đọc là o. Giữ cách viết kana của trợ từ; phiên âm trong ví dụ thể hiện cách đọc theo ngữ cảnh.",
            "Chỉ áp dụng cách đọc này đúng ngữ cảnh trợ từ; không đổi mọi は trong từ thành wa hoặc mọi へ thành e.", PublishedAt),
        PronunciationRuleTranslation.Create(SeedBase + 29, ParticleReadingsId, "en", "Reading the particles ha, he and wo",
            "As particles, は is read wa, へ is read e and を is normally read o. Keep their kana spelling; the example transcriptions show their contextual readings.",
            "Apply these readings in particle contexts. Do not change every は within a word to wa or every へ to e.", PublishedAt)
    ];

    public static readonly PronunciationExample[] ExampleRows =
    [
        .. AdditionalExamples.Select(row => PronunciationExample.Create(ExpansionBase + 100 + row.Key,
            row.Rule < 5 ? SeedBase + row.Rule : ExpansionBase + row.Rule - 5,
            row.Code, row.Text, row.Transcription, row.Rule == 5 ? "mora-guide" : "hepburn", null, row.Order, FoundationSeedData.ExpandedAt)),
        PronunciationExample.Create(SeedBase + 100, VoicedSoundsId, "ga", "が", "ga", "hepburn", null, 0, PublishedAt),
        PronunciationExample.Create(SeedBase + 101, VoicedSoundsId, "ba", "ば", "ba", "hepburn", null, 1, PublishedAt),
        PronunciationExample.Create(SeedBase + 102, VoicedSoundsId, "pa", "ぱ", "pa", "hepburn", null, 2, PublishedAt),
        PronunciationExample.Create(SeedBase + 103, ContractedSoundsId, "kyaku", "きゃく", "kyaku", "hepburn", null, 0, PublishedAt),
        PronunciationExample.Create(SeedBase + 104, ContractedSoundsId, "shumi", "しゅみ", "shumi", "hepburn", null, 1, PublishedAt),
        PronunciationExample.Create(SeedBase + 105, ContractedSoundsId, "choko", "チョコ", "choko", "hepburn", null, 2, PublishedAt),
        PronunciationExample.Create(SeedBase + 106, GeminateConsonantsId, "kitte", "きって", "kitte", "hepburn", null, 0, PublishedAt),
        PronunciationExample.Create(SeedBase + 107, GeminateConsonantsId, "gakkou", "がっこう", "gakkō", "hepburn", null, 1, PublishedAt),
        PronunciationExample.Create(SeedBase + 108, GeminateConsonantsId, "kappu", "カップ", "kappu", "hepburn", null, 2, PublishedAt),
        PronunciationExample.Create(SeedBase + 109, LongVowelsId, "obaasan", "おばあさん", "obāsan", "hepburn", null, 0, PublishedAt),
        PronunciationExample.Create(SeedBase + 110, LongVowelsId, "ojiisan", "おじいさん", "ojīsan", "hepburn", null, 1, PublishedAt),
        PronunciationExample.Create(SeedBase + 111, LongVowelsId, "coffee", "コーヒー", "kōhī", "hepburn", null, 2, PublishedAt),
        PronunciationExample.Create(SeedBase + 112, ParticleReadingsId, "topic-wa", "わたしは がくせいです。",
            "watashi wa gakusei desu.", "hepburn", null, 0, PublishedAt),
        PronunciationExample.Create(SeedBase + 113, ParticleReadingsId, "direction-e", "がっこうへ いきます。",
            "gakkō e ikimasu.", "hepburn", null, 1, PublishedAt),
        PronunciationExample.Create(SeedBase + 114, ParticleReadingsId, "object-o", "みずを のみます。",
            "mizu o nomimasu.", "hepburn", null, 2, PublishedAt)
    ];
}
