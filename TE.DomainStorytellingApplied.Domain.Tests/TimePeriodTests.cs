using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimePeriodTests
{

    [Fact]
    public void GivenEndAfterStart_WhenCreated_ShouldKeepTheTimes()
    {
        var start = new DateTime(2026, 11, 1, 10, 0, 0);
        var end = start.AddHours(1);

        var period = new TimePeriod(start, end);

        period.Start.ShouldBe(start);
        period.End.ShouldBe(end);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void GivenEndAtOrBeforeStart_WhenCreated_ShouldThrow(int minutes)
    {
        var start = new DateTime(2026, 11, 1, 10, 0, 0);

        var exception = Should.Throw<ArgumentException>(
            () => new TimePeriod(start, start.AddMinutes(minutes)));

        exception.ParamName.ShouldBe("end");
    }
    [Theory]
    [InlineData(9, 11, true)]
    [InlineData(11, 13, true)]
    [InlineData(9, 13, true)]
    [InlineData(10, 11, true)]
    [InlineData(10, 12, true)]
    [InlineData(8, 9, false)]
    [InlineData(9, 10, false)]
    [InlineData(12, 13, false)]
    [InlineData(13, 14, false)]
    public void GivenPeriods_WhenCheckingOverlap_ShouldCompareSymmetrically(
        int startHour, int endHour, bool expected)
    {
        var period = new TimePeriod(
            new DateTime(2026, 11, 1, 10, 0, 0),
            new DateTime(2026, 11, 1, 12, 0, 0));
        var other = new TimePeriod(
            new DateTime(2026, 11, 1, startHour, 0, 0),
            new DateTime(2026, 11, 1, endHour, 0, 0));

        period.Overlaps(other).ShouldBe(expected);
        other.Overlaps(period).ShouldBe(expected);
    }

    [Fact]
    public void GivenSameEndpoints_WhenCreatedSeparately_ShouldBeEqualByValue()
    {
        var start = new DateTime(2026, 11, 1, 10, 0, 0);
        var first = new TimePeriod(start, start.AddHours(1));
        var second = new TimePeriod(start, start.AddHours(1));

        first.ShouldNotBeSameAs(second);
        first.ShouldBe(second);
    }
}
