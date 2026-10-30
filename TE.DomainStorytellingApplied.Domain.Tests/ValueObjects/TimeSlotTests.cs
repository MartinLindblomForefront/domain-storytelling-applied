namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeSlotTests
{
    private static DateTime At(int hour)
    {
        return new DateTime(2026, 10, 20, hour, 0, 0, DateTimeKind.Utc);
    }

    [Fact]
    public void GivenEndBeforeStart_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new TimeSlot(At(11), At(10)));
    }

    [Fact]
    public void GivenEndEqualToStart_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new TimeSlot(At(10), At(10)));
    }

    [Fact]
    public void GivenAOneHourSlot_WhenAskingForDuration_ShouldBeOneHour()
    {
        var slot = new TimeSlot(At(10), At(11));

        slot.Duration.ShouldBe(TimeSpan.FromHours(1));
    }

    [Theory]
    [InlineData(10, 12, true)]   // same slot
    [InlineData(11, 13, true)]   // starts inside
    [InlineData(9, 11, true)]    // ends inside
    [InlineData(12, 13, false)]  // starts exactly when the other ends
    [InlineData(8, 10, false)]   // ends exactly when the other starts
    public void GivenAnotherSlot_WhenCheckingOverlap_ShouldReturnExpectedResult(int startHour, int endHour, bool expected)
    {
        var slot = new TimeSlot(At(10), At(12));
        var other = new TimeSlot(At(startHour), At(endHour));

        slot.OverlapsWith(other).ShouldBe(expected);
    }
}
