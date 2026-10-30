using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Timeslots;

public class TimeslotTests
{
    private static readonly Guid PremisesId = Guid.NewGuid();

    [Fact]
    public void GivenEndTimeBeforeStartTime_WhenCreatingTimeslot_ShouldThrow()
    {
        var startTime = new DateTime(2026, 10, 9, 12, 0, 0);
        var endTime = new DateTime(2026, 10, 9, 10, 0, 0);

        Should.Throw<ArgumentException>(() => new Timeslot(PremisesId, startTime, endTime));
    }

    [Fact]
    public void GivenStartTimeBeforeEndTime_WhenCreatingTimeslot_ShouldSucceed()
    {
        var startTime = new DateTime(2026, 10, 9, 10, 0, 0);
        var endTime = new DateTime(2026, 10, 9, 12, 0, 0);

        var timeslot = new Timeslot(PremisesId, startTime, endTime);

        timeslot.PremisesId.ShouldBe(PremisesId);
        timeslot.StartTime.ShouldBe(startTime);
        timeslot.EndTime.ShouldBe(endTime);
        timeslot.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void GivenStartTimeEqualToEndTime_WhenCreatingTimeslot_ShouldThrow()
    {
        var time = new DateTime(2026, 10, 9, 10, 0, 0);

        Should.Throw<ArgumentException>(() => new Timeslot(PremisesId, time, time));
    }

    [Fact]
    public void GivenEmptyPremisesId_WhenCreatingTimeslot_ShouldThrow()
    {
        var startTime = new DateTime(2026, 10, 9, 10, 0, 0);
        var endTime = new DateTime(2026, 10, 9, 12, 0, 0);

        Should.Throw<ArgumentException>(() => new Timeslot(Guid.Empty, startTime, endTime));
    }

    [Fact]
    public void GivenEmptyStartTime_WhenCreatingTimeslot_ShouldThrow()
    {
        var endTime = new DateTime(2026, 10, 9, 12, 0, 0);

        Should.Throw<ArgumentException>(() => new Timeslot(PremisesId, default, endTime));
    }

    [Fact]
    public void GivenEmptyEndTime_WhenCreatingTimeslot_ShouldThrow()
    {
        var startTime = new DateTime(2026, 10, 9, 10, 0, 0);

        Should.Throw<ArgumentException>(() => new Timeslot(PremisesId, startTime, default));
    }
}
