namespace VocabularyLearning.Tests;

internal sealed class TestClock : TimeProvider
{
    private long _ticks = DomainChecks.Now.UtcTicks;
    public override DateTimeOffset GetUtcNow() => new(Interlocked.Read(ref _ticks), TimeSpan.Zero);
    public void Advance(TimeSpan delta) => Interlocked.Add(ref _ticks, delta.Ticks);
}
