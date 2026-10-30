using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationExpiryTests
{
    private readonly ManualTimeProvider clock = new();
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
    private readonly Room room = new(1, "Meeting room");
    private readonly TimePeriod period = new(
        new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 12, 0, 0));

    [Fact]
    public void GivenReservation_WhenDeadlineReached_ShouldFreeTimeExactlyAtDeadline()
    {
        var system = CreateSystem(new StubPaymentGateway());
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        booking.ExpiresAt.ShouldBe(clock.GetUtcNow().AddMinutes(10));
        clock.Advance(TimeSpan.FromMinutes(10) - TimeSpan.FromTicks(1));
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);

        clock.Advance(TimeSpan.FromTicks(1));
        var replacement = system.Reserve(booker, room, period);

        booking.Status.ShouldBe(BookingStatus.Expired);
        replacement.Outcome.ShouldBe(ReservationOutcome.Reserved);
    }

    [Fact]
    public void GivenExpiredReservation_WhenPaying_ShouldNotCallGateway()
    {
        var payments = new StubPaymentGateway(true);
        var system = CreateSystem(payments);
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        clock.Advance(TimeSpan.FromMinutes(10));

        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));

        payments.Attempts.ShouldBeEmpty();
        booking.PaymentAttempts.ShouldBeEmpty();
        booking.Status.ShouldBe(BookingStatus.Expired);
    }

    [Fact]
    public void GivenExpiredReservation_WhenStartingPaymentDirectly_ShouldReject()
    {
        var schedule = new RoomSchedule(room, clock);
        var booking = schedule.Reserve(1, booker, period);
        clock.Advance(TimeSpan.FromMinutes(10));

        Should.Throw<InvalidOperationException>(() => schedule.StartPayment(booking.Id));

        booking.Status.ShouldBe(BookingStatus.Expired);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GivenPendingPayment_WhenResultArrivesAfterDeadline_ShouldResolveHeldTime(bool succeeded)
    {
        var payments = new StubPaymentGateway { ThrowOnPayment = true };
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output), clock);
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
        var attempt = booking.PaymentAttempts.Single();
        clock.Advance(TimeSpan.FromMinutes(11));
        system.ExpireReservations();
        booking.Status.ShouldBe(BookingStatus.Reserved);
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        Should.Throw<InvalidOperationException>(() => system.Cancel(room.Id, booking.Id));

        system.RecordPaymentResult(room.Id, booking.Id, attempt.Id, succeeded);

        booking.Status.ShouldBe(succeeded ? BookingStatus.Confirmed : BookingStatus.Expired);
        system.Reserve(booker, room, period).Outcome.ShouldBe(
            succeeded ? ReservationOutcome.RoomUnavailable : ReservationOutcome.Reserved);
        var notification = output.ToString();
        if (succeeded) notification.ShouldContain("är bekräftad");
        else notification.ShouldBeEmpty();
        system.RecordPaymentResult(room.Id, booking.Id, attempt.Id, succeeded);
        output.ToString().ShouldBe(notification);
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
    }

    [Fact]
    public void GivenDeclinedPayment_WhenDeadlinePasses_ShouldExpireWithoutExtendingDeadline()
    {
        var system = CreateSystem(new StubPaymentGateway(false));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        var deadline = booking.ExpiresAt;
        clock.Advance(TimeSpan.FromMinutes(9));
        system.Pay(room.Id, booking.Id);
        clock.Advance(TimeSpan.FromMinutes(1));

        system.ExpireReservations();

        booking.ExpiresAt.ShouldBe(deadline);
        booking.Status.ShouldBe(BookingStatus.Expired);
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.Reserved);
    }

    [Fact]
    public void GivenConfirmedOrCancelledBooking_WhenDeadlinePasses_ShouldKeepStatus()
    {
        var system = CreateSystem(new StubPaymentGateway(true));
        var confirmed = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        system.Pay(room.Id, confirmed.Id);
        var cancelled = system.Reserve(booker, new Room(2, "Other"), period).Booking.ShouldNotBeNull();
        system.Cancel(2, cancelled.Id);
        clock.Advance(TimeSpan.FromHours(1));

        system.ExpireReservations();

        confirmed.Status.ShouldBe(BookingStatus.Confirmed);
        cancelled.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public void GivenCustomDuration_WhenDeadlinePasses_ShouldExpire()
    {
        var system = new BookingSystem(new StubPaymentGateway(),
            new NotificationService(TextWriter.Null), clock, TimeSpan.FromMinutes(5));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        clock.Advance(TimeSpan.FromMinutes(5));

        system.ExpireReservations();

        booking.Status.ShouldBe(BookingStatus.Expired);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenInvalidDuration_WhenConstructing_ShouldReject(int minutes)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new RoomSchedule(room, clock, TimeSpan.FromMinutes(minutes)));
        Should.Throw<ArgumentOutOfRangeException>(() => new BookingSystem(new StubPaymentGateway(),
            new NotificationService(TextWriter.Null), clock, TimeSpan.FromMinutes(minutes)));
    }

    private BookingSystem CreateSystem(IPaymentGateway payments) =>
        new(payments, new NotificationService(TextWriter.Null), clock);
}
