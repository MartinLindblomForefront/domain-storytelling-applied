using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationServiceTests
{
    private readonly InMemoryReservationRepository _reservations = new();
    private readonly InMemoryTimeslotRepository _timeslots = new();

    private ReservationService NewService(int maxActiveReservations = 2) =>
        new(TestData.Policy(_reservations, _timeslots, maxActiveReservations), _timeslots, _reservations);

    [Fact]
    public void WhenReservingAvailableTimeslot_ShouldCreatePendingReviewReservation()
    {
        var booker = TestData.Booker();
        var timeslot = TestData.StoredTimeslot(_timeslots);

        var reservation = NewService().Reserve(booker, timeslot.Id);

        reservation.BookerId.ShouldBe(booker.Id);
        reservation.PremisesId.ShouldBe(timeslot.PremisesId);
        reservation.TimeslotId.ShouldBe(timeslot.Id);
        reservation.Status.ShouldBe(ReservationStatus.PendingReview);
    }

    [Fact]
    public void WhenReservingAvailableTimeslot_ShouldMarkTimeslotReserved()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);

        NewService().Reserve(TestData.Booker(), timeslot.Id);

        timeslot.IsReserved.ShouldBeTrue();
    }

    [Fact]
    public void WhenReservingTwice_ShouldCreateReservationsWithUniqueIds()
    {
        var first = TestData.StoredTimeslot(_timeslots, "t1", 1);
        var second = TestData.StoredTimeslot(_timeslots, "t2", 3);
        var service = NewService();

        var r1 = service.Reserve(TestData.Booker(), first.Id);
        var r2 = service.Reserve(TestData.Booker(), second.Id);

        r1.Id.ShouldNotBe(r2.Id);
    }

    [Fact]
    public void WhenPolicyFails_ShouldThrowBookingPolicyViolationException()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);
        var service = NewService();
        service.Reserve(TestData.Booker("b1"), timeslot.Id);

        var exception = Should.Throw<BookingPolicyViolationException>(
            () => service.Reserve(TestData.Booker("b2"), timeslot.Id));

        exception.Violations.ShouldContain(PolicyViolation.TimeslotAlreadyReserved);
    }

    [Fact]
    public void WhenPolicyFails_ShouldNotReserveTimeslotOrStoreReservation()
    {
        var booker = TestData.Booker();
        var first = TestData.StoredTimeslot(_timeslots, "t1", 1);
        var second = TestData.StoredTimeslot(_timeslots, "t2", 3);
        var service = NewService(maxActiveReservations: 1);
        service.Reserve(booker, first.Id);

        Should.Throw<BookingPolicyViolationException>(() => service.Reserve(booker, second.Id));

        second.IsReserved.ShouldBeFalse();
        _reservations.All.Count.ShouldBe(1);
    }

    [Fact]
    public void WhenReservingUnknownTimeslot_ShouldThrowBookingPolicyViolationException()
    {
        var exception = Should.Throw<BookingPolicyViolationException>(
            () => NewService().Reserve(TestData.Booker(), new Id("missing")));

        exception.Violations.ShouldBe([PolicyViolation.TimeslotNotFound]);
    }
}
