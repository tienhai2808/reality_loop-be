namespace VocabularyLearning.Tests;

internal static class Check
{
    public static int Count { get; private set; }
    public static void That(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
        Count++;
    }

    public static void Throws<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (T) { Count++; return; }
        throw new InvalidOperationException(message);
    }
}
