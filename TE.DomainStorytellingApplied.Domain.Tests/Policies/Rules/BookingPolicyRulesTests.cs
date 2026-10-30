using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Policies.Rules;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Policies.Rules;

public class BookingPolicyRulesTests
{
    private static readonly DateTime Now = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan OneYear = TimeSpan.FromDays(365);

    private static Timeslot Slot(DateTime startUtc, int hours = 1, PremisesId? premises = null) =>
        Timeslot.CreateAvailable(
            premises ?? new PremisesId(Guid.NewGuid()), new Period(startUtc, startUtc.AddHours(hours)));

    [Test]
    public void No_timeslots_breaks_the_not_empty_rule()
    {
        // Arrange
        var rule = new TimeslotsMustNotBeEmpty([]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(BookingPolicy));
    }

    [Test]
    public void A_timeslot_follows_the_not_empty_rule()
    {
        // Arrange
        var rule = new TimeslotsMustNotBeEmpty([Slot(Now.AddDays(1))]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void Timeslots_in_different_premises_break_the_same_premises_rule()
    {
        // Arrange
        var rule = new TimeslotsMustBeInSamePremises([Slot(Now.AddDays(1)), Slot(Now.AddDays(2))]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(BookingPolicy));
    }

    [Test]
    public void Timeslots_in_the_same_premises_follow_the_same_premises_rule()
    {
        // Arrange
        var premises = new PremisesId(Guid.NewGuid());
        var rule = new TimeslotsMustBeInSamePremises([Slot(Now.AddDays(1), 1, premises), Slot(Now.AddDays(2), 1, premises)]);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_total_duration_below_the_minimum_breaks_the_minimum_duration_rule()
    {
        // Arrange
        var rule = new TotalDurationMustBeAtLeast(TimeSpan.FromMinutes(30), TimeSpan.FromHours(1));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(BookingPolicy));
    }

    [Test]
    public void A_total_duration_equal_to_the_minimum_follows_the_minimum_duration_rule()
    {
        // Arrange
        var rule = new TotalDurationMustBeAtLeast(TimeSpan.FromHours(1), TimeSpan.FromHours(1));

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_timeslot_starting_before_now_breaks_the_not_in_the_past_rule()
    {
        // Arrange
        var rule = new ReservationMustNotStartInThePast([Slot(Now.AddMinutes(-1))], Now);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(BookingPolicy));
    }

    [Test]
    public void A_timeslot_starting_exactly_now_follows_the_not_in_the_past_rule()
    {
        // Arrange
        var rule = new ReservationMustNotStartInThePast([Slot(Now)], Now);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }

    [Test]
    public void A_timeslot_starting_after_the_maximum_advance_breaks_the_too_far_ahead_rule()
    {
        // Arrange
        var rule = new ReservationMustNotBeTooFarAhead([Slot(Now + OneYear.Add(TimeSpan.FromMinutes(1)))], Now, OneYear);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeTrue();
        rule.Domain.Should().Be(nameof(BookingPolicy));
    }

    [Test]
    public void A_timeslot_starting_exactly_at_the_maximum_advance_follows_the_too_far_ahead_rule()
    {
        // Arrange
        var rule = new ReservationMustNotBeTooFarAhead([Slot(Now + OneYear)], Now, OneYear);

        // Act
        var isBroken = rule.RuleIsBroken;

        // Assert
        isBroken.Should().BeFalse();
    }
}
