using TE.DomainStorytellingApplied.Domain.Approvers;
using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Reservations;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationTests
{
    private static readonly DateOnly RequestedOn = new(2026, 10, 8);
    private static readonly DateTimeOffset StartsAt = new(2026, 10, 10, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Request_WhenManualReviewIsRequired_CreatesPendingReservation()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(8));

        reservation.Status.ShouldBe(ReservationStatus.PendingApproval);
        reservation.ApproverId.ShouldBeNull();
    }

    [Fact]
    public void Request_WhenApprovalIsAutomatic_CreatesConfirmedReservation()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(1));

        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Approve_WhenPending_ConfirmsReservationAndRecordsApprover()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(8));
        var approverId = new ApproverId(10);

        reservation.Approve(approverId);

        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
        reservation.ApproverId.ShouldBe(approverId);
    }

    [Fact]
    public void Reject_WhenPending_RejectsReservationAndRecordsApprover()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(8));
        var approverId = new ApproverId(10);

        reservation.Reject(approverId);

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
        reservation.ApproverId.ShouldBe(approverId);
    }

    [Fact]
    public void Approve_WhenReservationIsNotPending_Throws()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(1));

        Should.Throw<InvalidOperationException>(
            () => reservation.Approve(new ApproverId(10)));
    }

    [Fact]
    public void Reject_WhenReservationIsNotPending_Throws()
    {
        var reservation = CreateReservation(TimeSpan.FromHours(1));

        Should.Throw<InvalidOperationException>(
            () => reservation.Reject(new ApproverId(10)));
    }

    [Fact]
    public void Request_WhenEndIsNotAfterStart_Throws()
    {
        Should.Throw<ArgumentException>(
            () => TimeSlot.Create(StartsAt, StartsAt));
    }

    [Fact]
    public void Cancel_BeforeNoonOnSecondBusinessDayBeforeStart_CancelsReservation()
    {
        var startsAt = new DateTimeOffset(2026, 10, 20, 9, 0, 0, TimeSpan.FromHours(2));
        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            startsAt,
            requestedOn: new DateOnly(2026, 10, 12));

        reservation.Cancel(
            new DateTimeOffset(2026, 10, 16, 11, 59, 0, TimeSpan.FromHours(2)));

        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AtNoonOnSecondBusinessDayBeforeStart_Throws()
    {
        var startsAt = new DateTimeOffset(2026, 10, 20, 9, 0, 0, TimeSpan.FromHours(2));
        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            startsAt,
            requestedOn: new DateOnly(2026, 10, 12));

        Should.Throw<InvalidOperationException>(
            () => reservation.Cancel(
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.FromHours(2))));
    }

    [Fact]
    public void Cancel_SkipsSwedishPublicHolidayWhenCountingBusinessDays()
    {
        var startsAt = new DateTimeOffset(2027, 1, 8, 9, 0, 0, TimeSpan.FromHours(1));
        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            startsAt,
            requestedOn: new DateOnly(2026, 12, 30));

        reservation.Cancel(
            new DateTimeOffset(2027, 1, 5, 11, 59, 0, TimeSpan.FromHours(1)));

        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Request_WhenOverlappingPendingReservationExistsForSamePremises_Throws()
    {
        var existing = CreateReservation(TimeSpan.FromHours(8));

        Should.Throw<InvalidOperationException>(() =>
            CreateReservation(
                TimeSpan.FromHours(1),
                StartsAt.AddMinutes(30),
                existingReservations: [existing]));
    }

    [Fact]
    public void Request_WhenOverlappingConfirmedReservationExistsForSamePremises_Throws()
    {
        var existing = CreateReservation(TimeSpan.FromHours(1));

        Should.Throw<InvalidOperationException>(() =>
            CreateReservation(
                TimeSpan.FromHours(1),
                StartsAt.AddMinutes(30),
                existingReservations: [existing]));
    }

    [Fact]
    public void Request_WhenExistingReservationEndsAtNewStart_AllowsBooking()
    {
        var existing = CreateReservation(TimeSpan.FromHours(1));

        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            StartsAt.AddHours(1),
            existingReservations: [existing]);

        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Request_WhenOverlappingReservationIsForDifferentPremises_AllowsBooking()
    {
        var existing = CreateReservation(TimeSpan.FromHours(1));

        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            premisesId: new PremisesId(99),
            existingReservations: [existing]);

        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Request_WhenOverlappingReservationsAreRejectedOrCancelled_AllowsBooking()
    {
        var startsAt = new DateTimeOffset(2026, 10, 20, 9, 0, 0, TimeSpan.FromHours(2));
        var requestedOn = new DateOnly(2026, 10, 12);
        var rejected = CreateReservation(
            TimeSpan.FromHours(8),
            startsAt,
            requestedOn);
        rejected.Reject(new ApproverId(10));

        var cancelled = CreateReservation(
            TimeSpan.FromHours(1),
            startsAt,
            requestedOn);
        cancelled.Cancel(new DateTimeOffset(2026, 10, 16, 11, 59, 0, TimeSpan.FromHours(2)));

        var reservation = CreateReservation(
            TimeSpan.FromHours(1),
            startsAt,
            requestedOn,
            existingReservations: [rejected, cancelled]);

        reservation.Status.ShouldBe(ReservationStatus.Confirmed);
    }

    private static Reservation CreateReservation(
        TimeSpan duration,
        DateTimeOffset? startsAt = null,
        DateOnly? requestedOn = null,
        PremisesId? premisesId = null,
        IEnumerable<Reservation>? existingReservations = null)
    {
        var reservationStartsAt = startsAt ?? StartsAt;

        return Reservation.Request(
            new ReservationId(1),
            new BookerId(2),
            premisesId ?? new PremisesId(3),
            TimeSlot.Create(reservationStartsAt, reservationStartsAt.Add(duration)),
            requestedOn ?? RequestedOn,
            existingReservations ?? []);
    }
}
