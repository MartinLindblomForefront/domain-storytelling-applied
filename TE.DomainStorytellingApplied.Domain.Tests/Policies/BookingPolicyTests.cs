namespace TE.DomainStorytellingApplied.Domain.Tests.Policies;

using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Timeslots;

public class BookingPolicyTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    private readonly BookingPolicy _policy = new();

    private static Timeslot TimeslotOf(int minutes) => new(Start, Start.AddMinutes(minutes));

    [Theory]
    [InlineData(30)]
    [InlineData(240)]
    [InlineData(600)]
    public void GivenMunicipalityEmployee_WhenCheckingAnyTimeslot_ShouldNotRequireReview(int minutes)
    {
        _policy.RequiresReview(BookerType.MunicipalityEmployee, TimeslotOf(minutes)).ShouldBeFalse();
    }

    [Theory]
    [InlineData(30)]
    [InlineData(240)]
    public void GivenAssociationRepresentative_WhenTimeslotIsAtMostFourHours_ShouldNotRequireReview(int minutes)
    {
        _policy.RequiresReview(BookerType.AssociationRepresentative, TimeslotOf(minutes)).ShouldBeFalse();
    }

    [Theory]
    [InlineData(241)]
    [InlineData(600)]
    public void GivenAssociationRepresentative_WhenTimeslotIsLongerThanFourHours_ShouldRequireReview(int minutes)
    {
        _policy.RequiresReview(BookerType.AssociationRepresentative, TimeslotOf(minutes)).ShouldBeTrue();
    }

    [Theory]
    [InlineData(30)]
    [InlineData(240)]
    [InlineData(600)]
    public void GivenPrivateIndividual_WhenCheckingAnyTimeslot_ShouldRequireReview(int minutes)
    {
        _policy.RequiresReview(BookerType.PrivateIndividual, TimeslotOf(minutes)).ShouldBeTrue();
    }

    [Fact]
    public void GivenUndefinedBookerType_WhenCheckingTimeslot_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(
            () => _policy.RequiresReview((BookerType)999, TimeslotOf(60)));
    }

    [Fact]
    public void GivenNullTimeslot_WhenCheckingTimeslot_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(
            () => _policy.RequiresReview(BookerType.MunicipalityEmployee, null!));
    }
}
