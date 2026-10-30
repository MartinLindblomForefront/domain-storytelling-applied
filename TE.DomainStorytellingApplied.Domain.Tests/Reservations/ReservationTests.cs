namespace TE.DomainStorytellingApplied.Domain.Tests.Reservations;

using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Timeslots;

public class ReservationTests
{
    private static readonly Guid SomeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Premise SomePremise = new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Conference Room A");
    private static readonly DateTimeOffset Start = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
    private static readonly Timeslot ShortTimeslot = new(Start, Start.AddHours(1));
    private static readonly ApprovalPolicy Policy = new(new BookingPolicy());

    private static Reservation CreateReservation(BookerType bookerType = BookerType.MunicipalityEmployee) =>
        Reservation.Create(SomeId, bookerType, SomePremise, ShortTimeslot);

    private static Reservation InStatus(ReservationStatus status)
    {
        switch (status)
        {
            case ReservationStatus.Requested:
                return CreateReservation();
            case ReservationStatus.Approved:
                var approved = CreateReservation(BookerType.MunicipalityEmployee);
                approved.Evaluate(Policy);
                return approved;
            case ReservationStatus.PendingReview:
                var pending = CreateReservation(BookerType.PrivateIndividual);
                pending.Evaluate(Policy);
                return pending;
            case ReservationStatus.Rejected:
                var rejected = CreateReservation(BookerType.PrivateIndividual);
                rejected.Evaluate(Policy);
                rejected.Reject();
                return rejected;
            default:
                throw new ArgumentOutOfRangeException(nameof(status));
        }
    }

    [Fact]
    public void GivenValidInput_WhenCreatingReservation_ShouldExposeValuesAndBeRequested()
    {
        var reservation = Reservation.Create(SomeId, BookerType.PrivateIndividual, SomePremise, ShortTimeslot);

        reservation.Id.ShouldBe(SomeId);
        reservation.BookerType.ShouldBe(BookerType.PrivateIndividual);
        reservation.Premise.ShouldBe(SomePremise);
        reservation.Timeslot.ShouldBe(ShortTimeslot);
        reservation.Status.ShouldBe(ReservationStatus.Requested);
    }

    [Fact]
    public void GivenEmptyId_WhenCreatingReservation_ShouldThrowInvalidReservationException()
    {
        var exception = Should.Throw<InvalidReservationException>(
            () => Reservation.Create(Guid.Empty, BookerType.PrivateIndividual, SomePremise, ShortTimeslot));

        exception.ShouldBeAssignableTo<DomainException>();
    }

    [Fact]
    public void GivenNullPremise_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(
            () => Reservation.Create(SomeId, BookerType.PrivateIndividual, null!, ShortTimeslot));
    }

    [Fact]
    public void GivenNullTimeslot_WhenCreatingReservation_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(
            () => Reservation.Create(SomeId, BookerType.PrivateIndividual, SomePremise, null!));
    }

    [Fact]
    public void GivenPolicyRequiresNoReview_WhenEvaluating_ShouldApproveReservation()
    {
        var reservation = CreateReservation(BookerType.MunicipalityEmployee);

        reservation.Evaluate(Policy);

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenPolicyRequiresReview_WhenEvaluating_ShouldRouteReservationForReview()
    {
        var reservation = CreateReservation(BookerType.PrivateIndividual);

        reservation.Evaluate(Policy);

        reservation.Status.ShouldBe(ReservationStatus.PendingReview);
    }

    [Fact]
    public void GivenNullPolicy_WhenEvaluating_ShouldThrow()
    {
        var reservation = CreateReservation();

        Should.Throw<ArgumentNullException>(() => reservation.Evaluate(null!));
    }

    [Theory]
    [InlineData(ReservationStatus.Approved)]
    [InlineData(ReservationStatus.PendingReview)]
    [InlineData(ReservationStatus.Rejected)]
    public void GivenReservationAlreadyEvaluated_WhenEvaluating_ShouldThrowInvalidReservationTransitionException(
        ReservationStatus status)
    {
        var reservation = InStatus(status);

        Should.Throw<InvalidReservationTransitionException>(() => reservation.Evaluate(Policy));
        reservation.Status.ShouldBe(status);
    }

    [Fact]
    public void GivenPendingReview_WhenApproverApproves_ShouldBeApproved()
    {
        var reservation = InStatus(ReservationStatus.PendingReview);

        reservation.Approve();

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenPendingReview_WhenApproverRejects_ShouldBeRejected()
    {
        var reservation = InStatus(ReservationStatus.PendingReview);

        reservation.Reject();

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
    }

    [Theory]
    [InlineData(ReservationStatus.Requested)]
    [InlineData(ReservationStatus.Approved)]
    [InlineData(ReservationStatus.Rejected)]
    public void GivenReservationNotPendingReview_WhenApproving_ShouldThrowInvalidReservationTransitionException(
        ReservationStatus status)
    {
        var reservation = InStatus(status);

        var exception = Should.Throw<InvalidReservationTransitionException>(() => reservation.Approve());

        exception.ShouldBeAssignableTo<DomainException>();
        reservation.Status.ShouldBe(status);
    }

    [Theory]
    [InlineData(ReservationStatus.Requested)]
    [InlineData(ReservationStatus.Approved)]
    [InlineData(ReservationStatus.Rejected)]
    public void GivenReservationNotPendingReview_WhenRejecting_ShouldThrowInvalidReservationTransitionException(
        ReservationStatus status)
    {
        var reservation = InStatus(status);

        Should.Throw<InvalidReservationTransitionException>(() => reservation.Reject());
        reservation.Status.ShouldBe(status);
    }
}
