using TE.DomainStorytellingApplied.Domain.Reservations;

namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations;

public class ReservationTests
{
    private static readonly Guid BookerId = Guid.NewGuid();
    private static readonly Guid TimeslotId = Guid.NewGuid();
    private static readonly Guid PremiseId = Guid.NewGuid();

    [Fact]
    public void GivenValidInput_WhenCreatingReservation_ShouldBePending()
    {
        var reservation = new Reservation(BookerId, TimeslotId, PremiseId);

        reservation.Status.ShouldBe(ReservationStatus.Pending);
    }

    [Fact]
    public void GivenValidInput_WhenCreatingReservation_ShouldReferenceBookerTimeslotAndPremiseById()
    {
        var reservation = new Reservation(BookerId, TimeslotId, PremiseId);

        reservation.Id.ShouldNotBe(Guid.Empty);
        reservation.BookerId.ShouldBe(BookerId);
        reservation.TimeslotId.ShouldBe(TimeslotId);
        reservation.PremiseId.ShouldBe(PremiseId);
    }

    [Fact]
    public void GivenTwoReservations_WhenCreated_ShouldHaveUniqueIds()
    {
        var first = new Reservation(BookerId, TimeslotId, PremiseId);
        var second = new Reservation(BookerId, TimeslotId, PremiseId);

        first.Id.ShouldNotBe(second.Id);
    }

    [Fact]
    public void GivenEmptyBookerId_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Reservation(Guid.Empty, TimeslotId, PremiseId));
    }

    [Fact]
    public void GivenEmptyTimeslotId_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Reservation(BookerId, Guid.Empty, PremiseId));
    }

    [Fact]
    public void GivenEmptyPremiseId_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Reservation(BookerId, TimeslotId, Guid.Empty));
    }
}
