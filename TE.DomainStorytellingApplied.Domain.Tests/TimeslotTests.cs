namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotTests
{
    private static readonly DateTimeOffset Ten = new(2030, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Eleven = new(2030, 1, 1, 11, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Twelve = new(2030, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void GivenStartBeforeEnd_WhenCreatingTimeslot_ShouldSetStartAndEnd()
    {
        var timeslot = new Timeslot(Ten, Eleven);

        timeslot.Start.ShouldBe(Ten);
        timeslot.End.ShouldBe(Eleven);
    }

    [Fact]
    public void GivenStartAfterEnd_WhenCreatingTimeslot_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Timeslot(Eleven, Ten));
    }

    [Fact]
    public void GivenStartEqualToEnd_WhenCreatingTimeslot_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Timeslot(Ten, Ten));
    }

    [Fact]
    public void GivenTimeslot_WhenGettingDuration_ShouldBeEndMinusStart()
    {
        var timeslot = new Timeslot(Ten, Twelve);

        timeslot.Duration.ShouldBe(TimeSpan.FromHours(2));
    }

    [Fact]
    public void GivenOverlappingTimeslots_WhenCheckingOverlap_ShouldBeTrue()
    {
        var first = new Timeslot(Ten, Twelve);
        var second = new Timeslot(Eleven, Twelve.AddHours(1));

        first.Overlaps(second).ShouldBeTrue();
        second.Overlaps(first).ShouldBeTrue();
    }

    [Fact]
    public void GivenTimeslotContainedInAnother_WhenCheckingOverlap_ShouldBeTrue()
    {
        var outer = new Timeslot(Ten, Twelve);
        var inner = new Timeslot(Ten.AddMinutes(15), Eleven);

        outer.Overlaps(inner).ShouldBeTrue();
        inner.Overlaps(outer).ShouldBeTrue();
    }

    [Fact]
    public void GivenAdjacentTimeslots_WhenCheckingOverlap_ShouldBeFalse()
    {
        var first = new Timeslot(Ten, Eleven);
        var second = new Timeslot(Eleven, Twelve);

        first.Overlaps(second).ShouldBeFalse();
        second.Overlaps(first).ShouldBeFalse();
    }

    [Fact]
    public void GivenSeparateTimeslots_WhenCheckingOverlap_ShouldBeFalse()
    {
        var first = new Timeslot(Ten, Ten.AddMinutes(30));
        var second = new Timeslot(Eleven, Twelve);

        first.Overlaps(second).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotsWithDifferentOffsets_WhenCheckingOverlap_ShouldCompareActualPointInTime()
    {
        var utc = new Timeslot(Ten, Eleven);
        var stockholmOverlapping = new Timeslot(
            new DateTimeOffset(2030, 1, 1, 11, 30, 0, TimeSpan.FromHours(1)),
            new DateTimeOffset(2030, 1, 1, 13, 0, 0, TimeSpan.FromHours(1)));
        var stockholmAdjacent = new Timeslot(
            new DateTimeOffset(2030, 1, 1, 12, 0, 0, TimeSpan.FromHours(1)),
            new DateTimeOffset(2030, 1, 1, 13, 0, 0, TimeSpan.FromHours(1)));

        utc.Overlaps(stockholmOverlapping).ShouldBeTrue();
        utc.Overlaps(stockholmAdjacent).ShouldBeFalse();
    }

    [Fact]
    public void GivenNoOtherTimeslot_WhenCheckingOverlap_ShouldThrow()
    {
        var timeslot = new Timeslot(Ten, Eleven);

        Should.Throw<ArgumentNullException>(() => timeslot.Overlaps(null!));
    }

    [Fact]
    public void GivenTimeslotsWithSameStartAndEnd_WhenComparing_ShouldBeEqual()
    {
        var first = new Timeslot(Ten, Eleven);
        var second = new Timeslot(Ten, Eleven);

        first.ShouldBe(second);
    }
}
