namespace WritingSystemPractice.Tests;

internal static class Check
{
    public static int Count { get; private set; }

    public static void That(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        Count++;
    }

    public static void Throws<TException>(Action action, string message) where TException : Exception
    {
        try { action(); }
        catch (TException) { Count++; return; }
        throw new InvalidOperationException(message);
    }

    public static async Task ThrowsAsync<TException>(Func<Task> action, string message) where TException : Exception
    {
        try { await action(); }
        catch (TException) { Count++; return; }
        throw new InvalidOperationException(message);
    }
}
