using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Reservations.Rules;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations.Rules;

public class ReservationRulesTests
{
    private static Reservation NewReservation() =>
        Reservation.Request(new BookerId(Guid.NewGuid()), new PremisesId(Guid.NewGuid()), [TimeslotId.New()]);

    [Test]
    public void A_reservation_without_timeslots_breaks_the_rule()
    {
        // Arrange
        var rule = new ReservationMustHaveTimeslots([]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Reservation));
    }

    [Test]
    public void A_reservation_with_a_timeslot_follows_the_rule()
    {
        // Arrange
        var rule = new ReservationMustHaveTimeslots([TimeslotId.New()]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void A_rejection_without_a_reason_breaks_the_rule(string? reason)
    {
        // Arrange
        var rule = new RejectionMustHaveReason(reason);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Reservation));
    }

    [Test]
    public void A_rejection_with_a_reason_follows_the_rule()
    {
        // Arrange
        var rule = new RejectionMustHaveReason("Premises closed");

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_reservation_in_a_disallowed_status_breaks_the_status_rule()
    {
        // Arrange
        var reservation = NewReservation();
        reservation.Approve();
        var rule = new ReservationMustBeInStatus(
            reservation, [ReservationStatus.Requested], ReservationStatus.Rejected);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("Booked").And.Contain("Rejected");
        rule.Domain.Should().Be(nameof(Reservation));
    }

    [Test]
    public void A_reservation_in_an_allowed_status_follows_the_status_rule()
    {
        // Arrange
        var reservation = NewReservation();
        var rule = new ReservationMustBeInStatus(
            reservation, [ReservationStatus.Requested], ReservationStatus.Booked);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }
}
