using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ConfirmationDeliveryTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
    private readonly Room room = new(1, "Room");
    private readonly TimePeriod period = new(
        new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 11, 0, 0));

    [Fact]
    public void GivenFailedNotification_WhenRetrying_ShouldRecoverWithoutChargingAgain()
    {
        using var output = new RecoveringWriter();
        var payments = new StubPaymentGateway(true);
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();

        system.Pay(room.Id, booking.Id).Status.ShouldBe(PaymentStatus.Succeeded);
        system.GetConfirmationDelivery(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Failed);
        // A repeated payment callback must not implicitly retry delivery.
        system.RecordPaymentResult(room.Id, booking.Id, booking.PaymentAttempts.Single().Id, true);
        output.Calls.ShouldBe(1);
        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Failed);
        output.Fail = false;

        var recovered = system.RetryConfirmation(room.Id, booking.Id);

        recovered.Status.ShouldBe(ConfirmationDeliveryStatus.Sent);
        recovered.Error.ShouldBeNull();
        system.GetConfirmationDelivery(room.Id, booking.Id).ShouldBe(recovered);
        var delivered = output.ToString();
        delivered.ShouldContain("är bekräftad");
        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Sent);
        output.ToString().ShouldBe(delivered);
        payments.Attempts.Count.ShouldBe(1);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        booking.PaymentAttempts.Single().Status.ShouldBe(PaymentStatus.Succeeded);
    }

    [Fact]
    public void GivenUnknownPayment_WhenApprovalCallbackArrives_ShouldKeepNotificationFailureSeparate()
    {
        using var output = new RecoveringWriter();
        var payments = new StubPaymentGateway { ThrowOnPayment = true };
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));

        system.RecordPaymentResult(room.Id, booking.Id, booking.PaymentAttempts.Single().Id, true);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        system.GetConfirmationDelivery(room.Id, booking.Id).Error.ShouldBe("Delivery unavailable.");
        output.Fail = false;
        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Sent);
        payments.Attempts.Count.ShouldBe(1);
    }

    [Fact]
    public void GivenFailedConfirmation_WhenConcurrentRetriesArrive_ShouldSendOnce()
    {
        using var output = new RecoveringWriter();
        var payments = new StubPaymentGateway(true);
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        system.Pay(room.Id, booking.Id);
        output.Fail = false;

        Parallel.For(0, 100, _ =>
            system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Sent));

        output.ToString().ShouldBe(
            $"Till: anna@example.com{Environment.NewLine}" +
            $"Din bokning av Room är bekräftad: 2026-11-01 10:00–2026-11-01 11:00.{Environment.NewLine}");
        payments.Attempts.Count.ShouldBe(1);
    }

    [Fact]
    public void GivenCancelledBooking_WhenRetryingFailedConfirmation_ShouldSuppressDelivery()
    {
        using var output = new RecoveringWriter();
        var system = new BookingSystem(new StubPaymentGateway(true), new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        system.Pay(room.Id, booking.Id);
        system.Cancel(room.Id, booking.Id);
        output.Fail = false;

        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Suppressed);
        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Suppressed);

        output.ToString().ShouldBeEmpty();
        output.Calls.ShouldBe(1);
        booking.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public void GivenUnconfirmedOrUnknownBooking_WhenRetrying_ShouldRejectWithoutSending()
    {
        using var output = new StringWriter();
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(output));
        Should.Throw<KeyNotFoundException>(() => system.RetryConfirmation(room.Id, 1));
        Should.Throw<KeyNotFoundException>(() => system.GetConfirmationDelivery(room.Id, 1));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        system.GetConfirmationDelivery(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.NotRequested);
        Should.Throw<InvalidOperationException>(() => system.RetryConfirmation(room.Id, booking.Id));
        system.Pay(room.Id, booking.Id);
        Should.Throw<InvalidOperationException>(() => system.RetryConfirmation(room.Id, booking.Id));
        Should.Throw<KeyNotFoundException>(() => system.RetryConfirmation(room.Id, 999));
        system.GetConfirmationDelivery(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.NotRequested);
        output.ToString().ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenSlowNotification_WhenReservingAdjacentTime_ShouldNotHoldScheduleLock()
    {
        using var output = new BlockingWriter();
        var system = new BookingSystem(new StubPaymentGateway(true), new NotificationService(output));
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        var payTask = Task.Run(() => system.Pay(room.Id, booking.Id));
        try
        {
            output.Started.Wait(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken).ShouldBeTrue();
            system.GetConfirmationDelivery(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Pending);
            await Task.Run(() => system.Reserve(booker, room, new TimePeriod(period.End, period.End.AddHours(1)))
                .Outcome.ShouldBe(ReservationOutcome.Reserved), TestContext.Current.CancellationToken)
                .WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }
        finally
        {
            output.Release.Set();
            await payTask.WaitAsync(TimeSpan.FromSeconds(10), CancellationToken.None);
        }
        system.RetryConfirmation(room.Id, booking.Id).Status.ShouldBe(ConfirmationDeliveryStatus.Sent);
        output.Calls.ShouldBe(2);
    }

    private sealed class RecoveringWriter : StringWriter
    {
        public bool Fail { get; set; } = true;
        public int Calls { get; private set; }
        public override void WriteLine(string? value)
        {
            Calls++;
            if (Fail) throw new IOException("Delivery unavailable.");
            base.WriteLine(value);
        }
    }

    private sealed class BlockingWriter : StringWriter
    {
        public ManualResetEventSlim Started { get; } = new();
        public ManualResetEventSlim Release { get; } = new();
        public int Calls { get; private set; }
        public override void WriteLine(string? value)
        {
            Calls++;
            Started.Set();
            if (!Release.Wait(TimeSpan.FromSeconds(15))) throw new TimeoutException();
            base.WriteLine(value);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Started.Dispose();
                Release.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
