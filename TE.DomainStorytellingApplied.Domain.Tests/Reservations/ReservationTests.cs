using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations;

public class ReservationTests
{
    private readonly Booker _booker = new("Anna", "Andersson", "070-1234567", "anna@example.com", new PrivateIndividual());
    private readonly Premise _premise = new("Gymnastiksalen");
    private readonly Timeslot _timeslot;

    public ReservationTests()
    {
        _timeslot = new Timeslot(_premise.Id, new DateTime(2026, 10, 9, 10, 0, 0), new DateTime(2026, 10, 9, 12, 0, 0));
    }

    [Fact]
    public void GivenValidInput_WhenCreatingReservation_ShouldBePending()
    {
        var reservation = new Reservation(_booker, _timeslot, _premise);

        reservation.Status.ShouldBe(ReservationStatus.Pending);
    }

    [Fact]
    public void GivenValidInput_WhenCreatingReservation_ShouldReferenceBookerTimeslotAndPremise()
    {
        var reservation = new Reservation(_booker, _timeslot, _premise);

        reservation.Id.ShouldNotBe(Guid.Empty);
        reservation.Booker.ShouldBe(_booker);
        reservation.Timeslot.ShouldBe(_timeslot);
        reservation.Premise.ShouldBe(_premise);
    }

    [Fact]
    public void GivenTwoReservations_WhenCreated_ShouldHaveUniqueIds()
    {
        var first = new Reservation(_booker, _timeslot, _premise);
        var second = new Reservation(_booker, _timeslot, _premise);

        first.Id.ShouldNotBe(second.Id);
    }

    [Fact]
    public void GivenNullBooker_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new Reservation(null!, _timeslot, _premise));
    }

    [Fact]
    public void GivenNullTimeslot_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new Reservation(_booker, null!, _premise));
    }

    [Fact]
    public void GivenNullPremise_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new Reservation(_booker, _timeslot, null!));
    }

    [Fact]
    public void GivenTimeslotForAnotherPremise_WhenCreatingReservation_ShouldThrow()
    {
        var otherPremise = new Premise("Aulan");

        Should.Throw<ArgumentException>(() => new Reservation(_booker, _timeslot, otherPremise));
    }
}
