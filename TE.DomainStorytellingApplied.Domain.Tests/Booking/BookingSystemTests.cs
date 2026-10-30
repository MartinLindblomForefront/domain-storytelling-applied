using TE.DomainStorytellingApplied.Domain.Booking;
using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Reservations;

namespace TE.DomainStorytellingApplied.Domain.Tests.Booking;

public class BookingSystemTests
{
    private static readonly DateTime Start = new(2026, 10, 9, 10, 0, 0);
    private static readonly DateTime End = new(2026, 10, 9, 12, 0, 0);

    private readonly BookingSystem _system = new();

    private Booker AddDefaultBooker() =>
        _system.AddBooker("Anna", "Andersson", "070-1234567", "anna@example.com", new PrivateIndividual());

    [Fact]
    public void GivenValidName_WhenAddingPremise_ShouldBeStored()
    {
        var premise = _system.AddPremise("Gymnastiksalen");

        _system.Premises.ShouldContain(premise);
    }

    [Fact]
    public void GivenValidValues_WhenAddingBooker_ShouldBeStored()
    {
        var booker = AddDefaultBooker();

        _system.Bookers.ShouldContain(booker);
    }

    [Fact]
    public void GivenExistingPremise_WhenAddingTimeslot_ShouldBeStored()
    {
        var premise = _system.AddPremise("Gymnastiksalen");

        var timeslot = _system.AddTimeslot(premise.Id, Start, End);

        _system.Timeslots.ShouldContain(timeslot);
        timeslot.PremisesId.ShouldBe(premise.Id);
    }

    [Fact]
    public void GivenUnknownPremise_WhenAddingTimeslot_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => _system.AddTimeslot(Guid.NewGuid(), Start, End));
    }

    [Fact]
    public void GivenEmptyPremiseName_WhenAddingPremise_ShouldThrowAndNotStore()
    {
        Should.Throw<ArgumentException>(() => _system.AddPremise(""));

        _system.Premises.ShouldBeEmpty();
    }

    [Fact]
    public void GivenInvalidTimeslot_WhenAddingTimeslot_ShouldThrowAndNotStore()
    {
        var premise = _system.AddPremise("Gymnastiksalen");

        Should.Throw<ArgumentException>(() => _system.AddTimeslot(premise.Id, End, Start));

        _system.Timeslots.ShouldBeEmpty();
    }

    [Fact]
    public void GivenExistingBookerAndTimeslot_WhenReserving_ShouldCreatePendingReservation()
    {
        var premise = _system.AddPremise("Gymnastiksalen");
        var timeslot = _system.AddTimeslot(premise.Id, Start, End);
        var booker = AddDefaultBooker();

        var reservation = _system.Reserve(booker.Id, timeslot.Id);

        reservation.Status.ShouldBe(ReservationStatus.Pending);
        reservation.Booker.ShouldBe(booker);
        reservation.Timeslot.ShouldBe(timeslot);
        reservation.Premise.ShouldBe(premise);
        _system.Reservations.ShouldContain(reservation);
    }

    [Fact]
    public void GivenUnknownBooker_WhenReserving_ShouldThrow()
    {
        var premise = _system.AddPremise("Gymnastiksalen");
        var timeslot = _system.AddTimeslot(premise.Id, Start, End);

        Should.Throw<ArgumentException>(() => _system.Reserve(Guid.NewGuid(), timeslot.Id));
    }

    [Fact]
    public void GivenUnknownTimeslot_WhenReserving_ShouldThrow()
    {
        var booker = AddDefaultBooker();

        Should.Throw<ArgumentException>(() => _system.Reserve(booker.Id, Guid.NewGuid()));
    }
}
