using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Timeslots;
using TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

namespace TE.DomainStorytellingApplied.Domain.Tests.Timeslots.Rules;

public class TimeslotRulesTests
{
    private static readonly DateTime Start = new(2026, 11, 2, 17, 0, 0, DateTimeKind.Utc);

    private static Timeslot NewTimeslot() =>
        Timeslot.CreateAvailable(new PremisesId(Guid.NewGuid()), new Period(Start, Start.AddHours(1)));

    private static Timeslot ReservedTimeslot(ReservationId reservationId)
    {
        var timeslot = NewTimeslot();
        timeslot.Reserve(reservationId);
        return timeslot;
    }

    [Test]
    public void A_reserved_timeslot_breaks_the_availability_rule()
    {
        // Arrange
        var rule = new TimeslotMustBeAvailable(ReservedTimeslot(ReservationId.New()));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Timeslot));
    }

    [Test]
    public void An_available_timeslot_follows_the_availability_rule()
    {
        // Arrange
        var rule = new TimeslotMustBeAvailable(NewTimeslot());

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_timeslot_reserved_by_another_reservation_breaks_the_ownership_rule()
    {
        // Arrange
        var rule = new TimeslotMustBeReservedByReservation(ReservedTimeslot(ReservationId.New()), ReservationId.New());

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Timeslot));
    }

    [Test]
    public void An_available_timeslot_breaks_the_ownership_rule()
    {
        // Arrange
        var rule = new TimeslotMustBeReservedByReservation(NewTimeslot(), ReservationId.New());

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
    }

    [Test]
    public void A_timeslot_reserved_by_the_same_reservation_follows_the_ownership_rule()
    {
        // Arrange
        var reservationId = ReservationId.New();
        var rule = new TimeslotMustBeReservedByReservation(ReservedTimeslot(reservationId), reservationId);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_reserved_timeslot_breaks_the_must_not_be_reserved_rule()
    {
        // Arrange
        var rule = new TimeslotMustNotBeReserved(ReservedTimeslot(ReservationId.New()));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Timeslot));
    }

    [Test]
    public void An_available_timeslot_follows_the_must_not_be_reserved_rule()
    {
        // Arrange
        var rule = new TimeslotMustNotBeReserved(NewTimeslot());

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_local_time_breaks_the_utc_rule()
    {
        // Arrange
        var local = new DateTime(2026, 11, 2, 17, 0, 0, DateTimeKind.Local);
        var rule = new PeriodMustBeInUtc(local, Start.AddHours(1));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Period));
    }

    [Test]
    public void Utc_times_follow_the_utc_rule()
    {
        // Arrange
        var rule = new PeriodMustBeInUtc(Start, Start.AddHours(1));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void A_period_that_ends_at_or_before_its_start_breaks_the_end_rule(int hoursAfterStart)
    {
        // Arrange
        var rule = new PeriodMustEndAfterItStarts(Start, Start.AddHours(hoursAfterStart));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(Period));
    }

    [Test]
    public void A_period_that_ends_after_its_start_follows_the_end_rule()
    {
        // Arrange
        var rule = new PeriodMustEndAfterItStarts(Start, Start.AddHours(1));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }
}
