namespace TE.DomainStorytellingApplied.Domain.Tests;

public class NotificationTests
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 8, 0, 0, TimeSpan.Zero);

    private static readonly Booker Booker =
        new("Anna Andersson", "anna@example.com", BookerType.PrivateIndividual);

    private static readonly Approver Approver = new("Bertil Berg");

    private static readonly Timeslot Timeslot =
        new(new DateTimeOffset(2030, 1, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2030, 1, 2, 12, 0, 0, TimeSpan.Zero));

    private static Premises CreatePremises(bool requiresReview) =>
        new("Stora salen", new BookingPolicy(
            [BookerType.PrivateIndividual],
            requiresReview,
            minDuration: TimeSpan.FromHours(1),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: new TimeOnly(8, 0),
            closesAt: new TimeOnly(22, 0)));

    [Fact]
    public void GivenAutomaticallyApprovedReservation_WhenCreatingNotification_ShouldBeConfirmationToBooker()
    {
        var reservation = CreatePremises(requiresReview: false).Reserve(Booker, Timeslot, Now);

        var notification = Notification.For(reservation);

        notification.Type.ShouldBe(NotificationType.Confirmation);
        notification.RecipientId.ShouldBe(Booker.Id);
        notification.ReservationId.ShouldBe(reservation.Id);
        notification.Message.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GivenRejectedReservation_WhenCreatingNotification_ShouldBeRejectionContainingReason()
    {
        var premises = CreatePremises(requiresReview: true);
        var reservation = premises.Reserve(Booker, Timeslot, Now);
        premises.Reject(reservation.Id, Approver, "Lokalen renoveras");

        var notification = Notification.For(reservation);

        notification.Type.ShouldBe(NotificationType.Rejection);
        notification.RecipientId.ShouldBe(Booker.Id);
        notification.ReservationId.ShouldBe(reservation.Id);
        notification.Message.ShouldContain("Lokalen renoveras");
    }

    [Fact]
    public void GivenPendingReviewReservation_WhenCreatingNotification_ShouldThrow()
    {
        var reservation = CreatePremises(requiresReview: true).Reserve(Booker, Timeslot, Now);

        Should.Throw<InvalidOperationException>(() => Notification.For(reservation));
    }

    [Fact]
    public void GivenCreatedReservation_WhenCreatingNotification_ShouldThrow()
    {
        var reservation = new Reservation(Booker.Id, Guid.NewGuid(), Timeslot);

        Should.Throw<InvalidOperationException>(() => Notification.For(reservation));
    }

    [Fact]
    public void GivenNoReservation_WhenCreatingNotification_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => Notification.For(null!));
    }
}
