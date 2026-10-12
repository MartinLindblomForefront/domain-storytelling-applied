namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ValueObjectTests
{
    [Fact]
    public void WhenCreatingIdWithValue_ShouldExposeValue()
    {
        new Id("x").Value.ShouldBe("x");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void WhenCreatingIdWithBlankValue_ShouldThrowException(string value)
    {
        Should.Throw<ArgumentException>(() => new Id(value));
    }

    [Fact]
    public void WhenComparingIdsWithSameValue_ShouldBeEqual()
    {
        new Id("x").ShouldBe(new Id("x"));
    }

    [Fact]
    public void WhenCreatingMaxActiveReservationsWithPositiveValue_ShouldExposeValue()
    {
        new MaxActiveReservations(3).Value.ShouldBe(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WhenCreatingMaxActiveReservationsWithNonPositiveValue_ShouldThrowException(int value)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new MaxActiveReservations(value));
    }

    [Fact]
    public void WhenComparingPolicyViolationsWithSameCode_ShouldBeEqual()
    {
        PolicyViolation.TimeslotInPast.ShouldBe(PolicyViolation.TimeslotInPast);
        PolicyViolation.TimeslotInPast.ShouldNotBe(PolicyViolation.TimeslotAlreadyReserved);
    }
}
