namespace TE.DomainStorytellingApplied.Domain.Tests;

public class CapacityTests
{
    [Fact]
    public void GivenPositiveCapacity_WhenCreating_ShouldKeepValue()
    {
        var capacity = Capacity.Create(12);

        capacity.Value.ShouldBe(12);
    }

    [Fact]
    public void GivenZeroCapacity_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Capacity.Create(0));
    }

    [Fact]
    public void GivenNegativeCapacity_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Capacity.Create(-1));
    }
}