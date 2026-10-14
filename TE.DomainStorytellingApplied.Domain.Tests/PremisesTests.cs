namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremisesTests
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 8, 0, 0, TimeSpan.Zero);

    private static readonly Booker Booker =
        new("Anna Andersson", "anna@example.com", BookerType.PrivateIndividual);

    private static readonly Booker AssociationBooker =
        new("Cecilia Carlsson", "cecilia@example.com", BookerType.AssociationRepresentative);

    private static readonly Approver Approver = new("Bertil Berg");

    private static BookingPolicy CreatePolicy(bool requiresReview = false) => new(
        [BookerType.PrivateIndividual],
        requiresReview,
        minDuration: TimeSpan.FromHours(1),
        maxDuration: TimeSpan.FromHours(4),
        opensAt: new TimeOnly(8, 0),
        closesAt: new TimeOnly(22, 0));

    private static Premises CreatePremises(bool requiresReview = false) =>
        new("Stora salen", CreatePolicy(requiresReview));

    private static Timeslot Slot(int startHour, int endHour) =>
        new(new DateTimeOffset(2030, 1, 2, startHour, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2030, 1, 2, endHour, 0, 0, TimeSpan.Zero));

    [Fact]
    public void GivenValidValues_WhenCreatingPremises_ShouldSetProperties()
    {
        var policy = CreatePolicy();

        var premises = new Premises("Stora salen", policy);

        premises.Id.ShouldNotBe(Guid.Empty);
        premises.Name.ShouldBe("Stora salen");
        premises.BookingPolicy.ShouldBe(policy);
        premises.Reservations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyName_WhenCreatingPremises_ShouldThrow(string name)
    {
        Should.Throw<ArgumentException>(() => new Premises(name, CreatePolicy()));
    }

    [Fact]
    public void GivenNoBookingPolicy_WhenCreatingPremises_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new Premises("Stora salen", null!));
    }

    [Fact]
    public void GivenNoBooker_WhenReserving_ShouldThrow()
    {
        var premises = CreatePremises();

        Should.Throw<ArgumentNullException>(() => premises.Reserve(null!, Slot(10, 12), Now));
    }

    [Fact]
    public void GivenNoTimeslot_WhenReserving_ShouldThrow()
    {
        var premises = CreatePremises();

        Should.Throw<ArgumentNullException>(() => premises.Reserve(Booker, null!, Now));
    }

    [Fact]
    public void GivenPolicyWithoutReview_WhenReserving_ShouldCreateApprovedReservation()
    {
        var premises = CreatePremises(requiresReview: false);

        var reservation = premises.Reserve(Booker, Slot(10, 12), Now);

        reservation.BookerId.ShouldBe(Booker.Id);
        reservation.PremisesId.ShouldBe(premises.Id);
        reservation.Timeslot.ShouldBe(Slot(10, 12));
        reservation.Status.ShouldBe(ReservationStatus.Approved);
        premises.Reservations.ShouldContain(reservation);
    }

    [Fact]
    public void GivenPolicyWithReview_WhenReserving_ShouldBePendingReview()
    {
        var premises = CreatePremises(requiresReview: true);

        var reservation = premises.Reserve(Booker, Slot(10, 12), Now);

        reservation.Status.ShouldBe(ReservationStatus.PendingReview);
    }

    [Fact]
    public void GivenPolicyDeniesBooking_WhenReserving_ShouldThrowAndNotAddReservation()
    {
        var premises = CreatePremises();

        Should.Throw<InvalidOperationException>(() => premises.Reserve(AssociationBooker, Slot(10, 12), Now));
        premises.Reservations.ShouldBeEmpty();
    }

    [Fact]
    public void GivenExistingReservation_WhenReservingOverlappingTimeslot_ShouldThrow()
    {
        var premises = CreatePremises();
        premises.Reserve(Booker, Slot(10, 12), Now);

        Should.Throw<InvalidOperationException>(() => premises.Reserve(Booker, Slot(11, 13), Now));
    }

    [Fact]
    public void GivenPendingReservation_WhenReservingOverlappingTimeslot_ShouldThrow()
    {
        var premises = CreatePremises(requiresReview: true);
        premises.Reserve(Booker, Slot(10, 12), Now);

        Should.Throw<InvalidOperationException>(() => premises.Reserve(Booker, Slot(11, 13), Now));
    }

    [Fact]
    public void GivenExistingReservation_WhenReservingAdjacentTimeslot_ShouldCreateReservation()
    {
        var premises = CreatePremises();
        premises.Reserve(Booker, Slot(10, 12), Now);

        var reservation = premises.Reserve(Booker, Slot(12, 14), Now);

        premises.Reservations.Count.ShouldBe(2);
        premises.Reservations.ShouldContain(reservation);
    }

    [Fact]
    public void GivenRejectedReservation_WhenReservingOverlappingTimeslot_ShouldCreateReservation()
    {
        var premises = CreatePremises(requiresReview: true);
        var rejected = premises.Reserve(Booker, Slot(10, 12), Now);
        premises.Reject(rejected.Id, Approver, "Lokalen renoveras");

        var reservation = premises.Reserve(Booker, Slot(10, 12), Now);

        reservation.Status.ShouldBe(ReservationStatus.PendingReview);
    }

    [Fact]
    public void GivenPendingReservation_WhenApproving_ShouldBeApproved()
    {
        var premises = CreatePremises(requiresReview: true);
        var reservation = premises.Reserve(Booker, Slot(10, 12), Now);

        premises.Approve(reservation.Id, Approver);

        reservation.Status.ShouldBe(ReservationStatus.Approved);
        reservation.ReviewedById.ShouldBe(Approver.Id);
    }

    [Fact]
    public void GivenPendingReservation_WhenRejecting_ShouldBeRejectedWithReason()
    {
        var premises = CreatePremises(requiresReview: true);
        var reservation = premises.Reserve(Booker, Slot(10, 12), Now);

        premises.Reject(reservation.Id, Approver, "Lokalen renoveras");

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
        reservation.ReviewedById.ShouldBe(Approver.Id);
        reservation.RejectionReason.ShouldBe("Lokalen renoveras");
    }

    [Fact]
    public void GivenUnknownReservationId_WhenApproving_ShouldThrow()
    {
        var premises = CreatePremises(requiresReview: true);

        Should.Throw<ArgumentException>(() => premises.Approve(Guid.NewGuid(), Approver));
    }

    [Fact]
    public void GivenUnknownReservationId_WhenRejecting_ShouldThrow()
    {
        var premises = CreatePremises(requiresReview: true);

        Should.Throw<ArgumentException>(() => premises.Reject(Guid.NewGuid(), Approver, "Lokalen renoveras"));
    }
}
