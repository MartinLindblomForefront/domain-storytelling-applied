namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations;

using TE.DomainStorytellingApplied.Domain.Reservations;

public class ReservationStatusTests
{
    [Fact]
    public void GivenDefaultStatus_WhenReading_ShouldBeRequested()
    {
        default(ReservationStatus).ShouldBe(ReservationStatus.Requested);
    }

    [Fact]
    public void GivenStoryOutcomes_WhenListingStatuses_ShouldContainExactlyRequestedPendingReviewApprovedRejected()
    {
        Enum.GetValues<ReservationStatus>().ShouldBe(
            [
                ReservationStatus.Requested,
                ReservationStatus.PendingReview,
                ReservationStatus.Approved,
                ReservationStatus.Rejected,
            ],
            ignoreOrder: true);
    }
}
