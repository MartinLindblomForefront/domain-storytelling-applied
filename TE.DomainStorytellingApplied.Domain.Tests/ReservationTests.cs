namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationTests
{
    private static readonly Guid BookerId = Guid.NewGuid();

    private static readonly Guid PremisesId = Guid.NewGuid();

    private static readonly Approver Approver = new("Bertil Berg");

    private static readonly Timeslot Timeslot =
        new(new DateTimeOffset(2030, 1, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2030, 1, 2, 12, 0, 0, TimeSpan.Zero));

    private static Reservation CreateReservation() => new(BookerId, PremisesId, Timeslot);

    private static Reservation CreatePendingReservation()
    {
        var reservation = CreateReservation();
        reservation.Submit(requiresReview: true);
        return reservation;
    }

    [Fact]
    public void GivenValidValues_WhenCreatingReservation_ShouldHaveStatusCreated()
    {
        var reservation = CreateReservation();

        reservation.Id.ShouldNotBe(Guid.Empty);
        reservation.BookerId.ShouldBe(BookerId);
        reservation.PremisesId.ShouldBe(PremisesId);
        reservation.Timeslot.ShouldBe(Timeslot);
        reservation.Status.ShouldBe(ReservationStatus.Created);
        reservation.ReviewedById.ShouldBeNull();
        reservation.RejectionReason.ShouldBeNull();
    }

    [Fact]
    public void GivenEmptyBookerId_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Reservation(Guid.Empty, PremisesId, Timeslot));
    }

    [Fact]
    public void GivenEmptyPremisesId_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Reservation(BookerId, Guid.Empty, Timeslot));
    }

    [Fact]
    public void GivenNoTimeslot_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new Reservation(BookerId, PremisesId, null!));
    }

    [Fact]
    public void GivenSubmittedReservation_WhenSubmittingAgain_ShouldThrow()
    {
        var reservation = CreatePendingReservation();

        Should.Throw<InvalidOperationException>(() => reservation.Submit(requiresReview: true));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenPendingReview_WhenRejectingWithoutReason_ShouldThrow(string reason)
    {
        var reservation = CreatePendingReservation();

        Should.Throw<ArgumentException>(() => reservation.Reject(Approver, reason));
    }

    [Fact]
    public void GivenPendingReview_WhenApprovingWithoutApprover_ShouldThrow()
    {
        var reservation = CreatePendingReservation();

        Should.Throw<ArgumentNullException>(() => reservation.Approve(null!));
    }

    [Fact]
    public void GivenCreatedReservation_WhenApproving_ShouldThrow()
    {
        var reservation = CreateReservation();

        Should.Throw<InvalidOperationException>(() => reservation.Approve(Approver));
    }

    [Fact]
    public void GivenApprovedReservation_WhenReviewingAgain_ShouldThrow()
    {
        var reservation = CreatePendingReservation();
        reservation.Approve(Approver);

        Should.Throw<InvalidOperationException>(() => reservation.Approve(Approver));
        Should.Throw<InvalidOperationException>(() => reservation.Reject(Approver, "För sent"));
    }

    [Fact]
    public void GivenRejectedReservation_WhenReviewingAgain_ShouldThrow()
    {
        var reservation = CreatePendingReservation();
        reservation.Reject(Approver, "Lokalen renoveras");

        Should.Throw<InvalidOperationException>(() => reservation.Approve(Approver));
        Should.Throw<InvalidOperationException>(() => reservation.Reject(Approver, "Igen"));
    }
}
