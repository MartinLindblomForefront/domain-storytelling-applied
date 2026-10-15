namespace TE.DomainStorytellingApplied.Domain.Tests.Fakes;

public sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now.ToUniversalTime();
}
