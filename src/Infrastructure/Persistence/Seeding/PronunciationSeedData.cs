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

    public static readonly PronunciationRule[] RuleRows =
    [
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
