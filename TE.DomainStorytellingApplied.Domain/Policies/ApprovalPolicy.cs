using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies;

// Domain policy
public sealed class ApprovalPolicy
{
    private readonly BookingPolicy _bookingPolicy;

    public ApprovalPolicy(BookingPolicy bookingPolicy)
    {
        ArgumentNullException.ThrowIfNull(bookingPolicy);

        _bookingPolicy = bookingPolicy;
    }

    public ApprovalRoute DetermineRoute(BookerType bookerType, Timeslot timeslot) =>
        _bookingPolicy.RequiresReview(bookerType, timeslot)
            ? ApprovalRoute.RouteForReview
            : ApprovalRoute.AutoApprove;
}
