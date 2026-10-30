namespace TE.DomainStorytellingApplied.Domain.Tests.Policies;

using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Timeslots;

public class ApprovalPolicyTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    private readonly ApprovalPolicy _policy = new(new BookingPolicy());

    private static Timeslot TimeslotOf(int minutes) => new(Start, Start.AddMinutes(minutes));

    [Fact]
    public void GivenBookingPolicyRequiresNoReview_WhenDeterminingRoute_ShouldAutoApprove()
    {
        var route = _policy.DetermineRoute(BookerType.MunicipalityEmployee, TimeslotOf(60));

        route.ShouldBe(ApprovalRoute.AutoApprove);
    }

    [Fact]
    public void GivenBookingPolicyRequiresReview_WhenDeterminingRoute_ShouldRouteForReview()
    {
        var route = _policy.DetermineRoute(BookerType.PrivateIndividual, TimeslotOf(60));

        route.ShouldBe(ApprovalRoute.RouteForReview);
    }

    [Fact]
    public void GivenAssociationRepresentative_WhenTimeslotCrossesFourHours_ShouldSwitchFromAutoApproveToReview()
    {
        _policy.DetermineRoute(BookerType.AssociationRepresentative, TimeslotOf(240))
            .ShouldBe(ApprovalRoute.AutoApprove);
        _policy.DetermineRoute(BookerType.AssociationRepresentative, TimeslotOf(241))
            .ShouldBe(ApprovalRoute.RouteForReview);
    }

    [Fact]
    public void GivenNullBookingPolicy_WhenCreatingApprovalPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new ApprovalPolicy(null!));
    }

    [Fact]
    public void GivenStoryOutcomes_WhenListingRoutes_ShouldContainExactlyAutoApproveAndRouteForReview()
    {
        Enum.GetValues<ApprovalRoute>().ShouldBe(
            [ApprovalRoute.AutoApprove, ApprovalRoute.RouteForReview],
            ignoreOrder: true);
    }
}
