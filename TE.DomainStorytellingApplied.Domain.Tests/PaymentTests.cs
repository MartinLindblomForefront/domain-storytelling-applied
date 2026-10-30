using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PaymentTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
    private readonly Room room = new(1, "Meeting room");
    private readonly TimePeriod period = new(
        new DateTime(2026, 11, 1, 10, 0, 0),
        new DateTime(2026, 11, 1, 12, 0, 0));

    [Fact]
    public void GivenApprovedPayment_WhenPaying_ShouldConfirmAndNotifyOnce()
    {
        var payments = new StubPaymentGateway(true);
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        output.ToString().ShouldBeEmpty();

        var attempt = system.Pay(room.Id, booking.Id);

        attempt.Status.ShouldBe(PaymentStatus.Succeeded);
        payments.Attempts.Single().ShouldBe(attempt.Id);
        booking.PaymentAttempts.Single().ShouldBeSameAs(attempt);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        output.ToString().ShouldBe(
            $"Till: anna@example.com{Environment.NewLine}" +
            $"Din bokning av Meeting room är bekräftad: 2026-11-01 10:00–2026-11-01 12:00.{Environment.NewLine}");
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);

        var confirmation = output.ToString();
        system.RecordPaymentResult(room.Id, booking.Id, attempt.Id, true);
        output.ToString().ShouldBe(confirmation);
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
        payments.Attempts.Count.ShouldBe(1);
    }

    [Fact]
    public void GivenDeclinedPayment_WhenRetrying_ShouldKeepHistoryAndConfirmOnSuccess()
    {
        var payments = new StubPaymentGateway(false, true);
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();

        var failed = system.Pay(room.Id, booking.Id);

        failed.Status.ShouldBe(PaymentStatus.Failed);
        booking.Status.ShouldBe(BookingStatus.Reserved);
        output.ToString().ShouldBeEmpty();
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        system.RecordPaymentResult(room.Id, booking.Id, failed.Id, false);

        var succeeded = system.Pay(room.Id, booking.Id);

        succeeded.Id.ShouldNotBe(failed.Id);
        succeeded.Status.ShouldBe(PaymentStatus.Succeeded);
        booking.PaymentAttempts.Count.ShouldBe(2);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        payments.Attempts.Count.ShouldBe(2);
        var confirmation =
            $"Till: anna@example.com{Environment.NewLine}" +
            $"Din bokning av Meeting room är bekräftad: 2026-11-01 10:00–2026-11-01 12:00.{Environment.NewLine}";
        output.ToString().ShouldBe(confirmation);

        // Delayed, duplicate responses must not undo success or send another confirmation.
        system.RecordPaymentResult(room.Id, booking.Id, failed.Id, false);
        system.RecordPaymentResult(room.Id, booking.Id, succeeded.Id, true);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        output.ToString().ShouldBe(confirmation);
    }

    [Fact]
    public void GivenDeclinedPayment_WhenCancelling_ShouldFreeTimeAndRejectFurtherPayments()
    {
        var payments = new StubPaymentGateway(false);
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        var failed = system.Pay(room.Id, booking.Id);

        system.Cancel(room.Id, booking.Id);
        system.RecordPaymentResult(room.Id, booking.Id, failed.Id, false);

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        var replacement = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        replacement.Id.ShouldNotBe(booking.Id);
        replacement.Status.ShouldBe(BookingStatus.Reserved);
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
        payments.Attempts.Count.ShouldBe(1);
        output.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void GivenUnknownPaymentOutcome_WhenRetrying_ShouldBlockUntilResultIsKnown()
    {
        var payments = new StubPaymentGateway { ThrowOnPayment = true };
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();

        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));

        var attempt = booking.PaymentAttempts.Single();
        attempt.Status.ShouldBe(PaymentStatus.Pending);
        booking.Status.ShouldBe(BookingStatus.Reserved);
        output.ToString().ShouldBeEmpty();
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
        Should.Throw<InvalidOperationException>(() => system.Cancel(room.Id, booking.Id));
        payments.Attempts.Count.ShouldBe(1);
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);

        system.RecordPaymentResult(room.Id, booking.Id, attempt.Id, true);

        attempt.Status.ShouldBe(PaymentStatus.Succeeded);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        output.ToString().ShouldContain("är bekräftad");
    }

    [Fact]
    public void GivenPendingPayment_WhenStartingAnotherAttempt_ShouldReject()
    {
        var schedule = new RoomSchedule(room);
        var booking = schedule.Reserve(1, booker, period);
        var pending = schedule.StartPayment(booking.Id);

        Should.Throw<InvalidOperationException>(() => schedule.StartPayment(booking.Id));

        pending.Status.ShouldBe(PaymentStatus.Pending);
        booking.PaymentAttempts.Count.ShouldBe(1);
        schedule.RecordPaymentResult(booking.Id, pending.Id, false).ShouldBeFalse();
        var retry = schedule.StartPayment(booking.Id);
        retry.Id.ShouldNotBe(pending.Id);
        schedule.RecordPaymentResult(booking.Id, pending.Id, false).ShouldBeFalse();
        retry.Status.ShouldBe(PaymentStatus.Pending);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GivenCompletedAttempt_WhenReceivingConflictingResult_ShouldReject(bool succeeded)
    {
        var schedule = new RoomSchedule(room);
        var booking = schedule.Reserve(1, booker, period);
        var attempt = schedule.StartPayment(booking.Id);
        schedule.RecordPaymentResult(booking.Id, attempt.Id, succeeded);

        Should.Throw<InvalidOperationException>(
            () => schedule.RecordPaymentResult(booking.Id, attempt.Id, !succeeded));

        attempt.Status.ShouldBe(succeeded ? PaymentStatus.Succeeded : PaymentStatus.Failed);
        booking.Status.ShouldBe(succeeded ? BookingStatus.Confirmed : BookingStatus.Reserved);
    }

    [Fact]
    public void GivenAttemptForAnotherBooking_WhenRecordingResult_ShouldReject()
    {
        var schedule = new RoomSchedule(room);
        var first = schedule.Reserve(1, booker, period);
        var second = schedule.Reserve(2, booker, new TimePeriod(period.End, period.End.AddHours(1)));
        var attempt = schedule.StartPayment(first.Id);

        Should.Throw<KeyNotFoundException>(
            () => schedule.RecordPaymentResult(second.Id, attempt.Id, true));
        Should.Throw<KeyNotFoundException>(
            () => schedule.RecordPaymentResult(first.Id, Guid.NewGuid(), true));

        attempt.Status.ShouldBe(PaymentStatus.Pending);
        second.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenMissingRoomOrBooking_WhenPaying_ShouldNotCallGateway()
    {
        var payments = new StubPaymentGateway(true);
        var system = new BookingSystem(payments, new NotificationService(TextWriter.Null));

        Should.Throw<KeyNotFoundException>(() => system.Pay(room.Id, 1));
        system.Reserve(booker, room, period);
        Should.Throw<KeyNotFoundException>(() => system.Pay(room.Id, 999));

        payments.Attempts.ShouldBeEmpty();
    }

    [Fact]
    public void GivenNotificationFailure_WhenPaymentSucceeds_ShouldKeepConfirmationAndBlockDoublePayment()
    {
        var payments = new StubPaymentGateway(true);
        using var output = new FailingWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();

        var attempt = system.Pay(room.Id, booking.Id);

        attempt.Status.ShouldBe(PaymentStatus.Succeeded);
        var delivery = system.GetConfirmationDelivery(room.Id, booking.Id);
        delivery.Status.ShouldBe(ConfirmationDeliveryStatus.Failed);
        delivery.Error.ShouldBe("Delivery failed.");
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        booking.PaymentAttempts.Single().Status.ShouldBe(PaymentStatus.Succeeded);
        system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
        payments.Attempts.Count.ShouldBe(1);
    }

    private sealed class FailingWriter : StringWriter
    {
        public override void WriteLine(string? value) => throw new InvalidOperationException("Delivery failed.");
    }
}
