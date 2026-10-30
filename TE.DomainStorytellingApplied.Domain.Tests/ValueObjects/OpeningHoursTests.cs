namespace TE.DomainStorytellingApplied.Domain.Tests;

public class OpeningHoursTests
{
    [Fact]
    public void GivenClosesBeforeOpens_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new OpeningHours(new TimeOnly(22, 0), new TimeOnly(8, 0)));
    }

    [Fact]
    public void GivenClosesEqualToOpens_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new OpeningHours(new TimeOnly(8, 0), new TimeOnly(8, 0)));
    }

    [Theory]
    [InlineData(10, 11, true)]   // in the middle of the day
    [InlineData(8, 22, true)]    // exactly from opening to closing
    [InlineData(7, 9, false)]    // starts before opening
    [InlineData(21, 23, false)]  // ends after closing
    public void GivenATimeSlotOnOneDay_WhenCheckingCovers_ShouldReturnExpectedResult(int startHour, int endHour, bool expected)
    {
        var openingHours = new OpeningHours(new TimeOnly(8, 0), new TimeOnly(22, 0));
        var timeSlot = new TimeSlot(
            new DateTime(2026, 10, 20, startHour, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 20, endHour, 0, 0, DateTimeKind.Utc));

        openingHours.Covers(timeSlot).ShouldBe(expected);
    }

    [Fact]
    public void GivenATimeSlotThatCrossesMidnight_WhenCheckingCovers_ShouldBeFalse()
    {
        var openingHours = new OpeningHours(new TimeOnly(8, 0), new TimeOnly(22, 0));

        // Both times of day are within the opening hours, but the slot runs through the night.
        var timeSlot = new TimeSlot(
            new DateTime(2026, 10, 20, 21, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 21, 9, 0, 0, DateTimeKind.Utc));

        openingHours.Covers(timeSlot).ShouldBeFalse();
    }
}
