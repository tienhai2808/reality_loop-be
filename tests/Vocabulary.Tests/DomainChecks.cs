using Domain.Vocabulary;

namespace Vocabulary.Tests;

internal static class DomainChecks
{
    public static void Run()
    {
        var now = new DateTimeOffset(2026, 11, 4, 10, 0, 0, TimeSpan.FromHours(7));
        VocabularyEntry Entry() => VocabularyEntry.Create(1, 2, " WATER ", "  Cafe\u0301  ", "  みず  ", null, true, 0, now);
        VocabularySense Sense() => VocabularySense.Create(3, 1, " LIQUID ", PartOfSpeech.Noun, 0, now);
        VocabularySenseTranslation Translation() => VocabularySenseTranslation.Create(4, 3, " EN-us ", "  Cafe\u0301  ", " \t ", now);
        var entry = Entry(); var sense = Sense(); var translation = Translation();
        Check.That(entry.Code == "water" && entry.Term == "Café" && entry.Reading == "みず", "Entry trims/NFC and normalizes stable code.");
        Check.That(entry.Id == 1 && entry.LanguageId == 2 && entry.IsPublished && entry.SortOrder == 0 && entry.AudioUrl is null, "Initial entry content.");
        Check.That(entry.Version == 1 && entry.CreatedAt == now && entry.UpdatedAt == now && entry.CreatedAt.Offset == TimeSpan.Zero, "Initial UTC timestamps/version.");
        Check.That(sense.Code == "liquid" && sense.VocabularyEntryId == 1 && sense.PartOfSpeech == PartOfSpeech.Noun && sense.Version == 1, "Sense owns POS independently of translation.");
        Check.That(translation.Locale == "en-us" && translation.Meaning == "Café" && translation.UsageNote is null && translation.Version == 1, "Translation normalizes locale/text/optional note.");
        Check.That(VocabularyEntry.Create(10, 2, "plain", "パン", " ", " \t ", false, 0, now).Reading is null, "Optional reading/audio can be blank and normalize to null.");
        var longTerm = new string('a', VocabularyEntry.TermMaxLength);
        Check.That(VocabularyEntry.Create(10, 2, new string('a', 64), longTerm, new string('a', 200), null, false, 0, now).Term == longTerm, "Exact entry length limits accepted.");
        Check.That(VocabularySenseTranslation.Create(4, 3, "vi", new string('a', 500), new string('a', 2000), now).UsageNote!.Length == 2000, "Exact translation length limits accepted.");
        foreach (var value in Enum.GetValues<PartOfSpeech>())
            Check.That(VocabularySense.Create(3, 1, "sense", value, 0, now).PartOfSpeech == value, $"Defined POS {value} accepted.");

        entry.Update("水", null, "https://media.example/audio/water.mp3", false, 2, now.AddMinutes(1));
        Check.That(entry.Term == "水" && entry.Reading is null && !entry.IsPublished && entry.SortOrder == 2 && entry.AudioUrl!.EndsWith("water.mp3"), "Controlled entry update and nullable reading.");
        Check.That(entry.Version == 2 && entry.CreatedAt == now && entry.UpdatedAt == now.AddMinutes(1) && entry.Code == "water" && entry.LanguageId == 2, "Update preserves identity/creation and increments version.");
        sense.Update(PartOfSpeech.Verb, 1, now.AddMinutes(1));
        Check.That(sense.Version == 2 && sense.PartOfSpeech == PartOfSpeech.Verb && sense.SortOrder == 1 && sense.Code == "liquid" && sense.VocabularyEntryId == 1, "Sense update preserves stable parent/code.");
        translation.Update("  Nước  ", "  Ghi chú  ", now.AddMinutes(1));
        Check.That(translation.Meaning == "Nước" && translation.UsageNote == "Ghi chú" && translation.Version == 2 && translation.Locale == "en-us" && translation.VocabularySenseId == 3, "Translation update preserves parent/locale.");
        Check.That(sense.CreatedAt.Offset == TimeSpan.Zero && translation.UpdatedAt.Offset == TimeSpan.Zero, "Child timestamps are UTC.");
        entry.Update(entry.Term, entry.Reading, null, entry.IsPublished, entry.SortOrder, entry.UpdatedAt);
        Check.That(entry.Version == 3 && entry.AudioUrl is null, "Catalog update increments version even for equal time; audio may be cleared.");

        foreach (var id in new[] { 0L, -1L })
        {
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularyEntry.Create(id, 2, "word", "word", null, null, false, 0, now), "Positive entry ID required.");
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularyEntry.Create(1, id, "word", "word", null, null, false, 0, now), "Positive language ID required.");
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularySense.Create(id, 1, "sense", PartOfSpeech.Noun, 0, now), "Positive sense ID required.");
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularySense.Create(3, id, "sense", PartOfSpeech.Noun, 0, now), "Positive entry FK required.");
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularySenseTranslation.Create(id, 3, "en", "meaning", null, now), "Positive translation ID required.");
            Check.Throws<ArgumentOutOfRangeException>(() => VocabularySenseTranslation.Create(4, id, "en", "meaning", null, now), "Positive sense FK required.");
        }
        foreach (var code in new[] { "", "\t", "1word", "two_words", "two words", new string('a', 65) })
        {
            Check.Throws<ArgumentException>(() => VocabularyEntry.Create(1, 2, code, "term", null, null, true, 0, now), "Invalid entry code rejected.");
            Check.Throws<ArgumentException>(() => VocabularySense.Create(3, 1, code, PartOfSpeech.Noun, 0, now), "Invalid sense code rejected.");
        }
        foreach (var term in new[] { "", " \t ", "term\0", new string('a', 201) })
            Check.Throws<ArgumentException>(() => VocabularyEntry.Create(1, 2, "word", term, null, null, true, 0, now), "Blank/NUL/oversized term rejected.");
        foreach (var reading in new[] { "reading\0", new string('a', 201) })
            Check.Throws<ArgumentException>(() => VocabularyEntry.Create(1, 2, "word", "term", reading, null, true, 0, now), "Invalid optional reading rejected.");
        foreach (var audio in new[] { "http://media.example/a.mp3", "/audio.mp3", "https://u:p@media.example/a.mp3", "https://media.example/a.mp3#frag", "https://media.example/a b.mp3", "https://media.example\\a.mp3", "https://media.example/" + new string('a', 1024) })
            Check.Throws<ArgumentException>(() => VocabularyEntry.Create(1, 2, "word", "term", null, audio, true, 0, now), "Invalid/oversized audio URL rejected.");
        Check.Throws<ArgumentOutOfRangeException>(() => VocabularyEntry.Create(1, 2, "word", "term", null, null, true, -1, now), "Negative entry order rejected.");
        Check.Throws<ArgumentOutOfRangeException>(() => VocabularySense.Create(3, 1, "sense", PartOfSpeech.Noun, -1, now), "Negative sense order rejected.");
        Check.Throws<ArgumentOutOfRangeException>(() => VocabularySense.Create(3, 1, "sense", (PartOfSpeech)99, 0, now), "Undefined POS rejected.");
        foreach (var locale in new[] { "", "en_US", "x", "en--us", "EN-TOOLONGTAG", "en-12345678-12345678-12345678-12345678" })
            Check.Throws<ArgumentException>(() => VocabularySenseTranslation.Create(4, 3, locale, "meaning", null, now), "Invalid locale rejected.");
        foreach (var meaning in new[] { "", " \t ", "text\0", new string('a', 501) })
            Check.Throws<ArgumentException>(() => VocabularySenseTranslation.Create(4, 3, "vi", meaning, null, now), "Invalid required meaning rejected.");
        Check.Throws<ArgumentException>(() => VocabularySenseTranslation.Create(4, 3, "vi", "meaning", new string('a', 2001), now), "Oversized note rejected.");
        Check.Throws<ArgumentException>(() => VocabularySenseTranslation.Create(4, 3, "vi", "meaning", "note\0", now), "NUL note rejected.");

        var invalidEntry = Entry();
        Check.Throws<ArgumentException>(() => invalidEntry.Update("Changed", "changed", "http://bad", false, 4, now.AddMinutes(1)), "Rejected update cannot partially change entry.");
        Check.That(invalidEntry.Term == "Café" && invalidEntry.Version == 1 && invalidEntry.IsPublished, "Invalid entry update leaves state intact.");
        Check.Throws<ArgumentException>(() => invalidEntry.Update("Changed", null, null, false, 0, now.AddSeconds(-1)), "Backdated entry update rejected.");
        var invalidSense = Sense();
        Check.Throws<ArgumentOutOfRangeException>(() => invalidSense.Update((PartOfSpeech)(-1), 4, now), "Invalid POS update rejected.");
        Check.Throws<ArgumentException>(() => invalidSense.Update(PartOfSpeech.Verb, 4, now.AddSeconds(-1)), "Backdated sense update rejected.");
        Check.That(invalidSense.Version == 1 && invalidSense.PartOfSpeech == PartOfSpeech.Noun && invalidSense.SortOrder == 0, "Rejected sense update leaves state intact.");
        var invalidTranslation = Translation();
        Check.Throws<ArgumentException>(() => invalidTranslation.Update("New", new string('a', 2001), now), "Invalid translation update rejected.");
        Check.Throws<ArgumentException>(() => invalidTranslation.Update("New", null, now.AddSeconds(-1)), "Backdated translation update rejected.");
        Check.That(invalidTranslation.Meaning == "Café" && invalidTranslation.Version == 1, "Rejected translation update leaves state intact.");

        var overflowEntry = Entry();
        typeof(VocabularyEntry).GetProperty("Version")!.SetValue(overflowEntry, long.MaxValue);
        Check.Throws<OverflowException>(() => overflowEntry.Update("Changed", null, null, false, 1, now), "Entry version overflow rejected.");
        Check.That(overflowEntry.Term == "Café" && overflowEntry.IsPublished && overflowEntry.UpdatedAt == now, "Entry overflow cannot partially mutate state.");
        var overflowSense = Sense();
        typeof(VocabularySense).GetProperty("Version")!.SetValue(overflowSense, long.MaxValue);
        Check.Throws<OverflowException>(() => overflowSense.Update(PartOfSpeech.Verb, 4, now), "Sense version overflow rejected.");
        Check.That(overflowSense.PartOfSpeech == PartOfSpeech.Noun && overflowSense.SortOrder == 0, "Sense overflow cannot partially mutate state.");
        var overflowTranslation = Translation();
        typeof(VocabularySenseTranslation).GetProperty("Version")!.SetValue(overflowTranslation, long.MaxValue);
        Check.Throws<OverflowException>(() => overflowTranslation.Update("New", "note", now), "Translation version overflow rejected.");
        Check.That(overflowTranslation.Meaning == "Café" && overflowTranslation.UsageNote is null, "Translation overflow cannot partially mutate state.");
        foreach (var type in new[] { typeof(VocabularyEntry), typeof(VocabularySense), typeof(VocabularySenseTranslation) })
            Check.That(type.GetProperties().All(x => x.SetMethod?.IsPublic != true) && type.GetConstructors().Length == 0,
                $"{type.Name} has no unrestricted public setters or constructor.");
    }
}
