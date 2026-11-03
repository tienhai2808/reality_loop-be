namespace WritingSystemPractice.Tests;

internal sealed class TestClock : TimeProvider
{
    private long _ticks = new DateTimeOffset(2026, 11, 3, 18, 0, 0, TimeSpan.Zero).Ticks;
    public override DateTimeOffset GetUtcNow() => new(Interlocked.Read(ref _ticks), TimeSpan.Zero);
    public void Advance() => Interlocked.Add(ref _ticks, TimeSpan.TicksPerMinute);
}
