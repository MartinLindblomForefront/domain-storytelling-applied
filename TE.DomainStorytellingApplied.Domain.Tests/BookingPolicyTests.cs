using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingPolicyTests
{
    private readonly InMemoryTimeslotRepository _timeslots = new();
    private readonly InMemoryReservationRepository _reservations = new();

    private BookingPolicy NewPolicy(int maxActiveReservations = 2, DateTimeOffset? now = null) =>
        TestData.Policy(_reservations, _timeslots, maxActiveReservations, now);

    [Fact]
    public void WhenAllRulesSatisfied_ShouldReportNoViolations()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);

        NewPolicy().Check(TestData.Booker(), timeslot.Id).ShouldBeEmpty();
    }

    [Fact]
    public void WhenTimeslotIsInPast_ShouldReportViolation()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);

        var violations = NewPolicy(now: timeslot.Start.AddMinutes(1)).Check(TestData.Booker(), timeslot.Id);

        violations.ShouldBe([PolicyViolation.TimeslotInPast]);
    }

    [Fact]
    public void WhenTimeslotAlreadyReserved_ShouldReportViolation()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);
        timeslot.Reserve();

        NewPolicy().Check(TestData.Booker(), timeslot.Id).ShouldBe([PolicyViolation.TimeslotAlreadyReserved]);
    }

    [Fact]
    public void WhenTimeslotDoesNotExist_ShouldReportViolation()
    {
        NewPolicy().Check(TestData.Booker(), new Id("missing")).ShouldBe([PolicyViolation.TimeslotNotFound]);
    }

    [Fact]
    public void WhenBookerHasMaxActiveReservations_ShouldReportViolation()
    {
        var booker = TestData.Booker();
        _reservations.Add(TestData.PendingReservation(booker));
        var timeslot = TestData.StoredTimeslot(_timeslots, "t2", hoursFromNow: 5);

        NewPolicy(maxActiveReservations: 1).Check(booker, timeslot.Id)
            .ShouldBe([PolicyViolation.MaxActiveReservationsReached]);
    }

    [Fact]
    public void WhenOtherBookerHasReservations_ShouldNotCountTowardsLimit()
    {
        _reservations.Add(TestData.PendingReservation(TestData.Booker("other")));
        var timeslot = TestData.StoredTimeslot(_timeslots, "t2", hoursFromNow: 5);

        NewPolicy(maxActiveReservations: 1).Check(TestData.Booker(), timeslot.Id).ShouldBeEmpty();
    }

    [Fact]
    public void WhenBookersPreviousReservationWasRejected_ShouldNotCountTowardsLimit()
    {
        var booker = TestData.Booker();
        var rejected = TestData.PendingReservation(booker);
        rejected.Reject(TestData.ApproverId);
        _reservations.Add(rejected);
        var timeslot = TestData.StoredTimeslot(_timeslots, "t2", hoursFromNow: 5);

        NewPolicy(maxActiveReservations: 1).Check(booker, timeslot.Id).ShouldBeEmpty();
    }

    [Fact]
    public void WhenSeveralRulesFail_ShouldReportAllViolations()
    {
        var booker = TestData.Booker();
        _reservations.Add(TestData.PendingReservation(booker));
        var timeslot = TestData.StoredTimeslot(_timeslots, "t2", hoursFromNow: 5);
        timeslot.Reserve();

        var violations = NewPolicy(maxActiveReservations: 1, now: timeslot.Start.AddMinutes(1))
            .Check(booker, timeslot.Id);

        violations.ShouldBe(
            [PolicyViolation.TimeslotInPast, PolicyViolation.TimeslotAlreadyReserved, PolicyViolation.MaxActiveReservationsReached],
            ignoreOrder: true);
    }

    [Fact]
    public void WhenEnforcingFailingPolicy_ShouldThrowBookingPolicyViolationException()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);
        timeslot.Reserve();

        var exception = Should.Throw<BookingPolicyViolationException>(
            () => NewPolicy().Enforce(TestData.Booker(), timeslot.Id));

        exception.Violations.ShouldBe([PolicyViolation.TimeslotAlreadyReserved]);
    }
}
