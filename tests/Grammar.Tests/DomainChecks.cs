using System.Reflection;
using System.Text.Json;
using Domain.Grammar;

namespace Grammar.Tests;

internal static class DomainChecks
{
    internal static readonly DateTimeOffset Now = new(2026, 11, 5, 10, 0, 0, TimeSpan.Zero);
    internal static GrammarPoint Point(long id = 10, string code = "noun-copula", string name = "名詞文", string pattern = "N は N です", int order = 0) =>
        GrammarPoint.Create(id, 1, code, name, pattern, true, order, Now);
    internal static GrammarPointTranslation Translation(string locale = "vi", string name = "Câu danh từ", string explanation = "Giải thích", string? note = null) =>
        GrammarPointTranslation.Create(11, 10, locale, name, explanation, note, Now);

    public static void Run()
    {
        var point = Point(code: " NOUN-COPULA ", name: " 名詞文 ", pattern: " N は N です ");
        Check.That(point.Code == "noun-copula" && point.NativeName == "名詞文" && point.Pattern == "N は N です", "Canonical code and trimmed content.");
        Check.That(point.Version == 1 && point.CreatedAt == Now && point.UpdatedAt == Now, "Initial version/timestamps.");
        var translation = Translation(" EN-US ", " Cafe\u0301 ", " Content ", "  ");
        Check.That(translation.Locale == "en-us" && translation.Name == "Café" && translation.Explanation == "Content" && translation.UsageNote is null,
            "Normalize locale/NFC and optional blank text.");
        Check.That(Translation(explanation: new string('x', 8000), name: new string('n', 150), note: new string('x', 2000)).Explanation.Length == 8000,
            "Translation maximum lengths accepted.");
        Check.That(Point(code: new string('x', 64), name: new string('n', 150), pattern: new string('x', 500)).Pattern.Length == 500, "Point maximum lengths accepted.");
        foreach (var code in new[] { "", " ", "1bad", "bad_code", "a/b", new string('x', 65) })
            Check.Throws<ArgumentException>(() => Point(code: code), "Invalid code rejected.");
        foreach (var name in new[] { "", " \t", "bad\0text", new string('x', 151) })
        {
            Check.Throws<ArgumentException>(() => Point(name: name), "Invalid native name rejected.");
            Check.Throws<ArgumentException>(() => Translation(name: name), "Invalid localized name rejected.");
        }
        foreach (var pattern in new[] { "", " \t", "bad\0text", new string('x', 501) })
            Check.Throws<ArgumentException>(() => Point(pattern: pattern), "Invalid pattern rejected.");
        foreach (var locale in new[] { "", "en_US", "x", "en--us", new string('e', 36) })
            Check.Throws<ArgumentException>(() => Translation(locale), "Invalid locale rejected.");
        foreach (var explanation in new[] { "", " \t", "bad\0text", new string('x', 8001) })
            Check.Throws<ArgumentException>(() => Translation(explanation: explanation), "Invalid explanation rejected.");
        Check.Throws<ArgumentException>(() => Translation(note: new string('x', 2001)), "Note bound enforced.");
        Check.Throws<ArgumentException>(() => Translation(note: "bad\0text"), "Invalid note rejected.");
        Check.Throws<ArgumentOutOfRangeException>(() => Point(0), "Snowflake must be positive.");
        Check.Throws<ArgumentOutOfRangeException>(() => Point(order: -1), "Order must be non-negative.");
        Check.Throws<ArgumentOutOfRangeException>(() => GrammarPoint.Create(10, 0, "point", "名詞文", "N", true, 0, Now), "Language ID must be positive.");
        Check.Throws<ArgumentOutOfRangeException>(() => GrammarPointTranslation.Create(0, 10, "en", "Title", "Content", null, Now), "Translation ID must be positive.");
        Check.Throws<ArgumentOutOfRangeException>(() => GrammarPointTranslation.Create(11, 0, "en", "Title", "Content", null, Now), "Parent ID must be positive.");
        var created = point.CreatedAt;
        point.Update("変更", "N ですか", false, 2, Now.AddMinutes(1).ToOffset(TimeSpan.FromHours(7)));
        Check.That(point.Version == 2 && point.CreatedAt == created && point.UpdatedAt == Now.AddMinutes(1) && point.UpdatedAt.Offset == TimeSpan.Zero &&
            point.Code == "noun-copula" && point.LanguageId == 1 && !point.IsPublished && point.SortOrder == 2, "Update controls mutation and preserves identity.");
        translation.Update(" Updated ", " New content ", " Note ", Now.AddMinutes(1));
        Check.That(translation.Version == 2 && translation.Locale == "en-us" && translation.GrammarPointId == 10 &&
            translation.Name == "Updated" && translation.Explanation == "New content" && translation.UsageNote == "Note" && translation.CreatedAt == Now,
            "Translation update preserves locale/parent/creation time.");
        var pointBefore = JsonSerializer.Serialize(point);
        var translationBefore = JsonSerializer.Serialize(translation);
        Check.Throws<ArgumentException>(() => point.Update("変更", "", true, 0, Now.AddMinutes(2)), "Invalid update rejected before mutation.");
        Check.Throws<ArgumentException>(() => point.Update("変更", "N", true, 0, Now), "Point cannot backdate.");
        Check.Throws<ArgumentException>(() => translation.Update("Changed", "", null, Now.AddMinutes(2)), "Invalid translation update rejected.");
        Check.Throws<ArgumentException>(() => translation.Update("Changed", "Content", null, Now), "Translation cannot backdate.");
        Check.That(JsonSerializer.Serialize(point) == pointBefore && JsonSerializer.Serialize(translation) == translationBefore, "Invalid updates are atomic.");
        foreach (var entity in new object[] { point, translation })
        {
            var type = entity.GetType();
            type.GetProperty("Version")!.SetValue(entity, long.MaxValue);
            var before = JsonSerializer.Serialize(entity, type);
            Check.Throws<OverflowException>(() =>
            {
                if (entity is GrammarPoint p) p.Update("Changed", "N", true, 0, Now.AddMinutes(2));
                else ((GrammarPointTranslation)entity).Update("Changed", "Content", null, Now.AddMinutes(2));
            }, "Version overflow rejected.");
            Check.That(before == JsonSerializer.Serialize(entity, type), "Overflow preserves every field.");
            Check.That(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0 &&
                !type.GetProperties().Any(x => x.SetMethod?.IsPublic == true), "Factory controlled entities.");
        }
        var utc = GrammarPoint.Create(12, 1, "utc", "UTC", "N", true, 0, Now.ToOffset(TimeSpan.FromHours(7)));
        Check.That(utc.CreatedAt == Now && utc.CreatedAt.Offset == TimeSpan.Zero, "Creation UTC normalization.");
    }
}
