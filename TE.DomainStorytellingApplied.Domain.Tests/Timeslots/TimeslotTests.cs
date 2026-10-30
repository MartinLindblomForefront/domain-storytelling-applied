using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Shared;

using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Timeslots;

public class TimeslotTests
{
    private static readonly PremisesId Premises = new(Guid.NewGuid());

    private static Timeslot NewTimeslot() =>
        Timeslot.CreateAvailable(
            Premises,
            new Period(
                new DateTime(2026, 11, 2, 17, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 11, 2, 18, 0, 0, DateTimeKind.Utc)));

    private static ReservationId NewReservationId() => new(Guid.NewGuid());

    [Test]
    public void A_new_timeslot_is_available_and_belongs_to_the_premises()
    {
        // Arrange
        // Act
        var timeslot = NewTimeslot();

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Available);
        timeslot.PremisesId.Should().Be(Premises);
        timeslot.ReservationId.Should().BeNull();
    }

    [Test]
    public void An_available_timeslot_can_be_reserved_by_a_booking()
    {
        // Arrange
        var timeslot = NewTimeslot();
        var reservationId = NewReservationId();

        // Act
        timeslot.Reserve(reservationId);

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Reserved);
        timeslot.ReservationId.Should().Be(reservationId);
    }

    [Test]
    public void A_reserved_timeslot_cannot_be_reserved_again()
    {
        // Arrange
        var timeslot = NewTimeslot();
        timeslot.Reserve(NewReservationId());

        // Act
        var act = () => timeslot.Reserve(NewReservationId());

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_blocked_timeslot_cannot_be_reserved()
    {
        // Arrange
        var timeslot = NewTimeslot();
        timeslot.Block();

        // Act
        var act = () => timeslot.Reserve(NewReservationId());

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_timeslot_is_available_again_when_its_booking_releases_it()
    {
        // Arrange
        var timeslot = NewTimeslot();
        var reservationId = NewReservationId();
        timeslot.Reserve(reservationId);

        // Act
        timeslot.Release(reservationId);

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Available);
        timeslot.ReservationId.Should().BeNull();
    }

    [Test]
    public void A_timeslot_cannot_be_released_by_another_booking()
    {
        // Arrange
        var timeslot = NewTimeslot();
        timeslot.Reserve(NewReservationId());

        // Act
        var act = () => timeslot.Release(NewReservationId());

        // Assert
        act.Should().Throw<DomainValidationException>();
        timeslot.Status.Should().Be(TimeslotStatus.Reserved);
    }

    [Test]
    public void A_reserved_timeslot_cannot_be_blocked_before_it_is_released()
    {
        // Arrange
        var timeslot = NewTimeslot();
        timeslot.Reserve(NewReservationId());

        // Act
        var act = () => timeslot.Block();

        // Assert
        act.Should().Throw<DomainValidationException>();
        timeslot.Status.Should().Be(TimeslotStatus.Reserved);
    }
}
