using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations;

public class ReservationTests
{
    private static Reservation NewReservation() =>
        Reservation.Request(
            new BookerId(Guid.NewGuid()),
            new PremisesId(Guid.NewGuid()),
            [TimeslotId.New()]);

    [Test]
    public void A_new_reservation_is_requested()
    {
        // Arrange
        // Act
        var reservation = NewReservation();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Requested);
    }

    [Test]
    public void A_reservation_without_timeslots_cannot_be_requested()
    {
        // Arrange
        var noTimeslots = Array.Empty<TimeslotId>();

        // Act
        var request = () => Reservation.Request(new BookerId(Guid.NewGuid()), new PremisesId(Guid.NewGuid()), noTimeslots);

        // Assert
        request.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_requested_reservation_can_be_booked_directly()
    {
        // Arrange
        var reservation = NewReservation();

        // Act
        reservation.Approve();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Booked);
    }

    [Test]
    public void A_requested_reservation_can_be_sent_to_review_and_then_booked()
    {
        // Arrange
        var reservation = NewReservation();
        reservation.SendToReview();

        // Act
        reservation.Approve();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Booked);
    }

    [Test]
    public void A_rejected_reservation_keeps_the_reason()
    {
        // Arrange
        var reservation = NewReservation();
        reservation.SendToReview();

        // Act
        reservation.Reject("Premises is closed");

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Rejected);
        reservation.RejectionReason.Should().Be("Premises is closed");
    }

    [Test]
    public void A_rejection_without_a_reason_is_not_allowed()
    {
        // Arrange
        var reservation = NewReservation();

        // Act
        var act = () => reservation.Reject(" ");

        // Assert
        act.Should().Throw<DomainValidationException>();
        reservation.Status.Should().Be(ReservationStatus.Requested);
    }

    [Test]
    public void A_booked_reservation_cannot_be_rejected()
    {
        // Arrange
        var reservation = NewReservation();
        reservation.Approve();

        // Act
        var act = () => reservation.Reject("Too late");

        // Assert
        act.Should().Throw<DomainValidationException>();
        reservation.Status.Should().Be(ReservationStatus.Booked);
    }

    [Test]
    public void A_reservation_under_review_cannot_be_sent_to_review_again()
    {
        // Arrange
        var reservation = NewReservation();
        reservation.SendToReview();

        // Act
        var act = () => reservation.SendToReview();

        // Assert
        act.Should().Throw<DomainValidationException>();
    }
}
