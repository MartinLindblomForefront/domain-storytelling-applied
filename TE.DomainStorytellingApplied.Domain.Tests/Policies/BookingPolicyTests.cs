using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Policies;

public class BookingPolicyTests
{
    private static readonly DateTime Now = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly PremisesId Premises = new(Guid.NewGuid());
    private readonly BookingPolicy _policy = new(TimeSpan.FromHours(4));

    private static Timeslot Slot(DateTime startUtc, int hours, PremisesId? premises = null) =>
        Timeslot.CreateAvailable(premises ?? Premises, new Period(startUtc, startUtc.AddHours(hours)));

    [Test]
    public void A_short_reservation_of_available_timeslots_passes_the_policy_without_review()
    {
        // Arrange
        var timeslots = new[] { Slot(Now.AddDays(10), 2) };

        // Act
        _policy.Check(timeslots, Now);

        // Assert
        _policy.RequiresReview(timeslots).Should().BeFalse();
    }

    [Test]
    public void A_reservation_longer_than_the_threshold_requires_review()
    {
        // Arrange
        var timeslots = new[] { Slot(Now.AddDays(10), 5) };

        // Act
        _policy.Check(timeslots, Now);

        // Assert
        _policy.RequiresReview(timeslots).Should().BeTrue();
    }

    [Test]
    public void The_duration_of_all_timeslots_together_decides_if_review_is_required()
    {
        // Arrange
        var timeslots = new[] { Slot(Now.AddDays(10), 3), Slot(Now.AddDays(11), 3) };

        // Act
        var requiresReview = _policy.RequiresReview(timeslots);

        // Assert
        requiresReview.Should().BeTrue();
    }

    [Test]
    public void A_reservation_shorter_than_one_hour_is_not_allowed()
    {
        // Arrange
        var start = Now.AddDays(10);
        var timeslots = new[] { Timeslot.CreateAvailable(Premises, new Period(start, start.AddMinutes(30))) };

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_reservation_more_than_one_year_ahead_is_not_allowed()
    {
        // Arrange
        var timeslots = new[] { Slot(Now.AddDays(366), 2) };

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_reservation_starting_in_the_past_is_not_allowed()
    {
        // Arrange
        var timeslots = new[] { Slot(Now.AddHours(-2), 2) };

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_reservation_with_an_unavailable_timeslot_is_not_allowed()
    {
        // Arrange
        var reserved = Slot(Now.AddDays(10), 2);
        reserved.Reserve(ReservationId.New());

        // Act
        var act = () => _policy.Check([reserved], Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_reservation_spanning_different_premises_is_not_allowed()
    {
        // Arrange
        var timeslots = new[]
        {
            Slot(Now.AddDays(10), 1),
            Slot(Now.AddDays(10).AddHours(1), 1, new PremisesId(Guid.NewGuid()))
        };

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_reservation_without_timeslots_is_not_allowed()
    {
        // Arrange
        var timeslots = Array.Empty<Timeslot>();

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Test]
    public void A_broken_rule_tells_which_domain_it_belongs_to()
    {
        // Arrange
        var timeslots = Array.Empty<Timeslot>();

        // Act
        var act = () => _policy.Check(timeslots, Now);

        // Assert
        act.Should().Throw<DomainValidationException>().Which.Domain.Should().Be(nameof(BookingPolicy));
    }
}
