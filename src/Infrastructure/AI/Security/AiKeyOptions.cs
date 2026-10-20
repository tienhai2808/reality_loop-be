namespace Infrastructure.AI.Security;

public sealed class AiKeyOptions
{
    public const string SectionName = "AI:KeyProtection";
    public string ActiveKeyId { get; init; } = "";
    public Dictionary<string, string> Keys { get; init; } = [];
    public string FingerprintKey { get; init; } = "";

    public bool IsValid()
    {
        static bool ValidId(string id) => id.Length is > 0 and <= 64 && id.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_');
        static bool ValidKey(string value)
        {
            try { return Convert.FromBase64String(value).Length == 32; }
            catch (FormatException) { return false; }
        }
        return ValidId(ActiveKeyId) && Keys.ContainsKey(ActiveKeyId) && Keys.Count is > 0 and <= 20 &&
            Keys.All(x => ValidId(x.Key) && ValidKey(x.Value)) && ValidKey(FingerprintKey) &&
            !Keys.Values.Contains(FingerprintKey, StringComparer.Ordinal);
    }
}
