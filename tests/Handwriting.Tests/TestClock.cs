namespace Handwriting.Tests;

internal sealed class TestClock : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);
}
