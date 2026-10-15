using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationNotifierTests
{
    private readonly FakeNotificationSender _sender = new();

    [Fact]
    public void WhenReservationApproved_ShouldSendConfirmationToBooker()
    {
        var reservation = TestData.PendingReservation();
        reservation.Approve(TestData.ApproverId);

        new ReservationNotifier(_sender).Notify(reservation);

        _sender.Confirmations.ShouldHaveSingleItem().ShouldBe((reservation.BookerId, reservation.Id));
        _sender.Rejections.ShouldBeEmpty();
    }

    [Fact]
    public void WhenReservationRejected_ShouldSendRejectionToBooker()
    {
        var reservation = TestData.PendingReservation();
        reservation.Reject(TestData.ApproverId);

        new ReservationNotifier(_sender).Notify(reservation);

        _sender.Rejections.ShouldHaveSingleItem().ShouldBe((reservation.BookerId, reservation.Id));
        _sender.Confirmations.ShouldBeEmpty();
    }

    [Fact]
    public void WhenReservationPendingReview_ShouldSendNothing()
    {
        new ReservationNotifier(_sender).Notify(TestData.PendingReservation());

        _sender.Confirmations.ShouldBeEmpty();
        _sender.Rejections.ShouldBeEmpty();
    }
}
