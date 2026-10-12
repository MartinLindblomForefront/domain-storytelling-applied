namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotTests
{
    [Fact]
    public void WhenCreatingTimeslotWithValidParameters_ShouldCreateAvailableTimeslot()
    {
        var id = new Id("t1");
        var premisesId = new Id("p1");
        var start = TestData.Now;

        var timeslot = new Timeslot(id, premisesId, start, start.AddHours(1));

        timeslot.Id.ShouldBe(id);
        timeslot.PremisesId.ShouldBe(premisesId);
        timeslot.Start.ShouldBe(start);
        timeslot.End.ShouldBe(start.AddHours(1));
        timeslot.IsReserved.ShouldBeFalse();
    }

    [Fact]
    public void WhenStartEqualsEnd_ShouldThrowException()
    {
        var start = TestData.Now;

        Should.Throw<ArgumentException>(() => new Timeslot(new Id("t1"), new Id("p1"), start, start));
    }

    [Fact]
    public void WhenStartIsAfterEnd_ShouldThrowException()
    {
        var start = TestData.Now;

        Should.Throw<ArgumentException>(() => new Timeslot(new Id("t1"), new Id("p1"), start, start.AddHours(-1)));
    }

    [Fact]
    public void WhenTimeslotsOverlap_ShouldReturnTrue()
    {
        var a = TestData.Timeslot("a", hoursFromNow: 1, durationHours: 2);
        var b = TestData.Timeslot("b", hoursFromNow: 2, durationHours: 2);

        a.Overlaps(b).ShouldBeTrue();
        b.Overlaps(a).ShouldBeTrue();
    }

    [Fact]
    public void WhenTimeslotsOnlyTouch_ShouldNotOverlap()
    {
        var a = TestData.Timeslot("a", hoursFromNow: 1, durationHours: 1);
        var b = TestData.Timeslot("b", hoursFromNow: 2, durationHours: 1);

        a.Overlaps(b).ShouldBeFalse();
    }

    [Fact]
    public void WhenReserving_ShouldBeReserved()
    {
        var timeslot = TestData.Timeslot();

        timeslot.Reserve();

        timeslot.IsReserved.ShouldBeTrue();
    }

    [Fact]
    public void WhenReservingAlreadyReservedTimeslot_ShouldThrowException()
    {
        var timeslot = TestData.Timeslot();
        timeslot.Reserve();

        Should.Throw<InvalidOperationException>(() => timeslot.Reserve());
    }

    [Fact]
    public void WhenReleasingReservedTimeslot_ShouldBeAvailableAgain()
    {
        var timeslot = TestData.Timeslot();
        timeslot.Reserve();

        timeslot.Release();

        timeslot.IsReserved.ShouldBeFalse();
    }

    [Fact]
    public void WhenStartIsAfterNow_ShouldBeInFuture()
    {
        var timeslot = TestData.Timeslot();

        timeslot.IsInFutureOf(timeslot.Start.AddMinutes(-1)).ShouldBeTrue();
    }

    [Fact]
    public void WhenStartIsAtOrBeforeNow_ShouldNotBeInFuture()
    {
        var timeslot = TestData.Timeslot();

        timeslot.IsInFutureOf(timeslot.Start).ShouldBeFalse();
        timeslot.IsInFutureOf(timeslot.Start.AddMinutes(1)).ShouldBeFalse();
    }
}
