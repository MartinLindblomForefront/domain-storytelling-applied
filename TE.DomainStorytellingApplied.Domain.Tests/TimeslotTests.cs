namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotTests
{
    [Fact]
    public void GivenEndTimeIsBeforeStartTime_WhenCreating_ShouldThrow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddHours(-1);

        Should.Throw<Exception>(() => new Timeslot(startTime, endTime));
    }

    [Fact]
    public void GivenEndTimeIsSameAsStartTime_WhenCreating_ShouldThrow()
    {
        var startTime = DateTimeOffset.UtcNow;

        Should.Throw<Exception>(() => new Timeslot(startTime, startTime));
    }

    [Fact]
    public void GivenEndTimeIsAfterStartTime_WhenCreating_ShouldSetTimes()
    {
        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddHours(1);

        var timeslot = new Timeslot(startTime, endTime);

        timeslot.StartTime.ShouldBe(startTime);
        timeslot.EndTime.ShouldBe(endTime);
    }

    [Fact]
    public void GivenTwoTimeslotsWithSameTimes_WhenComparing_ShouldBeEqual()
    {
        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddHours(1);

        new Timeslot(startTime, endTime).ShouldBe(new Timeslot(startTime, endTime));
    }
}
