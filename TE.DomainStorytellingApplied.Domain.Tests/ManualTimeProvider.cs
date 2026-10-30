namespace TE.DomainStorytellingApplied.Domain.Tests;

internal sealed class ManualTimeProvider : TimeProvider
{
    private long ticks = new DateTimeOffset(2026, 11, 1, 8, 0, 0, TimeSpan.Zero).Ticks;
    public override DateTimeOffset GetUtcNow() => new(Interlocked.Read(ref ticks), TimeSpan.Zero);
    public void Advance(TimeSpan duration) => Interlocked.Add(ref ticks, duration.Ticks);
}
