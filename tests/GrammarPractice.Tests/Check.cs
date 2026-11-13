namespace GrammarPractice.Tests;

internal static class Check
{
    public static int Count { get; private set; }
    public static void That(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        Count++;
    }
    public static void Throws<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (T) { Count++; return; }
        throw new InvalidOperationException(message);
    }

    public static async Task ThrowsAsync<T>(Func<Task> action, string message) where T : Exception
    {
        try { await action(); }
        catch (T) { Count++; return; }
        throw new InvalidOperationException(message);
    }
}
