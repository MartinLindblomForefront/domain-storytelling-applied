namespace TE.DomainStorytellingApplied.Domain.Tests.Timeslots;

using TE.DomainStorytellingApplied.Domain.Timeslots;

public class TimeslotTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void GivenStartBeforeEnd_WhenCreatingTimeslot_ShouldExposeStartAndEnd()
    {
        var end = Start.AddHours(2);

        var timeslot = new Timeslot(Start, end);

        timeslot.Start.ShouldBe(Start);
        timeslot.End.ShouldBe(end);
    }

    [Fact]
    public void GivenStartBeforeEnd_WhenReadingDuration_ShouldBeEndMinusStart()
    {
        var timeslot = new Timeslot(Start, Start.AddMinutes(90));

        timeslot.Duration.ShouldBe(TimeSpan.FromMinutes(90));
    }

    [Fact]
    public void GivenEndEqualToStart_WhenCreatingTimeslot_ShouldThrowInvalidTimeslotException()
    {
        Should.Throw<InvalidTimeslotException>(() => new Timeslot(Start, Start));
    }

    [Fact]
    public void GivenEndBeforeStart_WhenCreatingTimeslot_ShouldThrowInvalidTimeslotException()
    {
        Should.Throw<InvalidTimeslotException>(() => new Timeslot(Start, Start.AddMinutes(-1)));
    }

    [Fact]
    public void GivenInvalidTimeslot_WhenCreatingTimeslot_ShouldThrowDomainException()
    {
        var exception = Should.Throw<InvalidTimeslotException>(() => new Timeslot(Start, Start));

        exception.ShouldBeAssignableTo<DomainException>();
        exception.Message.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GivenTwoTimeslotsWithSameStartAndEnd_WhenComparing_ShouldBeEqual()
    {
        var first = new Timeslot(Start, Start.AddHours(1));
        var second = new Timeslot(Start, Start.AddHours(1));

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenTwoTimeslotsWithDifferentEnd_WhenComparing_ShouldNotBeEqual()
    {
        var first = new Timeslot(Start, Start.AddHours(1));
        var second = new Timeslot(Start, Start.AddHours(2));

        first.ShouldNotBe(second);
    }
}
