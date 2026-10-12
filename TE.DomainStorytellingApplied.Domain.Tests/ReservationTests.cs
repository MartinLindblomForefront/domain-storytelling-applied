namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationTests
{
    [Fact]
    public void WhenCreatingReservation_ShouldBePendingReviewAndActive()
    {
        var timeslot = TestData.Timeslot();

        var reservation = new Reservation(new Id("r1"), new Id("b1"), timeslot.PremisesId, timeslot.Id);

        reservation.Id.ShouldBe(new Id("r1"));
        reservation.BookerId.ShouldBe(new Id("b1"));
        reservation.PremisesId.ShouldBe(timeslot.PremisesId);
        reservation.TimeslotId.ShouldBe(timeslot.Id);
        reservation.Status.ShouldBe(ReservationStatus.PendingReview);
        reservation.ReviewedBy.ShouldBeNull();
        reservation.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void WhenApproverApproves_ShouldBeApprovedAndRecordReviewer()
    {
        var reservation = TestData.PendingReservation();

        reservation.Approve(TestData.ApproverId);

        reservation.Status.ShouldBe(ReservationStatus.Approved);
        reservation.ReviewedBy.ShouldBe(TestData.ApproverId);
        reservation.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void WhenApproverRejects_ShouldBeRejectedAndRecordReviewer()
    {
        var reservation = TestData.PendingReservation();

        reservation.Reject(TestData.ApproverId);

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
        reservation.ReviewedBy.ShouldBe(TestData.ApproverId);
        reservation.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void WhenAnyApproverReviews_ShouldBeAllowed()
    {
        var reservation = TestData.PendingReservation();

        reservation.Approve(new Id("any-approver"));

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void WhenReviewingTwice_ShouldThrowException()
    {
        var reservation = TestData.PendingReservation();
        reservation.Approve(TestData.ApproverId);

        Should.Throw<InvalidOperationException>(() => reservation.Reject(new Id("a2")));
        reservation.Status.ShouldBe(ReservationStatus.Approved);
        reservation.ReviewedBy.ShouldBe(TestData.ApproverId);
    }
}
