using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationReviewServiceTests
{
    private readonly FakeNotificationSender _sender = new();
    private readonly InMemoryTimeslotRepository _timeslots = new();

    private ReservationReviewService NewService() => new(new ReservationNotifier(_sender), _timeslots);

    private (Timeslot timeslot, Reservation reservation) NewPendingReservationWithReservedTimeslot()
    {
        var timeslot = TestData.StoredTimeslot(_timeslots);
        timeslot.Reserve();
        return (timeslot, TestData.PendingReservation(timeslot: timeslot));
    }

    [Fact]
    public void WhenApproved_ShouldBeApprovedAndKeepTimeslotReserved()
    {
        var (timeslot, reservation) = NewPendingReservationWithReservedTimeslot();

        NewService().Approve(reservation, TestData.Approver());

        reservation.Status.ShouldBe(ReservationStatus.Approved);
        timeslot.IsReserved.ShouldBeTrue();
    }

    [Fact]
    public void WhenApproved_ShouldSendConfirmationToBooker()
    {
        var (_, reservation) = NewPendingReservationWithReservedTimeslot();

        NewService().Approve(reservation, TestData.Approver());

        _sender.Confirmations.ShouldHaveSingleItem().ShouldBe((reservation.BookerId, reservation.Id));
    }

    [Fact]
    public void WhenRejected_ShouldBeRejectedAndReleaseTimeslot()
    {
        var (timeslot, reservation) = NewPendingReservationWithReservedTimeslot();

        NewService().Reject(reservation, TestData.Approver());

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
        timeslot.IsReserved.ShouldBeFalse();
    }

    [Fact]
    public void WhenRejected_ShouldSendRejectionToBooker()
    {
        var (_, reservation) = NewPendingReservationWithReservedTimeslot();

        NewService().Reject(reservation, TestData.Approver());

        _sender.Rejections.ShouldHaveSingleItem().ShouldBe((reservation.BookerId, reservation.Id));
    }

    [Fact]
    public void WhenReviewingAlreadyReviewedReservation_ShouldNotSendAnotherNotification()
    {
        var (timeslot, reservation) = NewPendingReservationWithReservedTimeslot();
        var service = NewService();
        service.Approve(reservation, TestData.Approver());

        Should.Throw<InvalidOperationException>(() => service.Reject(reservation, TestData.Approver()));

        _sender.Confirmations.Count.ShouldBe(1);
        _sender.Rejections.ShouldBeEmpty();
        timeslot.IsReserved.ShouldBeTrue();
    }
}
