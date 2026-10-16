using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotTests
{
    [Fact]
    public void GivenEndTimeIsBeforeStartTime_WhenCreating_ShouldThrow() 
    {
        var id = Id.Generate();
        var premisesId = Id.Generate();

        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddHours(-1);

        Should.Throw<Exception>(() => {
            var timeslot = new Timeslot(id, premisesId, startTime, endTime);
        });
    }

    [Fact]
    public void GivenEndTimeIsAfterStartTime_WhenCreating_ShouldNotThrow() 
    {
        var id = Id.Generate();
        var premisesId = Id.Generate();

        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddHours(1);

        var timeslot = new Timeslot(id, premisesId, startTime, endTime);

        timeslot.StartTime.ShouldBe(startTime);
        timeslot.EndTime.ShouldBe(endTime);
    }

    [Fact]
    public void GivenEndTimeIsSameAsStartTime_WhenCreating_ShouldThrow() 
    {
        var id = Id.Generate();
        var premisesId = Id.Generate();

        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime;

        Should.Throw<Exception>(() => {
            var timeslot = new Timeslot(id, premisesId, startTime, endTime);
        });
    }
}