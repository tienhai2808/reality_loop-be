namespace GrammarPractice.Tests;

internal sealed class TestClock : TimeProvider
{
    private static readonly DateTimeOffset Now = new(2026, 11, 11, 18, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => Now;
}
